//////////////////////////////////////////////////////////////////////////////////////////
// GPdotNET - Genetic Programming Tool                                                  //
// Copyright 2006-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  http://github.com/bhrnjica/gpdotnet/blob/master/license.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.wordpress.com                                                        //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Math;
namespace GPdotNet.MathStuff
{
    /// <summary>
    /// Implement extension methods for statistics calculation between two 2d data sets X and Y eg. sum of square error, pearson coeff,... 
    /// 
    /// </summary>
    public static partial class AdvancedStatistics
    {
        public static double AE(double[][] obsData, double[][] preData)
        {
            checkDataSets(obsData, preData);

            //calculate sum of the square residuals
            List<double> ssr = new List<double>();
            for (int i = 0; i < obsData[0].Length; i++)
                ssr.Add(0);

            for (int i = 0; i < obsData.Length; i++)
            {
                for(int j=0; j< obsData[0].Length;j++)
                {
                    var r = Abs(obsData[i][j] - preData[i][j]);
                    ssr[j] += r;
                }
                
            }
            return ssr.Average();
        }

        public static double SE(double[][] obsData, double[][] preData)
        {
            var ssr = SEEx(obsData, preData);

            return ssr.Average();
        }

        private static List<double> SEEx(double[][] obsData, double[][] preData)
        {
            checkDataSets(obsData, preData);
            //calculate sum of the square residuals
            List<double> ssr = new List<double>();
            for (int j = 0; j < obsData[0].Length; j++)
                ssr.Add(0);
            for (int i = 0; i < obsData.Length; i++)
            {
                for (int j = 0; j < obsData[i].Length; j++)
                {
                    var r = (obsData[i][j] - preData[i][j]);
                    ssr[j] += r * r;
                }

            }
            //
            return ssr;
        }

        public static double MSE(double[][] obsData, double[][] preData)
        {
            checkDataSets(obsData, preData);

            var rCount = obsData.Length + obsData[0].Length;

            return SE(obsData, preData) / (double)obsData.Length;
        }

        public static double RSE(double[][] obsData, double[][] preData)
        {
            checkDataSets(obsData, preData);

            return Math.Sqrt(SE(obsData, preData));
        }

        public static double RMSE(double[][] obsData, double[][] preData)
        {
            checkDataSets(obsData, preData);

            var rCount = obsData.Length + obsData[0].Length;

            return Math.Sqrt(SE(obsData, preData) / (double)rCount);
        }
        public static double NSE(this double[][] obsData, double[][] preData)
        {
            checkDataSets(obsData, preData);

            var se = SEEx(obsData, preData);

            //calculate the mean
            var mean = MeanOf(obsData);
            //calculate sum of square 
            List<double> ose = new List<double>();
            for (int j = 0; j < obsData[0].Length; j++)
                ose.Add(0);


            for (int i = 0; i < obsData.Length; i++)
            {
                for(int j=0; j< obsData[i].Length; j++)
                {
                    var res = (obsData[i][j] - mean[j]);

                    ose[j] += res * res;
                }
                
            }

            //calculate NSE
            var nse = new List<double>();
            for (int j = 0; j < obsData[0].Length; j++)
                nse.Add(1.0 - se[j] / ose[j]);

            return nse.Average();
        }

        public static double PBIAS(double[][] obsData, double[][] preData)
        {
            checkDataSets(obsData, preData);
            var ae = AE(obsData, preData);
            List<double> pbiases = new List<double>();

            for(int i=0; i<obsData[0].Length; i++)
            {
                pbiases.Add(ae / obsData[i].Sum());
            }
            

            return pbiases.Average();
        }


        public static double R(double[][] obsData, double[][] preData)
        {
            checkDataSets(obsData, preData);

            //calculate average for each dataset
            var aav = MeanOf(obsData);
            var bav = MeanOf(preData);

            var ab = new List<double>();
            var aa = new List<double>();
            var bb = new List<double>();
            for (int j = 0; j < obsData[0].Length; j++)
            {
                ab.Add(0);
                aa.Add(0);
                bb.Add(0);
            }

            for (int i = 0; i < obsData.Length; i++)
            {
                for (int j = 0; j < obsData[i].Length; j++)
                {
                    var a = obsData[i][j] - aav[j];
                    var b = preData[i][j] - bav[j];

                    ab[j] += a * b;
                    aa[j] += a * a;
                    bb[j] += b * b;
                }

            }
            var corr = new List<double>();
            for (int j = 0; j < obsData[0].Length; j++)
            {
                corr.Add(ab[j] / Math.Sqrt(aa[j] * bb[j]));
            }

            return corr.Average();
        }

        public static double R2(double[][] obsData, double[][] preData)
        {
            checkDataSets(obsData, preData);

            var r = R(obsData, preData);
            return r * r;
        }

        public static double[] MeanOf(double[][] mcolData)
        {
            if (mcolData == null || mcolData.Length < 2)
                throw new Exception("'coldData' cannot be null or empty!");
            if (mcolData[0].Length < 1)
                throw new Exception("'coldData' cannot be null or empty!");

            //calculate summ of the values
            List<double> sum = new List<double>();
            for (int j = 0; j < mcolData[0].Length; j++)
                sum.Add(0);

            ///
            for (int i = 0; i < mcolData.Length; i++)
            {
                for (int j = 0; j < mcolData[i].Length; j++)
                {
                    sum[j] += mcolData[i][j];
                }
            }


            //calculate mean
            for (int j = 0; j < mcolData[0].Length; j++)
                sum[j] = sum[j] / (double)mcolData.Length;

            return sum.ToArray();
        }

        public static double[] StdOf(double[][] mcolData)
        {
            if (mcolData == null || mcolData.Length < 2)
                throw new Exception("'coldData' cannot be null or empty!");
            if (mcolData[0].Length < 1)
                throw new Exception("'coldData' cannot be null or empty!");

            //calculate summ of the values
            var stds = new List<double>();
            var cols = mcolData.ToColumnVector();
            foreach (var v in cols)
                stds.Add(v.Stdev());

            return stds.ToArray();
        }

        public static double[] MinOf(double[][] mcolData)
        {
            if (mcolData == null || mcolData.Length < 2)
                throw new Exception("'coldData' cannot be null or empty!");
            if (mcolData[0].Length < 1)
                throw new Exception("'coldData' cannot be null or empty!");

            //calculate summ of the values
            var min = new List<double>();
            var cols = mcolData.ToColumnVector();
            foreach (var v in cols)
                min.Add(v.Min());

            return min.ToArray();
        }
        public static double[] MaxOf(double[][] mcolData)
        {
            if (mcolData == null || mcolData.Length < 2)
                throw new Exception("'coldData' cannot be null or empty!");
            if (mcolData[0].Length < 1)
                throw new Exception("'coldData' cannot be null or empty!");

            //calculate summ of the values
            var max = new List<double>();
            var cols = mcolData.ToColumnVector();
            foreach (var v in cols)
                max.Add(v.Max());

            return max.ToArray();
        }
        private static void checkDataSets(double[][] obsData, double[][] preData)
        {
            if (obsData == null || obsData.Length < 2)
                throw new Exception("'observed Data' cannot be null or empty!");


            if (preData == null || preData.Length < 2)
                throw new Exception("'predicted data' cannot be null or empty!");

            if (obsData.Length != preData.Length)
                throw new Exception("Both datasets must be of the same size!");

            if (obsData[0].Length != preData[0].Length)
                throw new Exception("Both datasets must be of the same size!");
        }
    }
}
