﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Anndotnet.Shared.Entities;

namespace Anndotnet.App.Model;

public class ProjectModel
{
    public Guid                            Id          { get; set; }
    public string                          Name        { get; set; } = "New_Project";
    public string?                         Description { get; set; }
    public DataParser?                     Parser      { get; set; }
    public List<HeaderInfo>?               Metadata    { get; set; }
    public ObservableCollection<MlConfig>? MlConfigs   { get; set; }

    [JsonIgnore]
    public string?                         Path        { get; set; }

}

public class MlConfig
{
    public string? Name { get; set; }
    public string? Path { get; set; }
}   
    
