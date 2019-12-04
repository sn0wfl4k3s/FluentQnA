using FluentQnA.Exception;
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
    public class FluentQnAService : IFluentQnA
    {
        private MLContext _mlContext;
        private ITransformer _trainedModel;
        private IEnumerable<QnA> _knowledgebase;

        public string TrainedModelPath { get; set; } = "trained_model.zip";

        public FluentQnAService(string knowledgeBasePath)
        {
            LoadFromFile(knowledgeBasePath);

            TrainingModel();
        }

        private void LoadFromFile(string knowledgeBasePath)
        {
            if (!File.Exists(knowledgeBasePath))
            {
                throw new KnowledgebaseFileNotFoundException();
            }

            var extension = knowledgeBasePath.Split('.').Last();

            switch (extension)
            {
                case "json": LoadFromJson(knowledgeBasePath); break;

                case "xlsx": LoadFromExcel(knowledgeBasePath); break;

                default: throw new FileFormatNotException();
            }
        }

        private void LoadFromExcel(string knowledgeBasePath)
        {
            var workbook = WorkBook.LoadExcel(knowledgeBasePath);

            var sheet = workbook.WorkSheets.First();

            var knowledgebase = new List<QnA>();

            for (int i = 2; i <= sheet.Rows.Count; i++)
            {
                var range = $"A{i}:ZZ{i}";

                var answer = string.Empty;

                var questions = new List<string>();

                foreach (var cell in sheet[range])
                {
                    if (!string.IsNullOrEmpty(cell.Text))
                    {
                        if (cell.ColumnIndex == 0)
                        {
                            answer = cell.Text;
                        }
                        else
                        {
                            questions.Add(cell.Text);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                var qna = new QnA
                {
                    Answer = answer,
                    Questions = questions.ToArray()
                };

                knowledgebase.Add(qna);
            }

            _knowledgebase = knowledgebase;
        }

        private void LoadFromJson(string knowledgeBasePath)
        {
            using (var file = new StreamReader(knowledgeBasePath))
            {
                var json = file.ReadToEnd();

                _knowledgebase = JsonConvert.DeserializeObject<IEnumerable<QnA>>(json);
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

            var predictEngine = _mlContext.Model.CreatePredictionEngine<QnA, AnswerPrediction>(_trainedModel);

            var qna = new QnA { Questions = new string[] { question, question, question, question, question } };

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