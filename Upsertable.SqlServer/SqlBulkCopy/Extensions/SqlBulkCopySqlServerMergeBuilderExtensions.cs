﻿using System;

namespace Upsertable.SqlServer.SqlBulkCopy.Extensions;

public static class SqlBulkCopySqlServerMergeBuilderExtensions
{
    public static SqlServerMergeBuilder<T> UsingSqlBulkCopy<T>(this SqlServerMergeBuilder<T> @this, Action<SqlBulkCopyDataLoaderOptions> configure = default) where T : class
    {
        var options = new SqlBulkCopyDataLoaderOptions();
        configure?.Invoke(options);
        return @this.WithSourceLoader(new SqlBulkCopyDataLoader(options));
    }
}