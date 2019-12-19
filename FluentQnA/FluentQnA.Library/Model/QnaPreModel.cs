using System.Collections.Generic;

namespace FluentQnA.Library.Model
{
    public class QnaPreModel
    {
        public IList<string> Questions { get; set; } = new List<string>();
        public string Answer { get; set; }
    }
}