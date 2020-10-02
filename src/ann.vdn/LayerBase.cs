
namespace ann.vnd
{
    public class LayerBase
    {
        public int Id { get; set; }

        //layer type (dense, LSTM, drop, ...)
        public LayerType Type { get; set; }

        //name of a layer
        public string Name { get; set; }

        //Output dimension for the layer (Dense, Embedding, LSTM, CudaLSTM, GRU, TanH and ReLU)
        public int OutDim { get; set; }

        public Activation A { get; set; }

        public int? Seed { get; set; }
        public bool HasBias {get;set;}

        
    }
    public class LayerType
    {
    }
    public class Activation
    {
    }
}