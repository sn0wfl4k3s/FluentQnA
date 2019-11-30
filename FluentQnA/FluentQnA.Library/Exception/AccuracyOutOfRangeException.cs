namespace FluentQnA.Exception
{
    public class AccuracyOutOfRangeException : System.Exception
    {
        public AccuracyOutOfRangeException():base("The accuracy value should be between 0 and 1.") { }
    }
}
