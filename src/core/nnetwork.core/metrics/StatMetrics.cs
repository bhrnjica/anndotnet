//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                      //
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
using CNTK;
using System.Linq;

namespace NNetwork.Core.Metrics
{
    /// <summary>
    /// Class implement statistic measures based on CNTK Functions. 
    /// The advantage of this implementation is ability to perform calculation on GPU by using CNTK library infrastructure.
    /// </summary>
    public static class StatMetrics
    {
        /// <summary>
        /// Weighted Squared error
        /// </summary>
        /// <param name="prediction"></param>
        /// <param name="actual"></param>
        /// <param name="weights"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Function WeightedSE(Variable prediction, Variable actual, Constant weights, string name = null)
        {

            var remainder = CNTKLib.Minus(actual, prediction);
            var squared = CNTKLib.Square(remainder);
            var ret = CNTKLib.ElementTimes(squared, weights);

            var sum = CNTKLib.ReduceSum(ret, Axis.AllAxes());


            return sum;
        }

        /// <summary>
        /// Calculate Correlation coefficient of two sets
        /// </summary>
        /// <param name="prediction"></param>
        /// <param name="target"></param>
        /// <returns>scalar of zero rank</returns>
        public static Function CorrelationC(Variable prediction, Variable target, string name = null)
        {
            var meana = CNTKLib.ReduceMean(target, Axis.AllAxes());
            var meanp = CNTKLib.ReduceMean(prediction, Axis.AllAxes());

            //
            var remaindera = CNTKLib.Minus(target, meana);
            var remainderp = CNTKLib.Minus(prediction, meanp);
            var eltime1 = CNTKLib.ElementTimes(remaindera, remainderp);
            var frac1 = CNTKLib.ReduceSum(eltime1, Axis.AllAxes());
            //
            var squarea = CNTKLib.Square(remaindera);
            var sum1 = CNTKLib.ReduceSum(squarea, Axis.AllAxes());
            var squarep = CNTKLib.Square(remainderp);
            var sum2 = CNTKLib.ReduceSum(squarep, Axis.AllAxes());
            var roota = CNTKLib.Sqrt(sum1);
            var rootp = CNTKLib.Sqrt(sum2);
            var frac2 = CNTKLib.ElementTimes(roota, rootp);
            var val = CNTKLib.ElementDivide(frac1, frac2);

            if (name != null)
                val.SetName(name);
            return val;
        }

        /// <summary>
        /// Calculates Mean Squared Error two sets
        /// </summary>
        /// <param name="prediction"></param>
        /// <param name="actual"></param>
        /// <returns>scalar of zero rank</returns>
        public static Function MSError(Variable prediction, Variable actual, string name = null)
        {
            var remainder = CNTKLib.Minus(actual, prediction);
            var squared = CNTKLib.Square(remainder);
            var mean = CNTKLib.ReduceMean(squared, Axis.AllAxes());

            if (name != null)
                mean.SetName(name);
            return mean;
        }

        /// <summary>
        /// Calculates Sum of Squared Error of two sets
        /// </summary>
        /// <param name="prediction"></param>
        /// <param name="actual"></param>
        /// <returns>scalar of zero rank</returns>
        public static Function SError(Variable prediction, Variable actual, string name = null)
        {
            var remainder = CNTKLib.Minus(actual, prediction);
            var squared = CNTKLib.Square(remainder);
            var sum = CNTKLib.ReduceSum(squared, Axis.AllAxes());

            return sum;
        }

        /// <summary>
        /// Calculate Root of Mean Squared Error of two sets
        /// </summary>
        /// <param name="prediction"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Function RMSError(Variable prediction, Variable target, string name=null)
        {
            var remainder = CNTKLib.Minus(target, prediction);
            var squared = CNTKLib.Square(remainder);
            var mean = CNTKLib.ReduceMean(squared, Axis.AllAxes());
            var squar = CNTKLib.Sqrt(mean);
            if (name != null)
                squar.SetName(name);
            return squar;
        }

        /// <summary>
        /// Calculate variance for data
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Function Variance(Variable input, string name = null)
        {
            var mean = CNTKLib.ReduceMean(input, new Axis(input.Shape.Dimensions.First() - 1));
            var remainder = CNTKLib.Minus(input, mean);
            var squared = CNTKLib.Square(remainder);
            //the last dimension indicate the number of samples
            var n = new Constant(new NDShape(0), DataType.Float, input.Shape.Dimensions.First() - 1);
            var elm = CNTKLib.ElementDivide(squared, n);
            var sum = CNTKLib.ReduceSum(elm, new Axis(2));

            if (name != null)
                sum.SetName(name);

            return sum;
        }

        /// <summary>
        /// Calculate standard deviation
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Function Std(Variable input, string name = null)
        {
            var mean = CNTKLib.ReduceMean(input, new Axis(input.Shape.Dimensions.First() - 1));
            var remainder = CNTKLib.Minus(input, mean);
            var squared = CNTKLib.Square(remainder);
            //the last dimension indicate the number of samples
            var n = new Constant(new NDShape(0), DataType.Float, input.Shape.Dimensions.First() - 1);
            var elm = CNTKLib.ElementDivide(squared, n);
            var sum = CNTKLib.ReduceSum(elm, new Axis(2));
            var stdVal = CNTKLib.Sqrt(sum);

            if (name != null)
                stdVal.SetName(name);
            return stdVal;
        }

        /// <summary>
        /// Covariance calculation between two datasets
        /// </summary>
        /// <param name="prediction"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Function Covariance(Variable prediction, Variable target, string name = null)
        {
            var meana = CNTKLib.ReduceMean(target, Axis.AllAxes());
            var meanp = CNTKLib.ReduceMean(prediction, Axis.AllAxes());

            //
            var remaindera = CNTKLib.Minus(target, meana);
            var remainderp = CNTKLib.Minus(prediction, meanp);
            var eltime1 = CNTKLib.ElementTimes(remaindera, remainderp);
            var sum = CNTKLib.ReduceSum(eltime1, Axis.AllAxes());

            //the last dimension indicate the number of samples
            var n = new Constant(new NDShape(0), DataType.Float, target.Shape.Dimensions.First() - 1.0);

            var fun =  CNTKLib.ElementDivide(sum, n);

            if (name != null)
                fun.SetName(name);
            return fun;
        }

        /// <summary>
        /// Calculate classification accuracy between prediction and target.
        /// </summary>
        /// <param name="prediction"></param>
        /// <param name="target"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Function ClassificationAccuracy(Variable prediction, Variable target, string name = null)
        {
            //
            var one = new Constant(new NDShape(0), DataType.Float, 1);
            var error = CNTKLib.ClassificationError(prediction, target);
            var acc = CNTKLib.Minus(one, error);

            if (name != null)
                acc.SetName(name);

            return acc;
        }

        /// <summary>
        /// Returns true if the evaluation function is based on minimization eg. Error, ClassificationError.
        /// In case of ClassificationAccuracy the evaluation is based on Maximum so the function should return false
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public static bool IsGoalToMinimize(Function function)
        {
            if (function.Name.Contains("ClassificationAccuracy") || function.Name.Contains("CorrelationC"))
                return false;
            else
                return true;
        }
    }
}
