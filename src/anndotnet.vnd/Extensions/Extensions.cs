///////////////////////////////////////////////////////////////////////////////
//               ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                           //
//                Created by anndotnet community, anndotnet.com              //
//                                                                           //
//                     Licensed under the MIT License                        //
//             See license section at https://github.com/anndotnet/anndotnet //
//                                                                           //
//             For feedback:https://github.com/anndotnet/anndotnet/issues    //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////

using System.Linq;
using System.Runtime.InteropServices;
using Tensorflow;
using Tensorflow.NumPy;
using static Tensorflow.Binding;

namespace AnnDotNet.Vnd.Extensions;

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