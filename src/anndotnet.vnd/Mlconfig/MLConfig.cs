using Anndotnet.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anndotnet.Vnd
{
    public class MLConfig
    {
        public Guid Id { get; set; }
        public DataParser Parser { get; set; }
        public List<ColumnInfo> Metadata { get; set; }
        public List<LayerBase> Network { get; set; }
        public LearningParameters LParameters { get; set; }
        public TrainingParameters TParameters { get; set; }
        public Dictionary<string, string> Paths { get; set; }
    }
}
