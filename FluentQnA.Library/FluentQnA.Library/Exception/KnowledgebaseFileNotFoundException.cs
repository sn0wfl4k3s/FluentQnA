namespace FluentQnA.Exception
{
    public class KnowledgebaseFileNotFoundException : System.Exception
    {
        public KnowledgebaseFileNotFoundException() : base("Knowledgebase file not found.") { }
    }
}
