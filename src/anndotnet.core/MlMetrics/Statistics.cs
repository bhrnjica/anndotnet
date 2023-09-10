////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace Anndotnet.Core.MlMetrics;

//Statistic for Column
public class Statistics
{
    public double Mean;
    public double Median;
    public double Mode;
    public double Random;
    public double Range;
    public double Min;
    public double Max;
    public double StdDev;
    public List<string> Categories;
}