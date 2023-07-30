using AnnDotNet.Core;
using AnnDotNet.Core.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnnDotNet.Core.Entities;
using Tensorflow.NumPy;
using Daany;

namespace AnnDotNet.test;

public class TestDataProvider
{
    public static (NDArray x, NDArray y) Prepare1DData()
    {
        var x = np.array(1.1f, 2.2f, 3.3f, 4.4f, 5.5f, 6.6f, 7.7f, 8.8f, 9.9f, 10.10f);
        var y = np.array(11.1f, 21.2f, 31.3f, 41.4f, 51.5f, 61.6f, 71.7f, 81.8f, 91.9f, 101.10f);

        return (x, y);
    }

    public static (NDArray, NDArray) PrepareIrisData()
    {

        //prepare the data
        var cols = new List<ColumnInfo>()
        {
            new ColumnInfo(){ Id=1, Name= "sepal_length", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
            new ColumnInfo(){ Id=2, Name= "sepal_width", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
            new ColumnInfo(){ Id=3, Name= "petal_length", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
            new ColumnInfo(){ Id=4, Name= "petal_width", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
            new ColumnInfo(){ Id=5, Name= "species", MLType=MLColumnType.Label, ValueColumnType= ColType.IN, Transformer= new DataTransformer(){ DataNormalization = ColumnTransformer.OneHot } },
        };

        var colTypes = cols.Select(x => x.ValueColumnType).ToList();
        colTypes.Insert(0, ColType.I32);
        colTypes.RemoveAt(colTypes.Count - 1);
        colTypes.Add(ColType.STR);

        //read the iris data and create DataFrame object
        var df = DataFrame.FromCsv("files/iris.txt", colTypes: colTypes.ToArray(), sep: '\t');

        //
        return df.TransformData(cols);

    }

    public static (NDArray, NDArray) PrepareIrisDataOrdinal()
    {

        //prepare the data
        var cols = new List<ColumnInfo>()
        {
            new ColumnInfo(){ Id=1, Name= "sepal_length", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
            new ColumnInfo(){ Id=2, Name= "sepal_width", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
            new ColumnInfo(){ Id=3, Name= "petal_length", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
            new ColumnInfo(){ Id=4, Name= "petal_width", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
            new ColumnInfo(){ Id=5, Name= "species", MLType=MLColumnType.Label, ValueColumnType= ColType.IN, Transformer= new DataTransformer(){ DataNormalization = ColumnTransformer.Ordinal } },
        };

        var colTypes = cols.Select(x => x.ValueColumnType).ToList();
        colTypes.Insert(0, ColType.I32);
        colTypes.RemoveAt(colTypes.Count - 1);
        colTypes.Add(ColType.STR);

        //read the iris data and create DataFrame object
        var df = DataFrame.FromCsv("files/iris.txt", colTypes: colTypes.ToArray(), sep: '\t');

        //
        return df.TransformData(cols);

    }

    public static (NDArray, NDArray) PrepareIrisDataDummy()
    {

        //prepare the data
        var cols = new List<ColumnInfo>()
        {
            new ColumnInfo(){ Id=1, Name= "sepal_length", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
            new ColumnInfo(){ Id=2, Name= "sepal_width", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
            new ColumnInfo(){ Id=3, Name= "petal_length", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
            new ColumnInfo(){ Id=4, Name= "petal_width", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
            new ColumnInfo(){ Id=5, Name= "species", MLType=MLColumnType.Label, ValueColumnType= ColType.IN, 
                Transformer= new DataTransformer(){ DataNormalization = ColumnTransformer.Dummy } },
        };

        var colTypes = cols.Select(x => x.ValueColumnType).ToList();
        colTypes.Insert(0, ColType.I32);
        colTypes.RemoveAt(colTypes.Count - 1);
        colTypes.Add(ColType.STR);

        //read the iris data and create DataFrame object
        var df = DataFrame.FromCsv("files/iris.txt", colTypes: colTypes.ToArray(), sep: '\t');

        //
        return df.TransformData(cols);

    }

    public static (NDArray, NDArray) Prepare2DData()
    {
        var array = new float[10, 2]
        { 
            { 1f, 11f }, 
            { 2f, 12f }, 
            { 3f, 13f }, 
            { 4f, 14f }, 
            { 5f, 15f }, 
            { 6f, 16f }, 
            { 7f, 17f }, 
            { 8f, 18f }, 
            { 9f, 19f }, 
            { 10f, 20f } 
        };

        //create 2D array (2x10)
        var X = new NDArray(array);

        var Y = new NDArray(new float[]
            {
                21f, 22f, 23f, 24f, 25f, 26f, 27f, 28f, 29f, 30f
            }
        );

        return (X, Y);
    }

    public static (NDArray, NDArray) Prepare2DData_binary_label()
    {
        var X = new float[10, 2]
        {
            { 1f, 11f },
            { 2f, 12f },
            { 3f, 13f },
            { 4f, 14f },
            { 5f, 15f },
            { 6f, 16f },
            { 7f, 17f },
            { 8f, 18f },
            { 9f, 19f },
            { 10f, 20f }
        };

        var Y = new NDArray(new bool[]
            {
                true, false, true, false, true, false, true, false, false, true
            }
        );

        return (X, Y);
    }

    public static (NDArray, NDArray) Prepare2DData_binary1_encoding()
    {
        //create 3D array (3x6x5)
        var dic = new Dictionary<string, List<object>>
        {
            {"col1", new List<object>{  1.0f,  2f,  3f,  4f,  5f,  6f,  7f,  8f,  9f, 10f} },
            {"col2", new List<object>{ 11f, 12f, 13f, 14f, 15f, 16f, 17f, 18f, 19f, 20f} },
            {"lab", new List<object> { true, false, true, false, true, false, true, false, false, true} }
        };

        //read the iris data and create DataFrame object
        var df = new DataFrame(dic);

        //prepare the data
        var cols = new List<ColumnInfo>()
        {
            new ColumnInfo(){ Id=1, Name= "col1", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
            new ColumnInfo(){ Id=2, Name= "col2", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
            new ColumnInfo(){ Id=3, Name= "lab", MLType=MLColumnType.Label, ValueColumnType= ColType.I2,
                Transformer= new DataTransformer(){ DataNormalization = ColumnTransformer.Binary1 } },
        };

        //
        return df.TransformData(cols);
            
    }

    public static (NDArray, NDArray) Prepare2DData_binary2_encoding()
    {
        //create 3D array (3x6x5)
        var dic = new Dictionary<string, List<object>>
        {
            {"col1", new List<object>{  1.0f,  2f,  3f,  4f,  5f,  6f,  7f,  8f,  9f, 10f} },
            {"col2", new List<object>{ 11f, 12f, 13f, 14f, 15f, 16f, 17f, 18f, 19f, 20f} },
            {"lab", new List<object> { true, false, true, false, true, false, true, false, false, true} }
        };

        //read the iris data and create DataFrame object
        var df = new DataFrame(dic);

        //prepare the data
        var cols = new List<ColumnInfo>()
        {
            new ColumnInfo(){ Id=1, Name= "col1", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
            new ColumnInfo(){ Id=2, Name= "col2", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
            new ColumnInfo(){ Id=3, Name= "lab", MLType=MLColumnType.Label, ValueColumnType= ColType.I2,
                Transformer= new DataTransformer(){ DataNormalization = ColumnTransformer.Binary2 } },
        };

        //
        return df.TransformData(cols);

    }

    public static (NDArray, NDArray) Prepare2DData_binary_label_one_hot()
    {
        var X = new float[10, 2]
        {
            { 1f, 11f },
            { 2f, 12f },
            { 3f, 13f },
            { 4f, 14f },
            { 5f, 15f },
            { 6f, 16f },
            { 7f, 17f },
            { 8f, 18f },
            { 9f, 19f },
            { 10f, 20f }
        };

        var Y = new NDArray(new float[10,2]
            {
                {0,1}, 
                {0,1}, 
                {0,1}, 
                {0,1}, 
                {0,1}, 
                {0,1}, 
                {0,1}, 
                {0,1}, 
                {0,1}, 
                {0,1}
            }
        );

        return (X, Y);
    }

    public static (NDArray, NDArray) Prepare2DData_multiclass_label()
    {
        var X = new float[10, 2]
        {
            { 1f, 11f },
            { 2f, 12f },
            { 3f, 13f },
            { 4f, 14f },
            { 5f, 15f },
            { 6f, 16f },
            { 7f, 17f },
            { 8f, 18f },
            { 9f, 19f },
            { 10f, 20f }
        };

        var Y = new NDArray(new float[,]
            {
                {0,1,0}, {0,0,1}, {0,0,1}, {1,0,0}, {0,0,1}, {1,0,0}, {0,1,1}, {0,1,0}, {0,0,1}, {1,0,0}
            }
        );

        return (X, Y);
    }

    public static (NDArray, NDArray) PrepareSimple3DData()
    {
        //create 3D array (3x6x5)
        var X = new NDArray(new float[,,]
        {
            {
                { 1, 2, 3, 4, 5 , 1},
                { 6, 7, 8, 9, 10 , 6},
                { 11, 12, 13, 14, 15, 11 },
                { 16, 17, 18, 19, 20, 16 },
                { 21, 22, 23, 24, 25, 21 }
            },
            {
                {26, 27, 28, 29, 30, 26},
                {31, 32, 33, 34, 35, 31 },
                {36, 37, 38, 39, 40, 36 },
                {41, 42, 43, 44, 45, 41 },
                {46, 47, 48, 49, 50, 46 }
            },
            {
                {51, 52, 53, 54, 55, 51 },
                {56, 57, 58, 59, 60, 56 },
                {61, 62, 63, 64, 65, 61 },
                {66, 67, 68, 69, 70, 66 },
                {71, 72, 73, 74, 75, 71 }
            }
        });


        var Y = new NDArray(new float[] { 1, 2, 3, 4, 5, 6 });

        return (X, Y);

    }
}