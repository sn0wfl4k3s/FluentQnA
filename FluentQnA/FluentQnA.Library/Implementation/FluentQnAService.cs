using FluentQnA.Exception;
using FluentQnA.Models;
using Microsoft.ML;
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

        public FluentQnAService(IEnumerable<QnA> knowledgebase, bool trainModelInInit = true)
        {
            _knowledgebase = knowledgebase;

            if (trainModelInInit)
            {
                TrainingModel();
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