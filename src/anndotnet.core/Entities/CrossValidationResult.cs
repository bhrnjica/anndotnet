using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anndotnet.Core.Entities
{
    public sealed class CrossValidationResult<T> where T : class
    {
        /// <summary>Metrics for this cross-validation fold.</summary>
        public readonly T Metrics;
        
        /// <summary>Model trained during cross-validation fold.</summary>
        public readonly object Model;

        /// <summary>The scored hold-out set for this fold.</summary>
        public readonly object ScoredHoldOutSet;
        
        /// <summary>Fold number.</summary>
        public readonly int Fold;
    }
}
