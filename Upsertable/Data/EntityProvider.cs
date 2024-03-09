﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Upsertable.Extensions;

namespace Upsertable.Data;

public static class EntityProvider
{
    public static EntityProviderFunc Join(DbContext db, ISkipNavigation navigation, EntityProviderFunc provider)
    {
        return () =>
        {
            var entities = new List<object>();

#pragma warning disable EF1001 // Internal EF Core API usage.
            var context = new MaterializationContext(new ValueBuffer(), db);
            var materializer = db.GetDependencies().StateManager.EntityMaterializerSource.GetMaterializer(navigation.JoinEntityType);
#pragma warning restore EF1001 // Internal EF Core API usage.

            foreach (var source in provider())
            foreach (var target in (IEnumerable)navigation.GetCollectionAccessor()?.GetOrCreate(source, false) ?? Enumerable.Empty<object>())
            {
                var instance = materializer(context);

                navigation.ForeignKey.Properties.SetValues(instance, navigation.ForeignKey.PrincipalKey.Properties.GetValues(source));
                navigation.Inverse.ForeignKey.Properties.SetValues(instance, navigation.Inverse.ForeignKey.PrincipalKey.Properties.GetValues(target));

                entities.Add(instance);
            }

            return entities.Distinct();
        };
    }

    public static EntityProviderFunc Lazy(INavigationBase navigation, EntityProviderFunc provider)
    {
        return () =>
        {
            var entities = new List<object>();

            foreach (var source in provider())
            {
                var value = navigation.GetValue(source);
                if (value == null)
                    continue;

                if (navigation.IsCollection)
                    entities.AddRange(((ICollection)value).Cast<object>());
                else
                    entities.Add(value);
            }

            return entities.Distinct();
        };
    }

    public static EntityProviderFunc List<T>(IEnumerable<T> entities)
    {
        return entities.Distinct;
    }
}