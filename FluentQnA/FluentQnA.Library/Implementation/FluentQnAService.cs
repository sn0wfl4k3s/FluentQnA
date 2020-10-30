using FluentQnA.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FluentQnA
{
    public class FluentQnAService : IFluentQnA
    {
        private MLContext _mlContext;
        private ITransformer _trainedModel;

        public string TrainedModelPath { get; set; } = "trainedModel.zip";
        public IEnumerable<QnA> Knowledgebase { get; set; }


        public FluentQnAService(IEnumerable<QnA> knowledgebase)
        {
            Knowledgebase = knowledgebase;
        }

        #region TrainingModel
        public void TrainingModel()
        {
            _trainedModel = GetTrainedModel();
        }
        public Task TrainingModelAsync()
        {
            _trainedModel = GetTrainedModel();

            return Task.CompletedTask;
        }
        #endregion
        
        #region GetAnswer
        public QnAResult GetAnswer(string question)
        {
            var predictEngine = GetPredictEngine();

            var answerPrediction = predictEngine.Predict(new QnA { Question = question });

            var qnaResult = new QnAResult
            {
                Accuracy = answerPrediction.Score.First(),
                Answer = answerPrediction.Answer
            };

            return qnaResult;
        }
        public async Task<QnAResult> GetAnswerAsync(string question)
        {
            var answer = GetAnswer(question);

            return await Task.FromResult(answer);
        }
        #endregion

        #region GetAnswers
        public IEnumerable<QnAResult> GetAnswers(string question)
        {
            var predictEngine = GetPredictEngine();

            var result = predictEngine.Predict(new QnA { Question = question });

            // https://github.com/dotnet/docs/issues/14265
            // https://stackoverflow.com/questions/53266283/ml-net-0-7-get-scores-and-labels-for-multiclassclassification

            var column = predictEngine.OutputSchema.GetColumnOrNull("Label") ?? throw new ArgumentNullException($"column:Label");

            var vbuffer = new VBuffer<ReadOnlyMemory<char>>();

            column.GetKeyValues(ref vbuffer);

            var results = vbuffer
                .DenseValues()
                .Select((label, index) => (Score: result.Score[index], Label: label.ToString()))
                .OrderByDescending(t => t.Score)
                .Select(t => new QnAResult { Answer = t.Label, Accuracy = t.Score })
                .ToImmutableArray();

            return results;
        }
        public async Task<IEnumerable<QnAResult>> GetAnswersAsync(string question)
        {
            var results = GetAnswers(question);

            return await Task.FromResult(results);
        }
        #endregion

        #region private methods
        private ITransformer GetTrainedModel()
        {
            _mlContext = new MLContext(seed: 0);

            var dataView = _mlContext.Data.LoadFromEnumerable(Knowledgebase);

            var pipeline = _mlContext.Transforms.Conversion
                .MapValueToKey(inputColumnName: "Answer", outputColumnName: "Label")
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Question", outputColumnName: "QuestionFeaturized"))
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Answer", outputColumnName: "AnswerFeaturized"))
                .Append(_mlContext.Transforms.Concatenate("Features", "QuestionFeaturized", "AnswerFeaturized"))
                .AppendCacheCheckpoint(_mlContext);

            var trainingPipeline = pipeline
                .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            _trainedModel = trainingPipeline.Fit(dataView);

            _mlContext.Model.Save(_trainedModel, dataView.Schema, TrainedModelPath);

            return _trainedModel;
        }

        private PredictionEngine<QnA, AnswerPrediction> GetPredictEngine()
        {
            if (!File.Exists(TrainedModelPath))
            {
                _trainedModel = GetTrainedModel();
            }

            _trainedModel = _mlContext.Model.Load(TrainedModelPath, out _);

            var predictEngine = _mlContext.Model.CreatePredictionEngine<QnA, AnswerPrediction>(_trainedModel);

            return predictEngine;
        }
        #endregion
    }
}