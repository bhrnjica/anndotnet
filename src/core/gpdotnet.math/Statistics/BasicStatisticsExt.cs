//////////////////////////////////////////////////////////////////////////////////////////
// GPdotNET - Genetic Programming Tool                                                  //
// Copyright 2006-2017 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  http://github.com/bhrnjica/gpdotnet/blob/master/license.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac,Bosnia and Herzegovina                                                         //
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
    /// Implement extension methods for statistics calculation on one data set eg. mean, median, variance,... 
    /// Modul calculate mean value of array of numbers. 
    /// The mean is the average of the numbers.
    /// </summary>
    public static class BasicStatistics
    {
        /// <summary>
        /// Calculate mode value of array of numbers. Mode represent the most frequent element in the array 
        /// </summary>
        /// <param name="colData"> array of values </param>
        /// <returns>calculated mode</returns>
        public static double ModeOf(this double[] colData)
        {
            if (colData == null || colData.Length < 2)
                throw new Exception("'coldData' cannot be null or empty!");

            Dictionary<int, int> counts = new Dictionary<int, int>();
            foreach (int a in colData)
            {
                if (counts.ContainsKey(a))
                    counts[a] = counts[a] + 1;
                else
                    counts[a] = 1;
            }

            int result = int.MinValue;
            int max = int.MinValue;
            foreach (int key in counts.Keys)
            {
                if (counts[key] > max)
                {
                    max = counts[key];
                    result = key;
                }
            }

            return result;
        }
        /// <summary>
        /// Select random element from the array
        /// </summary>
        /// <param name="colData"> array of values </param>
        /// <returns>random element</returns>
        public static double RandomOf(this double[] colData)
        {
            if (colData == null || colData.Length < 2)
                throw new Exception("'coldData' cannot be null or empty!");

            Random rand = new Random((int)DateTime.Now.Ticks);
            var randIndex= rand.Next(0,colData.Length);
            return colData[randIndex];
        }
        /// <summary>
        /// Calculate mean value of array of numbers. 
        /// </summary>
        /// <param name="colData"> array of values </param>
        /// <returns>calculated mean</returns>
        public static double MeanOf(this double[] colData)
        {
            if (colData == null || colData.Length < 2)
                throw new Exception("'coldData' cannot be null or empty!");

            //calculate summ of the values
            double sum = 0;
            for(int i=0; i < colData.Length; i++)
                sum += colData[i];

            //calculate mean
            double retVal = sum / colData.Length;

            return retVal;
        }

        /// <summary>
        /// Calculate median value of array of numbers. 
        /// If there is an odd number of data values 
        /// then the median will be the value in the middle. 
        /// If there is an even number of data values the median 
        /// is the mean of the two data values in the middle. 
        /// For the data set 1, 1, 2, 5, 6, 6, 9 the median is 5. 
        /// For the data set 1, 1, 2, 6, 6, 9 the median is 4.
        /// </summary>
        /// <param name="colData"></param>
        /// <returns></returns>
        public static double MedianOf(this double[] colData)
        {
            if (colData == null || colData.Length < 2)
                throw new Exception("'coldData' cannot be null or empty!");

            //initial mean value
            double median = 0;
            int medianIndex = colData.Length / 2;

            //make a deep copy of the data
            var temp = new double[colData.Length];
            Array.Copy(colData, temp, colData.Length);
            //sort the values

            Array.Sort(temp);

            //in case we have odd number of elements in data set
            // median is just in the middle of the dataset
            if(temp.Length % 2 == 1)
            {
                // 
                median = temp[medianIndex];
            }
            else//otherwize calculate average between two element in the middle
            {
                var val1 = temp[medianIndex - 1];
                var val2 = temp[medianIndex];

                //calculate the median
                median = (val1 + val2) / 2; 
            }

            return median;
        }

        /// <summary>
        /// Calculate variance for the sample data .
        /// </summary>
        /// <param name="colData"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static double VarianceOfS(this double[] colData)
        {
            if (colData == null || colData.Length < 3)
                throw new Exception("'coldData' cannot be null or less than 4 elements!");
            
            //number of elements
            int count = colData.Length;

            //calculate the mean
            var mean = colData.MeanOf();

            //calculate summ of square 
            double parSum = 0;
            for (int i = 0; i < colData.Length; i++)
            {
                var res = (colData[i] - mean);

                parSum += res*res;
            }
                
            return parSum/(count-1);
        }
        /// <summary>
        /// Calculation covariance brtween two vectors
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static double Covariance(this double[] X, double[] Y)
        {
            if (X == null || X.Length < 2 || Y == null || Y.Length < 2)
                throw new Exception("'data' cannot be null or less than 4 elements!");

            var mx = X.MeanOf();
            var my = Y.MeanOf();
            double Sxy = 0;
            for (int i = 0; i < X.Length; i++)
            {
                Sxy += (X[i] - mx) * (Y[i] - my);
            }
            //divide by number of samples -1
            Sxy = Sxy / (X.Length - 1);
            //

            return Sxy;
        }
        /// <summary>
        /// Calculation of covariance matrix and return as 2D arry
        /// </summary>
        /// <param name="Xi">arbitrary number of vectors </param>
        /// <returns></returns>
        public static double[][] Covariance( IList<double[]> Xi)
        {
            //
            var covMat = CovMatrix(Xi);
            
            var mat = new double[Xi.Count][];
            for (int i = 0; i < Xi.Count; i++)
            {
                mat[i] = new double[Xi.Count];
                for (int j = 0; j < Xi.Count; j++)
                    mat[i][j] = covMat[i, j];
            }

            return mat;
        }

        /// <summary>
        /// Calculation of covariance matrix and return Matrix type
        /// </summary>
        /// <param name="Xi">arbitrary number of vectors </param>
        /// <returns></returns>
        public static Matrix CovMatrix(IList<double[]> Xi)
        {

            if (Xi == null || Xi.Count < 2)
                throw new Exception("'data' cannot be null or less than 4 elements!");
            //
            Matrix matrix = new Matrix(Xi.Count, Xi.Count);
            //
            for (int i = 0; i < Xi.Count; i++)
            {
                for (int j = 0; j < Xi.Count; j++)
                {
                    if (i > j)
                        matrix[i, j] = matrix[j, i];
                    else if (i == j)
                        matrix[i, j] = VarianceOfS(Xi[i]);
                    else
                        matrix[i, j] = Covariance(Xi[i], Xi[j]);
                }
            }

            //inverse matrix
            try
            {
                var covMat = matrix.Invert();
                return covMat;
            }
            catch
            {

               return Matrix.IdentityMatrix(Xi.Count, Xi.Count); 
            }
                
          
        }

        /// <summary>
        /// Calculate StandardDeviation
        /// </summary>
        /// <param name="colData"></param>
        /// <returns></returns>
        public static double Stdev(this double[] colData)
        {
            if (colData == null || colData.Length < 3)
                throw new Exception("'coldData' cannot be null or less than 3 elements!");

            //number of elements
            int count = colData.Length;

            //calculate the mean
            var vars = colData.VarianceOfS();

            //calculate summ of square 
            return Sqrt(vars);
        }

        /// <summary>
        /// Calculate variance for the whole population.
        /// </summary>
        /// <param name="colData"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static double VarianceOfP(this double[] colData)
        {
            if (colData == null || colData.Length < 4)
                throw new Exception("'coldData' cannot be null or less than 4 elements!");

            //number of elements
            int count = colData.Length;

            //calculate the mean
            var mean = colData.MeanOf();

            //calculate summ of square 
            double parSum = 0;
            for (int i = 0; i < colData.Length; i++)
            {
                var res = (colData[i] - mean);

                parSum += res * res;
            }

            return parSum / count;
        }

        /// <summary>
        /// Calculates the minimum and maximum value for each column in dataset
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns>tuple where the first value is MIN, and second value is MAX</returns>
        public static Tuple<double[], double[]> calculateMinMax(this double[][] dataset)
        {
            //
            if (dataset == null || dataset.Length == 0)
                throw new Exception("data cannot be null or empty!");

            var minMax = new Tuple<double[], double[]>( new double[dataset[0].Length], new double[dataset[0].Length]);


            for (int i = 0; i < dataset.Length; i++)
            {
                for (int j = 0; j < dataset[0].Length; j++)
                {
                    if (dataset[i][j] > minMax.Item2[j])
                        minMax.Item2[j] = dataset[i][j];

                    if (dataset[i][j] < minMax.Item1[j])
                        minMax.Item1[j] = dataset[i][j];
                }
            }

            return minMax;
        }

        /// <summary>
        /// Calculate Mean and Standard deviation
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns>return tuple of mean and StDev</returns>
        public static Tuple<double[], double[]> calculateMeanStDev(this double[][] dataset)
        {
            //
            if (dataset == null || dataset.Length <= 1)
                throw new Exception("data cannot be null or empty!");

            var meanStdev = new Tuple<double[], double[]>(new double[dataset[0].Length], new double[dataset[0].Length]);
            double[] means = new double[dataset[0].Length];
            double[] stdevs = new double[dataset[0].Length];

            //first calculate means
            for (int i = 0; i < dataset.Length; i++)
            {
                for (int j = 0; j < dataset[0].Length; j++)
                { 
                    means[j] += dataset[i][j];
                }
            }
            //devide by number of rows
            for (int i = 0; i < means.Length; i++)
            {
                means[i] = means[i]/ dataset.Length;
            }

            //calculate standard deviation
            for (int i = 0; i < dataset.Length; i++)
            {
                for (int j = 0; j < dataset[0].Length; j++)
                {
                    var v = dataset[i][j] - means[j];
                    stdevs[j] += v*v;
                }
            }

            //calculate stdev
            for (int i = 0; i < means.Length; i++)
            {
                stdevs[i] = Sqrt(stdevs[i]/( dataset.Length - 1));
            }

            return new Tuple<double[], double[]>(means, stdevs);
        }
    }
}
