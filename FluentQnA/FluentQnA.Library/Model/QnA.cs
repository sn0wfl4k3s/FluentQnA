using Microsoft.ML.Data;

namespace FluentQnA.Models
{
    public class QnA
    {
        [VectorType]
        public string[] Questions { get; set; }
        public string Answer { get; set; }
    }
}