//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
// Copyright 2017-2020 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////

using Tensorflow;

namespace Anndotnet.Core.Interface
{
    public interface IMLModel
    {
        /// <summary>
        /// Loads TensofrFlow model from the disk
        /// </summary>
        /// <param name="modelPath"></param>
        /// <returns></returns>
        Session LoadModel(string modelPath);

        /// <summary>
        /// Save Tensorflow model to the disk
        /// </summary>
        /// <param name="session">Tensorflow session object</param>
        /// <param name="folderPath">The folder where the models should be stored</param>
        /// <returns>Returns the model name. Null or empty if the save failes. Otherwize thors the exception.</returns>
        string SaveModel(Session session, string folderPath);

        /// <summary>
        /// Create Tensorflow model from the given input and output shape
        /// </summary>
        /// <param name="shapeX">Input shape</param>
        /// <param name="shapeY">Output shape</param>
        /// <returns>Return Tensorflow graph object, otherwize throws an excepton.</returns>
        Graph CreateModel(Shape shapeX, Shape shapeY);
    }
}
