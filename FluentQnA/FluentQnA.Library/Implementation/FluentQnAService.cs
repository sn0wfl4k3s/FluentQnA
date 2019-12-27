using FluentQnA.Exception;
using FluentQnA.Library.Model;
using FluentQnA.Models;
using IronXL;
using Microsoft.ML;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FluentQnA
{
    // nova arquitetura
    public class FluentQnAService : IFluentQnA
    {
        private MLContext _mlContext;
        private ITransformer _trainedModel;
        private IEnumerable<QnaModel> _knowledgebase;

        public string TrainedModelPath { get; set; } = "trained_model.zip";

        public FluentQnAService(string knowledgeBasePath)
        {
            _knowledgebase = LoadFromFile(knowledgeBasePath);

            TrainingModel();
        }

        private IEnumerable<QnaModel> LoadFromFile(string knowledgeBasePath)
        {
            if (!File.Exists(knowledgeBasePath))
            {
                throw new KnowledgebaseFileNotFoundException();
            }

            var extension = knowledgeBasePath.Split('.').Last();

            IList<QnaLine> qnaLines;

            switch (extension)
            {
                case "json": qnaLines = LoadFromJson(knowledgeBasePath); break;

                case "xlsx": qnaLines = LoadFromExcel(knowledgeBasePath); break;

                default: throw new FileFormatNotException();
            }

            var models = TransformInModel(qnaLines);

            return models;
        }

        private IList<QnaModel> TransformInModel(IList<QnaLine> qnaLines)
        {
            var collection = new List<QnaPreModel>();

            foreach (var line in qnaLines)
            {
                if (!string.IsNullOrEmpty(line.Answer) || !string.IsNullOrEmpty(line.Question))
                {
                    if (!collection.Any(m => m.Answer == line.Answer))
                    {
                        var qna = new QnaPreModel
                        {
                            Answer = line.Answer,
                            Questions = new List<string> { line.Question }
                        };

                        collection.Add(qna);
                    }
                    else
                    {
                        var registred = collection.Where(c => c.Answer == line.Answer).First();

                        registred.Questions.Add(line.Question);
                    }
                }
            }

            var qnaModels = collection.Select(c => new QnaModel
            {
                Answer = c.Answer,
                Questions = c.Questions.ToArray()
            }).ToList();

            return qnaModels;
        }

        private IList<QnaLine> LoadFromExcel(string knowledgeBasePath)
        {
            var workbook = WorkBook.LoadExcel(knowledgeBasePath);

            var sheet = workbook.WorkSheets.First();

            var lines = new List<QnaLine>();

            for (int i = 2; i <= sheet.Rows.Count; i++)
            {
                var range = $"A{i}:B{i}";

                var cell = sheet[range].ToArray();

                var line = new QnaLine
                {
                    Question = cell[0].ToString(),
                    Answer = cell[1].ToString()
                };

                lines.Add(line);
            }

            return lines;
        }

        private IList<QnaLine> LoadFromJson(string knowledgeBasePath)
        {
            using (var file = new StreamReader(knowledgeBasePath))
            {
                var json = file.ReadToEnd();

                return JsonConvert.DeserializeObject<IList<QnaLine>>(json);
            }
        }

        public void TrainingModel()
        {
            _mlContext = new MLContext(seed: 0);

            var dataView = _mlContext.Data.LoadFromEnumerable(_knowledgebase);

            var pipeline = _mlContext.Transforms.Conversion
                .MapValueToKey(inputColumnName: "Answer", outputColumnName: "Label")
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Questions", outputColumnName: "QuestionsFeaturized"))
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Answer", outputColumnName: "AnswerFeaturized"))
                .Append(_mlContext.Transforms.Concatenate("Features", "QuestionsFeaturized", "AnswerFeaturized"))
                .AppendCacheCheckpoint(_mlContext);

            var trainingPipeline = pipeline
                .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            _trainedModel = trainingPipeline.Fit(dataView);

            _mlContext.Model.Save(_trainedModel, dataView.Schema, TrainedModelPath);
        }

        public async Task<IEnumerable<QnAResult>> GetAnswers(string question, float? minAccuracy = 0)
        {
            if (!File.Exists(TrainedModelPath))
            {
                TrainingModel();
            }

            if (minAccuracy.HasValue && (minAccuracy < 0 || minAccuracy > 1))
            {
                throw new AccuracyOutOfRangeException(); 
            }

            _trainedModel = _mlContext.Model.Load(TrainedModelPath, out _);

            var predictEngine = _mlContext.Model.CreatePredictionEngine<QnaModel, AnswerPrediction>(_trainedModel);

            var qna = new QnaModel { Questions = new string[] { question } };

            var prediction = predictEngine.Predict(qna);

            var scores = prediction.Score as IList<float>;

            var query = _knowledgebase
                .Select((knowledge, index) => new QnAResult
                {
                    Answer = knowledge.Answer,
                    Questions = knowledge.Questions,
                    Accuracy = scores[index]
                })
                .Where(q => q.Accuracy >= (minAccuracy ?? 0))
                .OrderByDescending(q => q.Accuracy);

            return await Task.FromResult(query);
        }
    }
}