namespace NeuralNetwork
{
    public interface ITrainingPoint
    {
        double[] ExpectedOutput { get; }
        double[] Input { get; }
    }
}