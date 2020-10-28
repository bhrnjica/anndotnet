using System;
using System.Collections.Generic;
using System.Text;

namespace Anndotnet.Core.Entities
{
    public class TrainingHistory
    {
        public List<TrainingEvent> History { get; set; }
    }

    public class TrainingEvent
    {
        public int Id { get; set; }
        public string ModelPath { get; set; }
        public float Loss { get; set; }
        public Dictionary<string, float> Evals { get; set; }
    }
}
