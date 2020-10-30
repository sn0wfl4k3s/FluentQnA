using Microsoft.ML.Data;

namespace FluentQnA.Models
{
    class AnswerPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Answer;

        [ColumnName("Score")]
        public float[] Score;
    }
}