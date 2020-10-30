namespace FluentQnA.Library.Exception
{
    public class TrainedModelNotZipException : System.Exception
    {
        public TrainedModelNotZipException() : base("Trained model file is not a zip") { }
    }
}
