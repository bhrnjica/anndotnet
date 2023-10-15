using Anndotnet.App.Models;
using Anndotnet.App.ViewModels;
using Anndotnet.Core.Entities;
using Daany;

namespace Anndotnet.App.Extensions;

public static class ModelExtension
{
    public static ColMlDataType Map(this MLColumnType mlType)
    {
        switch (mlType)
        {
            case MLColumnType.Feature:
                return ColMlDataType.Feature;
            case MLColumnType.Label:
                return ColMlDataType.Label;
            case MLColumnType.None:
                return ColMlDataType.Ignore;
            default:
                return ColMlDataType.Ignore;

        }
    }
    public static ColValueType Map(this ColType colType)
    {
        switch (colType)
        {
            case ColType.I2:
            case ColType.I32:
            case ColType.I64:
            case ColType.F32:
            case ColType.DD:
                return ColValueType.Numeric;
            case ColType.IN:
                return ColValueType.Categoric;
            case ColType.STR:
                return ColValueType.None;
            default:
                return ColValueType.None; 

        }
    }

    //public static ColType Map(this ColValueType colVType)
    //{
    //    switch (colVType)
    //    {
    //        case ColValueType.Categoric:
    //            return ColType.IN;
    //        default:
    //            return ColType.STR;
    //    }
    //}

    public static ColMissingValue Map(this Daany.Aggregation missValue)
    {
        switch (missValue)
        {
            case Daany.Aggregation.Avg:                
               return ColMissingValue.Average;
            case Daany.Aggregation.Random:
                return ColMissingValue.Random;
            default:
                return ColMissingValue.None;    

        }
    }

}
