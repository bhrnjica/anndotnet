using Tensorflow.NumPy;

namespace AnnDotNET.Tool
{
    public class LR
    {
        static NDArray dsX;
        static NDArray dsY;
        static int nSamples;
        static (NDArray x, NDArray y) PrepareData()
        {
            // Prepare training Data
            dsX = np.array(3.3f, 4.4f, 5.5f, 6.71f, 6.93f, 4.168f, 9.779f, 6.182f, 7.59f, 2.167f, 7.042f, 10.791f, 5.313f, 7.997f, 5.654f, 9.27f, 3.1f);
            dsY = np.array(1.7f, 2.76f, 2.09f, 3.19f, 1.694f, 1.573f, 3.366f, 2.596f, 2.53f, 1.221f, 2.827f, 3.465f, 1.65f, 2.904f, 2.42f, 2.94f, 1.3f);
            nSamples =(int) dsX.shape.dims[0];

            return (dsX,dsY); 
        }
    }
}
