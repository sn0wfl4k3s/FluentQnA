using Microsoft.ML.Data;

namespace FluentQnA.Models
{
    public class AnswerPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Answer;

        [ColumnName("Score")]
        public float[] Score;
    }
}