using System;
using System.Collections.Generic;
using System.Text;

namespace ANNdotNET.Core
{
    public enum EvaluationType
    {
        FeaturesOnly,//evaluation function will return only dataset with features
        Results,    //evaluation function will return Actual and Predicted final values
        ResultExtended, //evaluation function will return Actual and Predicted values in One-Hot -Vector if available
    }
}
