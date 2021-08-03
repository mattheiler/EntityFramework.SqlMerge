﻿using System;

namespace Marvolo.EntityFrameworkCore.SqlServer.Merge.SqlDataAdapter.Extensions
{
    public static class SqlDataAdapterMergeBuilderExtensions
    {
        public static MergeBuilder<T> UsingSqlDataAdapter<T>(this MergeBuilder<T> @this, Action<SqlDataAdapterMergeSourceLoaderOptions> configure = default) where T : class
        {
            var options = new SqlDataAdapterMergeSourceLoaderOptions();
            configure?.Invoke(options);
            return @this.Using(new SqlDataAdapterMergeSourceLoader(options));
        }
    }
}