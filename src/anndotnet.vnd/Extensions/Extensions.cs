using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tensorflow;
using static Tensorflow.Binding;
using static Microsoft.FSharp.Core.ByRefKinds;
using Tensorflow.NumPy;

namespace Anndotnet.Vnd.Extensions
{
    public static class Extensions
    {
        public static string GetPathInCurrentOS(this string path)
        {
            if (System.OperatingSystem.IsOSPlatform(OSPlatform.Windows.ToString()))
                return path.Replace("/", "\\");
            else
                return path.Replace("\\", "/");
        }
        
        public static (NDArray Xpred, NDArray Ypred) GetRandomSample(NDArray X, NDArray Y, int count)
        {
            var indx = tf.range(X.shape[0]);
            var ridxs = tf.random_shuffle(indx).numpy().Take(count);
            var rinput = tf.gather(X, new Tensor(ridxs.ToArray()));

            //idxs = tf.range(tf.shape(inputs)[0])
            //ridxs = tf.random.shuffle(idxs)[:sample_num]
            //rinput = tf.gather(inputs, ridxs)
            return (rinput.numpy(), rinput.numpy());
        }
    }
}
