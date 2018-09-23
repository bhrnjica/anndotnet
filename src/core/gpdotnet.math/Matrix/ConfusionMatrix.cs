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
using System.Text;
using System.Threading.Tasks;

namespace GPdotNet.MathStuff
{
    /// <summary>
    /// Implementation of Confusion matrix for n-classes 
    /// </summary>
    public class ConfusionMatrix
    {
        #region Init
        int[][] m_matrix;
        public int [][] Matrix { get=>m_matrix;}
        /// <summary>
        /// Matrix construction based on pre calculated values
        /// </summary>
        /// <param name="matrix"></param>
        public ConfusionMatrix(int [][] matrix)
        {
            if (matrix == null && matrix.Length != matrix[0].Length)
                throw new Exception("Confusion matrix cannot be created with invalid data!");
            m_matrix = matrix;
        }

        /// <summary>
        /// Confusion matrix constructor
        /// </summary>
        /// <param name="observed"></param>
        /// <param name="predicted"></param>
        /// <param name="classCount"></param>
        public ConfusionMatrix (int [] observed, int [] predicted, int classCount)
        {
            if (observed == null || predicted == null || observed.Length == 0 || observed.Length != predicted.Length)
                throw new Exception("Cannot create Confusion matrix. Invalid data.");
            //reserve space for matrix
            m_matrix = new int[classCount][];
            for (int i = 0; i < classCount; i++)
                m_matrix[i] = new int[classCount];

            //calculate confusion matrix
            for (int row = 0; row < observed.Length; row++)
            {
                //retrieve the values
                var o = observed[row];
                var p = predicted[row];

                ProcessRowValue(m_matrix,o, p);
            }
            
        }

        #endregion

        #region basic calculation of Confusion matrix
        /// <summary>
        /// True Positive for class Ci, i-zero based index.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="classIndex">zero based index</param>
        /// <returns></returns>
        public static int TPi(int[][] matrix, int classIndex = 1)
        {
            return matrix[classIndex][classIndex];
        }
        /// <summary>
        /// False Positive for class Ci, i-zero based index in case of MCC.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="classIndex">i-zero based index</param>
        /// <returns></returns>
        public static int FPi(int[][] matrix, int classIndex = 1)
        {
            int counter = 0;
            //
            for (int i = 0; i < matrix.Length; i++)
            {
                if (i == classIndex)
                    continue;
                counter += matrix[i][classIndex];
            }
           return counter;
        }
        /// <summary>
        /// False Negative for class Ci,i-zero based index in case of MCC.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="classIndex">i-zero based index</param>
        /// <returns></returns>
        public static int FNi(int[][] matrix, int classIndex = 1)
        {
            int counter = 0;
            //
            for (int i = 0; i < matrix.Length; i++)
            {
                if (i == classIndex)
                    continue;
                counter += matrix[classIndex][i];
            }
            return counter;
        }
        /// <summary>
        /// True Negative for class Ci,i-zero based index in case of MCC. 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="classIndex"></param>
        /// <returns></returns>
        public static int TNi(int[][] matrix, int classIndex = 1)
        {
            int counter = 0;
            //
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix.Length; j++)
                {
                    if (i == classIndex || j == classIndex)
                        continue;
                    counter += matrix[i][j];
                }
                  
            }
            return counter;
        }

        /// <summary>
        /// Calculate overall accuracy
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double OAC(int[][] matrix)
        {
            double acc = 0;
            double tmp = 0;
            double count = 0;
            //
            for (int k = 0; k < matrix.Length; k++)
            {
                tmp += matrix[k][k];
                for (int i = 0; i < matrix.Length; i++)
                    count += matrix[k][i];

            }
            acc = tmp / count;

            return acc;

            //var tpi = 0.0;
            //var tni = 0.0;
            //var fni = 0.0;
            //var fpi = 0.0;
            //for (int i = 0; i < matrix.Length; i++)
            //{
            //    tpi += TPi(matrix, i);
            //    tni += TNi(matrix, i);
            //    fni += FNi(matrix, i);
            //    fpi += FPi(matrix, i);
            //}

            //return (tpi + tni) / (tpi + tni + fni + fpi);
        }
        /// <summary>
        /// Average Accuracy,The average per-class effectiveness of a classifier
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double AAC(int[][] matrix)
        {
            var tpi = 0.0;
            var tni = 0.0;
            var fni = 0.0;
            var fpi = 0.0;
            var sum = 0.0;
            for (int i = 0; i < matrix.Length; i++)
            {
                tpi = TPi(matrix, i);
                tni = TNi(matrix, i);
                fni = FNi(matrix, i);
                fpi = FPi(matrix, i);

                sum += (tpi + tni) / (tpi + tni + fni + fpi);
            }

            return sum / matrix.Length;
        }

        /// <summary>
        /// Overall Error rate
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static double Error(int[][] matrix)
        {
            return 1 - OAC(matrix);
        }

        /// <summary>
        /// Average Error rate
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static double AError(int[][] matrix)
        {
            return 1 - AAC(matrix);
        }

        /// <summary>
        /// Process the observed and predicted row value, and put in to the right place of confusion matrix 
        /// </summary>
        /// <param name="matrix">confusion matrix</param>
        /// <param name="oValue">observed value</param>
        /// <param name="pValue">predicted value</param>
        public static void ProcessRowValue(int[][] matrix, int oValue, int pValue)
        {
            //
            for (int j = 0; j < matrix.Length; j++)
            {
                for (int k = 0; k < matrix.Length; k++)
                {
                    if (pValue == k && oValue == j)
                        ++matrix[j][k];
                }
            }
        }
        #endregion
        
        #region Precision
        /// <summary>
        /// Micro Recall value calculate based on given confusion matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double MicroPrecision(int[][] matrix)
        {
            double sumTpi = 0;
            double sumtpi_fpi = 0;
            for (int i = 0; i < matrix.Length; i++)
            {
                var tpi = TPi(matrix, i);
                var fpi = FPi(matrix,i);
                sumTpi += tpi;
                sumtpi_fpi += (tpi + fpi);
            }

            if (sumtpi_fpi == 0)
                return 1;
            else
                return sumTpi / sumtpi_fpi;
        }

        /// <summary>
        /// Macro Precision value: An average per-class agreement of the data class labels with those of a classifiers
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double MacroPrecision(int[][] matrix)
        {
            double sum = 0;

            for (int i = 0; i < matrix.Length; i++)
            {
                sum += Precision(matrix, i);
            }
            return sum / matrix.Length;
        }
        /// <summary>
        /// Positive Predicted Value, Precision, a/a+c
        /// Calculate precision for the i th Class
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double Precision(int[][] matrix, int classIndex)
        {
            double tpi = TPi(matrix, classIndex);
            double fpi = FPi(matrix, classIndex);

            if ((tpi + fpi) == 0)
                return 1;
            else
                return  tpi / (tpi + fpi);
        }
        #endregion

        #region Recall
        /// <summary>
        /// Micro Recall value calculate based on given confusion matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="classCount"></param>
        /// <returns></returns>
        public static double MicroRecall(int[][] matrix)
        {
            double sumTpi = 0;
            double sumtpi_fni = 0;
            for (int i = 0; i < matrix.Length; i++)
            {
                var tpi = TPi(matrix, i);
                var fpi = FNi(matrix, i);
                sumTpi += tpi;
                sumtpi_fni += (tpi + fpi);
            }

            if (sumtpi_fni == 0)
                return 1;
            else
                return sumTpi / sumtpi_fni;
        }

        
        /// <summary>
        /// Macro Recall value calculate based on given confusion matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="classCount"></param>
        /// <returns></returns>
        public static double MacroRecall(int[][] matrix)
        {
            double sum = 0;

            for (int i = 0; i < matrix.Length; i++)
            {
                sum += Recall(matrix, i);
            }
            return sum / matrix.Length;
        }

        /// <summary>
        /// True positive rate, Sensitivity, Hit Rate or Recall, a/a+c
        /// Recall for i th Class
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double Recall(int[][] matrix, int classIndex)
        {
            double tpi = TPi(matrix, classIndex);
            double fni = FNi(matrix, classIndex);

            if ((tpi + fni) == 0)
                return 1;
            else
                return tpi / (tpi + fni);
        }


        #endregion

        #region FScore
        public static double MacroFscore(int[][] matrix)
        {
            var p = MacroPrecision(matrix);
            var r = MacroRecall(matrix);

            if ((p + r) == 0)
                return 0;

           return  2 * p * r/( p + r);
        }
        /// <summary>
        /// Micro Recall value calculate based on given confusion matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="classCount"></param>
        /// <returns></returns>
        public static double MicroFscore(int[][] matrix)
        {
            var p = MicroPrecision(matrix);
            var r = MicroRecall(matrix);

            if ((p + r) == 0)
                return 0;

            return 2 * p * r / (p + r);
        }

        /// <summary>
        /// Fscore for i th class
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double Fscore(int[][] matrix, int classIndex=1)
        {

            var p = Precision(matrix, classIndex);
            var r = Recall(matrix, classIndex);

            if ((p + r) == 0)
                return 0;

            return 2 * p * r / (p + r);
        }
        #endregion

        #region Specificity
        /// <summary>
        /// True Negative Rate, Specificity, d/b+d
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="negativeValueIndex">Index in Confusion Matrix of Negative Value</param>
        /// <returns></returns>
        public static double Specificity(int[][] matrix, int negativeValueIndex = 0)
        {
            double a = matrix[negativeValueIndex][negativeValueIndex];
            double tmp = 0;
            //
            for (int k = 0; k < matrix.Length; k++)
            {
                tmp += matrix[negativeValueIndex][k];
            }
            if (tmp == 0)
                return 1;

            var ppv = a / tmp;

            return ppv;
        }
        #endregion

        #region Skill Scores 
        /// <summary>
        /// Heidke Skill Score calculation for n-classes
        /// </summary>
        /// <param name="matrix">Confusion matrix</param>
        /// <param name="count">total number of observation/predictions </param>
        /// <returns></returns>
        public static double HSS(int[][] matrix, int count)
        {
            double NC = 0;
            double T = count;
            double E = 0;
            //
            double[] xip = new double[matrix.Length];
            double[] xpi = new double[matrix.Length];
            //
            for (int k = 0; k < matrix.Length; k++)
            {
                NC += matrix[k][k];

                for (int i = 0; i < matrix.Length; i++)
                {
                    xip[k] += matrix[k][i];
                    xpi[k] += matrix[i][k];
                }

                E += xip[k] * xpi[k];
            }

            E = E / T;

            var hss = ((NC - E) / (T - E));
            return hss;
        }
        /// <summary>
        /// Pierce Skill Score calculation
        /// </summary>
        /// <param name="matrix">confusion matrix</param>
        /// <param name="count">total number of observation/predictions </param>
        /// <returns></returns>
        public static double PSS(int[][] matrix, int count)
        {
            double NC = 0;
            double T = count;
            double E = 0;
            double Ep = 0;
            //
            double[] xip = new double[matrix.Length];
            double[] xpi = new double[matrix.Length];
            //
            for (int k = 0; k < matrix.Length; k++)
            {
                NC += matrix[k][k];

                for (int i = 0; i < matrix.Length; i++)
                {
                    xip[k] += matrix[k][i];
                    xpi[k] += matrix[i][k];
                }

                E += xip[k] * xpi[k];
                Ep += xip[k] * xip[k];
            }

            E = E / T;
            Ep = Ep / T;

            var hss = ((NC - E) / (T - Ep));
            return hss;
        }
        #endregion
    }
}
