//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                       //
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
using System.Collections.Generic;

namespace ANNdotNET.Core
{

    public class ModelPerformance
    {
        public string DatSetName { get; set; }
        public Dictionary<string, List<object>> PerformanceData { get; set; }

        //regression
        public float SE { set; get; }
        public float RMSE { set; get; }
        public float NSE { set; get; }
        public float PB { set; get; }
        public float CORR { set; get; }
        public float DETC { set; get; }

        //binary
        //True Positive
        public float TP { set; get; }
        //True Negative
        public float FN { set; get; }
        //False Positive
        public float FP { set; get; }
        //True Negative
        public float TN { set; get; }
        //Area under Curve
        public float AUC { set; get; }

        //Classification error
        public float ER { set; get; }

        //classification class values
        public string[] Classes { set; get; }

        //Classification accuracy
        public float Acc { set; get; }
        //Precision
        public float Precision { set; get; }
        //Recall 
        public float Recall { set; get; }
        //F1 Score
        public float F1Score { set; get; }

        //multiclass
        //confusion matrix
        //Overall accuracy
        public float OAcc { set; get; }
        //Average accuracy
        public float AAcc { set; get; }
        //Micro-average Precision
        public float MicPrec{ set; get; }
        //Macro-average Precision
        public float MacPrec { set; get; }
        //Micro-average Recall
        public float MicRcall { set; get; }
        //Macro-average Recall
        public float MacRcall { set; get; }
        //Heideke Skill Score
        public float HSS { set; get; }
        //Peirce Skill Score
        public float PSS { set; get; }
    }
}