//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using ANNdotNET.Core;
using CNTK;
using ExcelDna.Integration;
using System;
using System.Collections.Generic;
using System.Globalization;
namespace anndotnet.exceladdIn
{
    public class ModelEvaluation
    {
        //Example of usage in Excel: =ANNdotNETEval(A1:A34, modelPath)
        [ExcelFunction(Description = "ANNdotNETEval - ANNdotNET model.")]
        public static object ANNdotNETEval(object arg, string modelPath)
        {
            try
            {
                object[,] obj = null;

                if (!(arg is object[,]))
                {
                    obj = new object[1, 1];
                    obj[0, 0] = arg;
                }

                else
                    //First convert object in to array
                    obj = (object[,])arg;


                //create list to convert values
                List<float> calculatedOutput = new List<float>();

                //
                foreach (var s in obj)
                {
                    var ss = float.Parse(s.ToString(), CultureInfo.InvariantCulture);
                    calculatedOutput.Add(ss);
                }
                //
                return ANNdotNET.Core.MLEvaluator.TestModel(modelPath, calculatedOutput.ToArray(), DeviceDescriptor.UseDefaultDevice());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        
    }
}
