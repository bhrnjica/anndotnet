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
    [Description("Average")]
    Average,
    [Description("Random")]
    Random
}

public enum ColMlDataType
{
    [Description("Ignore")]
    Ignore,
    [Description("Feature")]
    Feature,
    [Description("Label")]
    Label,
}   
