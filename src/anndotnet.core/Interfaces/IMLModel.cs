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

using System.Threading.Tasks;
using Tensorflow;

namespace AnnDotNet.Core.Interfaces;

public interface IMlModel
{
    /// <summary>
    /// Loads TensofrFlow model from the disk
    /// </summary>
    /// <param name="modelPath"></param>
    /// <returns></returns>
    Task<Session> LoadModelAsync(string modelPath);

    /// <summary>
    /// Save Tensorflow model to the disk
    /// </summary>
    /// <param name="session">Tensorflow session object</param>
    /// <param name="folderPath">The folder where the models should be stored</param>
    /// <returns>Returns the model name. Null or empty if the save fails. Otherwise throws the exception.</returns>
    string SaveModel(Session session, string folderPath);

    /// <summary>
    /// Create Tensorflow model from the given input and output shape
    /// </summary>
    /// <param name="shapeX">Input shape</param>
    /// <param name="shapeY">Output shape</param>
    /// <returns>Return Tensorflow graph object, otherwise throws an exception.</returns>
    Graph CreateModel(Shape shapeX, Shape shapeY);
}