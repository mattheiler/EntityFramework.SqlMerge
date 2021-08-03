﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Marvolo.EntityFrameworkCore.SqlServer.Merge
{
    public class MergeBuilder<T> : IMergeBuilder where T : class
    {
        private readonly DbContext _db;
        private readonly ICollection<IMergeBuilder> _dependents = new List<IMergeBuilder>();
        private readonly List<INavigation> _navigations = new List<INavigation>();
        private readonly ICollection<IMergeBuilder> _principals = new List<IMergeBuilder>();
        private MergeBehavior _behavior;
        private IEntityType _entityType;
        private MergeInsert _insert;
        private IMergeSourceLoader _loader;
        private MergeOn _on;
        private MergeUpdate _update;
        private IMergeSourceBuilder _builder;

        public MergeBuilder(DbContext db, MergeContext context)
        {
            _db = db;
            Context = context;
        }

        internal MergeContext Context { get; }

        internal IEntityType EntityType => _entityType ??= _db.Model.FindEntityType(typeof(T));

        public IMerge ToMerge()
        {
            var builder = _builder ?? _db.GetService<IMergeSourceBuilder>();
            var loader = _loader ?? _db.GetService<IMergeSourceLoader>();
            var keys = EntityType.FindPrimaryKey().Properties;
            var navigations =
                from navigation in EntityType.GetNavigations()
                where !navigation.IsDependentToPrincipal()
                where !navigation.IsCollection()
                where navigation.GetTargetType().IsOwned()
                select navigation;
            var properties = EntityType.GetProperties().Concat<IPropertyBase>(navigations).ToList();

            var target = new MergeTarget(EntityType);
            var source = new MergeSource(_db, properties, builder, loader);
            var on = _on ?? new MergeOn(keys);
            var insert = _behavior.HasFlag(MergeBehavior.WhenNotMatchedByTargetThenInsert) ? _insert ?? new MergeInsert(properties) : null;
            var update = _behavior.HasFlag(MergeBehavior.WhenMatchedThenUpdate) ? _update ?? new MergeUpdate(properties) : null;
            var output = new MergeOutput(_db, on.Properties.Union(EntityType.GetKeys().SelectMany(key => key.Properties).Distinct()));

            foreach (var entity in Context.Get(typeof(T)))
            foreach (var navigation in _navigations)
            {
                var value = navigation.GetGetter().GetClrValue(entity);
                if (value == null)
                    continue;

                var type = navigation.GetTargetType().ClrType;

                if (navigation.IsCollection())
                    Context.AddRange(type, (IEnumerable) value);
                else
                    Context.Add(type, value);
            }

            var principals = _principals.Select(principal => principal.ToMerge());
            var dependents = _dependents.Select(dependent => dependent.ToMerge());
            var merge = new Merge(_db, target, source, on, _behavior, insert, update, output);

            return new MergeComposite(principals.Append(merge).Concat(dependents));
        }

        private MergeBuilder<T> Merge<TProperty>(LambdaExpression property, Action<MergeBuilder<TProperty>> build) where TProperty : class
        {
            // ReSharper disable once UseNegatedPatternMatching
            var body = property.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("Expression body must describe a navigation property.");

            var navigation = EntityType.FindNavigation(body.Member);
            if (navigation == null)
                throw new ArgumentException("Expression body must describe a navigation property.");

            var builder = new MergeBuilder<TProperty>(_db, Context);

            build(builder);

            if (navigation.IsDependentToPrincipal())
                _principals.Add(builder);
            else
                _dependents.Add(builder);

            _navigations.Add(navigation);

            return this;
        }

        public MergeBuilder<T> Merge<TProperty>(Expression<Func<T, TProperty>> property, Action<MergeBuilder<TProperty>> build) where TProperty : class
        {
            return Merge((LambdaExpression) property, build);
        }

        public MergeBuilder<T> MergeMany<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> property, Action<MergeBuilder<TProperty>> build) where TProperty : class
        {
            return Merge(property, build);
        }

        public MergeBuilder<T> Using(IMergeSourceBuilder builder)
        {
            _builder = builder;
            return this;
        }

        public MergeBuilder<T> Using(IMergeSourceLoader loader)
        {
            _loader = loader;
            return this;
        }

        public MergeBuilder<T> On(MergeOn on)
        {
            _on = on;
            return this;
        }

        public MergeBuilder<T> Behavior(MergeBehavior behavior, bool enable = true)
        {
            _behavior = enable ? _behavior | behavior : _behavior & ~behavior;
            return this;
        }

        public MergeBuilder<T> Insert(MergeInsert insert)
        {
            _insert = insert;
            return this;
        }

        public MergeBuilder<T> Update(MergeUpdate update)
        {
            _update = update;
            return this;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return ToMerge().ExecuteAsync(Context, cancellationToken);
        }
    }
}