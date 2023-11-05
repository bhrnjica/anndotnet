using System.ComponentModel;

namespace Anndotnet.Shared.Entities;

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
    [Description("Integer")]
    Integer,
    [Description("Float")]
    Float,
    [Description("Categorical")]
    Categorical,
    [Description("String")]
    String
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
