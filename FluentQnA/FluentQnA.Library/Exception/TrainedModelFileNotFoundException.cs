namespace FluentQnA.Exception
{
    public class TrainedModelFileNotFoundException : System.Exception
    {
        public TrainedModelFileNotFoundException () : base("Trained model file not found.") { }
    }
}