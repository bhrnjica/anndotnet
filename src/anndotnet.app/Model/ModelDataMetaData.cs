
using Anndotnet.App.Model;
using Anndotnet.Shared.Entities;

namespace Anndotnet.App.Model;

public class ModelDataMetaData
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public ColValueType ValueType { get; set; }

    public ColMlDataType MlDataType { get; set; }

    public ColMissingValue MissingValue { get; set; }

}
