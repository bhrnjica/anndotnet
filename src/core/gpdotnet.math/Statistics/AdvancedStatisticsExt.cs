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
    /// Implement extension methods for statistics calculation between two data sets X and Y eg. sum of square error, pearson coeff,... 
    /// 
    /// </summary>
    public static partial class AdvancedStatistics
    {
        /// <summary>
        /// Calculate Classification Accuracy
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double CA(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            //calculate sum of the square residuals
            double corected = 0;
            for (int i = 0; i < obsData.Length; i++)
            {
                if(obsData[i] == preData[i])
                    corected ++;
            }
            return corected/obsData.Length;
        }
        /// <summary>
        /// Calculate Classification Error
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double CE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            //calculate sum of the square residuals
            double corected = 0;
            for (int i = 0; i < obsData.Length; i++)
            {
                if (obsData[i] == preData[i])
                    corected++;
            }
            return 1.0 - corected / obsData.Length;
        }
        /// <summary>
        /// Calculate Absolute error of two data sets
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double AE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            //calculate sum of the square residuals
            double ssr = 0;
            for (int i = 0; i < obsData.Length; i++)
            {
                var r = Abs(obsData[i] - preData[i]);
                ssr += r;
            }
            return ssr;
        }

        /// <summary>
        /// Calculates Mean Absolute error
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double MAE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            //c
            return AE(obsData, preData) / obsData.Length;
        }

        /// <summary>
        /// Calculates sum of squares of the two datasets
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double SE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            //calculate sum of the square residuals
            double ssr = 0;
            for(int i=0; i < obsData.Length; i++)
            {
                var r = (obsData[i] - preData[i]);
                ssr += r * r;
            }
               

            return ssr;
        }


        /// <summary>
        /// Calculate Mean Square Error of two data sets
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double MSE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            return SE(obsData, preData)/obsData.Length;
        }


        /// <summary>
        /// Calculate Root Square Error of two data sets
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double RSE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            return Sqrt(SE(obsData, preData));
        }

        /// <summary>
        /// Calculates Root Mean Square error
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double RMSE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            return Sqrt(SE(obsData, preData)/(double)obsData.Length);
        }


        /// <summary>
        /// Calculates Pearson correlation coefficient of two data sets
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double R(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            //calculate average for each dataset
            double aav = obsData.MeanOf();
            double bav = preData.MeanOf();

            double corr = 0;
            double ab = 0, aa = 0, bb = 0;
            for (int i = 0; i < obsData.Length; i++)
            {
                var a = obsData[i] - aav;
                var b = preData[i] - bav;

                ab += a * b;
                aa += a * a;
                bb += b * b;
            }

            corr = ab / Sqrt(aa * bb);

            return corr;
        }

        /// <summary>
        /// Calculates Coefficient of Determination (Square of Pearson coeff)
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double R2(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            var r = R(obsData, preData);
            return r*r;
        }
        /// <summary>
        /// The Nash-Sutcliffe efficiency (NSE)  proposed by Nash and Sutcliffe (1970) 
        /// is defined as one minus the sum of the absolute squared differences between 
        /// the predicted and observed values normalized by the variance of the observed
        /// values during the period under investigation.
        /// 
        /// The Nash-Sutcliffe efficiency (NSE) is a normalized statistic
        /// that determines the relative magnitude of the residual variance (“noise”) compared to the 
        /// measured data variance (“information”) (Nash and Sutcliffe, 1970). NSE indicates how well 
        /// the plot of observed versus simulated data fits the 1:1 line. 
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns>NSE ranges between −∞ and 1.0 (1 inclusive), with NSE = 1 being the optimal value. 
        /// Values between 0.0 and 1.0 are generally viewed as acceptable levels of performance, 
        /// whereas values lower than 0.0 indicates that the mean observed value is a better predictor 
        /// than the simulated value, which indicates unacceptable performance.</returns>
        public static double NSE (this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            var se = SE(obsData, preData);

            //calculate the mean
            var mean = obsData.MeanOf();
            //calculate sum of square 
            double ose = 0;
            for (int i = 0; i < obsData.Length; i++)
            {
                var res = (obsData[i] - mean);

                ose += res * res;
            }

            //calculate NSE
            var nse = 1.0 - se / ose;
            return nse;
        }

        /// <summary>
        /// Percent bias (PBIAS) measures the average tendency of the simulated data to be larger or smaller 
        /// than their observed counterparts.The optimal value of PBIAS is 0.0, 
        /// with low-magnitude values indicating accurate model simulation. 
        /// Positive values indicate model underestimation bias, and negative values indicate model 
        /// overestimation bias. 
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double PBIAS(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);
            var ae = AE(obsData, preData);
            var pbias =  ae / obsData.Sum();

            return pbias;
        }

        /// <summary>
        /// RSR standardizes RMSE using the observations standard deviation, and it combines both 
        /// an error index and the additional information recommended by Legates and McCabe (1999).
        /// RSR is calculated as the ratio of the RMSE and standard deviation of measured data.
        /// RSR incorporates the benefits of error index statistics and includes a scaling/normalization factor,
        /// so that the resulting statistic and reported values can apply to various constituents. 
        /// RSR varies from the optimal value of 0, which indicates zero RMSE or residual variation 
        /// and therefore perfect model simulation, to a large positive value. The lower RSR, the 
        /// lower the RMSE, and the better the model simulation performance.
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double RSR(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            var rmse = RMSE(obsData, preData);
            var rsr = obsData.Stdev();

            return rmse/rsr;
        }

        /// <summary>
        /// Calculate Mahanalobis distance of vector using mean and covariance matrix
        /// </summary>
        /// <param name="vector"> vector distance is calculating</param>
        /// <param name="mean">mean value </param>
        /// <param name="covMatrix">cov matrix</param>
        /// <returns></returns>
        public static double MD(double [] vector, double [] mean, double[][] covMatrix)
        {
            //create matrix from arrays
            var covM = new Matrix(covMatrix.Length, covMatrix.Length);
            var m = new Matrix(1,mean.Length);
            var v = new Matrix(1, vector.Length);
            //init matrices
            for (int i= 0; i<vector.Length; i++)
            {
                v[0, i] = vector[i];
                m[0, i] = mean[i];
                for (int j = 0; j < vector.Length; j++)
                    covM[i, j] = covMatrix[i][j];
            }
                
            
            //perform calculation
            var vm = v - m;
            var trm = Matrix.Transpose(vm);
            var tmp = (vm * covM);
            var retVal = Sqrt((tmp * trm)[0, 0]);
            //return value
            return retVal;

        }


        /// <summary>
        /// transforms the 2D row based array into 2D column based array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T[][] ToColumnVector<T>(this T[][] input)
        {
            var colVecData = new List<T[]>();
            for (int j = 0; j < input[0].Length; j++)
            {
                var col = new T[input.Length];

                for (int i = 0; i < input.Length; i++)
                {
                    col[i] = input[i][j];

                }

                colVecData.Add(col);
            }
            return colVecData.ToArray();
        }

        private static void checkDataSets(double[] obsData, double[] preData)
        {
            if (obsData == null || obsData.Length < 2)
                throw new Exception("'observed Data' cannot be null or empty!");

            if (preData == null || preData.Length < 2)
                throw new Exception("'predicted data' cannot be null or empty!");

            if (obsData.Length != preData.Length)
                throw new Exception("Both datasets must be of the same size!");
        }
    }
}
