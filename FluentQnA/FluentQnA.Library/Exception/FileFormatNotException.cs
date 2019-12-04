namespace FluentQnA.Exception
{
    public class FileFormatNotException : System.Exception
    {
        public FileFormatNotException() : base("File Format not exception. Use a knowledge base in json or xlsx formats.") { }
    }
}