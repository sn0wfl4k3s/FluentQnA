using Microsoft.ML.Data;

namespace FluentQnA.Models
{
    public class QnA
    {
        [VectorType]
        [LoadColumn(0)]
        public string[] Questions { get; set; }
        [LoadColumn(1)]
        public string Answer { get; set; }
    }
}