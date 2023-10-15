using System.ComponentModel;

namespace Anndotnet.App.Models;

public enum Examples
{
    [Description("Slump")]
    Slump,
    [Description("Iris")]
    Iris,
    [Description("Titanic")]
    Titanic,
    [Description("Mnist")]
    Mnist,
    [Description("New")]
    New,
    [Description("Open")]
    Open,
    Close
}

public enum ColValueType
{
    [Description("Numeric")]
    Numeric,
    [Description("Categoric")]
    Categoric,
    [Description("None")]
    None
}

public enum ColMissingValue
{
    [Description("None")]
    None,
    [Description("Random")]
    Random,
    [Description("Average")]
    Average
}

public enum ColMlDataType
{
    [Description("Feature")]
    Feature,
    [Description("Label")]
    Label,
    [Description("Ignore")]
    Ignore
}   
