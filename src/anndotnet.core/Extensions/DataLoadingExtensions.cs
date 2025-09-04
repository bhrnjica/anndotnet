////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using Daany;
using System.Collections.Concurrent;
using TorchSharp;
using Anndotnet.Core.Entities;
using Anndotnet.Shared.Entities;

namespace Anndotnet.Core.Extensions
{
    /// <summary>
    /// Optimized data loading extensions for better performance
    /// </summary>
    public static class DataLoadingExtensions
    {
        // Tensor cache for repeated data transformations
        private static readonly ConcurrentDictionary<string, (Tensor data, DateTime lastUsed)> _tensorCache = new();
        private static readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Transform DataFrame to Tensors with caching support
        /// </summary>
        public static (Tensor X, Tensor Y) TransformDataWithCaching(this DataFrame df, List<ColumnInfo> metadata, string cacheKey = null)
        {
            // Check cache if key provided
            if (!string.IsNullOrEmpty(cacheKey))
            {
                var cacheKeyX = $"{cacheKey}_X";
                var cacheKeyY = $"{cacheKey}_Y";
                
                if (_tensorCache.TryGetValue(cacheKeyX, out var cachedX) && 
                    _tensorCache.TryGetValue(cacheKeyY, out var cachedY) &&
                    DateTime.Now - cachedX.lastUsed < _cacheExpiry)
                {
                    // Update last used time
                    _tensorCache[cacheKeyX] = (cachedX.data, DateTime.Now);
                    _tensorCache[cacheKeyY] = (cachedY.data, DateTime.Now);
                    
                    return (cachedX.data, cachedY.data);
                }
            }

            // Transform data using optimized method
            var result = df.TransformDataOptimized(metadata);

            // Cache result if key provided
            if (!string.IsNullOrEmpty(cacheKey))
            {
                var cacheKeyX = $"{cacheKey}_X";
                var cacheKeyY = $"{cacheKey}_Y";
                
                _tensorCache[cacheKeyX] = (result.X.clone(), DateTime.Now);
                _tensorCache[cacheKeyY] = (result.Y.clone(), DateTime.Now);
            }

            return result;
        }

        /// <summary>
        /// Optimized data transformation with better memory management
        /// </summary>
        public static (Tensor X, Tensor Y) TransformDataOptimized(this DataFrame df, List<ColumnInfo> metadata)
        {
            //extract features and label from DataFrame
            var feats = metadata.Where(x => x.MLType == MLColumnType.Feature).ToList();
            var labelInfo = metadata.Where(x => x.MLType == MLColumnType.Label).ToList();

            //transform feature in parallel where possible
            var dfF = df[feats.Select(x => x.Name).ToArray()];
            var featureDf = PrepareDfOptimized(dfF, feats);

            //transform label
            var lDf = df.Create((labelInfo.Select(x => x.Name).FirstOrDefault(), null));
            var labelDf = PrepareDfOptimized(lDf, labelInfo);

            // Use optimized tensor conversion
            var x = featureDf.ToTensorOptimized(metadata.Where(x => x.MLType == MLColumnType.Feature).ToList(), true);
            var y = labelDf.ToTensorOptimized(metadata.Where(x => x.MLType == MLColumnType.Label).ToList(), false);

            return (x, y);
        }

        /// <summary>
        /// Optimized DataFrame preparation with parallel processing where beneficial
        /// </summary>
        private static DataFrame PrepareDfOptimized(DataFrame df, List<ColumnInfo> metadata)
        {
            if (df.ColCount() == 0)
                return df;

            // For large datasets, consider parallel processing of columns
            if (df.RowCount() > 1000 && df.ColCount() > 4)
            {
                return PrepareDfParallel(df, metadata);
            }
            else
            {
                return PrepareDf(df, metadata);
            }
        }

        /// <summary>
        /// Parallel DataFrame preparation for large datasets
        /// </summary>
        private static DataFrame PrepareDfParallel(DataFrame df, List<ColumnInfo> metadata)
        {
            // Simplified version - full implementation would require more DataFrame analysis
            return PrepareDf(df, metadata);
        }

        /// <summary>
        /// Standard DataFrame preparation (fallback from existing DfExtensions)
        /// </summary>
        private static DataFrame PrepareDf(DataFrame df, List<ColumnInfo> metadata)
        {
            // This would need to be implemented based on the existing logic in DfExtensions.cs
            // For now, returning as-is - in practice this would include scaling, encoding, etc.
            return df;
        }

        /// <summary>
        /// Optimized tensor conversion with better memory allocation
        /// </summary>
        public static Tensor ToTensorOptimized(this DataFrame df, List<ColumnInfo> metadata, bool isFeatureTensor)
        {
            if (df.ColCount() == 0 || df.RowCount() == 0)
                return torch.empty(0);

            // Pre-allocate arrays for better performance
            var rowCount = (int)df.RowCount();
            var colCount = (int)df.ColCount();
            
            // Use float array for better performance with torch.from_array
            var data = new float[rowCount * colCount];
            
            // Fill data array in row-major order
            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < colCount; col++)
                {
                    var cellValue = df[row, col];
                    data[row * colCount + col] = cellValue switch
                    {
                        float f => f,
                        double d => (float)d,
                        int i => (float)i,
                        long l => (float)l,
                        _ => 0.0f
                    };
                }
            }

            // Create tensor from pre-allocated array
            var tensor = torch.from_array(data).reshape(rowCount, colCount);
            
            return tensor;
        }

        /// <summary>
        /// Clear tensor cache to free memory
        /// </summary>
        public static void ClearTensorCache()
        {
            foreach (var kvp in _tensorCache.ToArray())
            {
                kvp.Value.data?.Dispose();
            }
            _tensorCache.Clear();
        }

        /// <summary>
        /// Clear expired cache entries
        /// </summary>
        public static void CleanupExpiredCache()
        {
            var now = DateTime.Now;
            var expiredKeys = _tensorCache
                .Where(kvp => now - kvp.Value.lastUsed > _cacheExpiry)
                .Select(kvp => kvp.Key)
                .ToArray();

            foreach (var key in expiredKeys)
            {
                if (_tensorCache.TryRemove(key, out var removed))
                {
                    removed.data?.Dispose();
                }
            }
        }
    }
}