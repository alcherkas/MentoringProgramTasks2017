using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;

namespace ClassMapper
{
    public class MappingGenerator
    {
        public Mapper<TSource, TDestination> Generate<TSource, TDestination>()
        {
            var sourceParam = Expression.Parameter(typeof(TSource));
            var propertyBindings = new List<MemberBinding>();
            foreach(var property in typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!property.CanRead) continue;

                var targetProperty = typeof(TDestination).GetProperty(property.Name);

                if(!targetProperty.CanWrite || property.PropertyType != targetProperty.PropertyType) continue;

                var propertiesBinding = Expression.Bind(targetProperty, Expression.Property(sourceParam, property));
                propertyBindings.Add(propertiesBinding);
            }

            var targetMemberInitializer = Expression.MemberInit(Expression.New(typeof(TDestination)), propertyBindings);
            var mapFunction = Expression.Lambda<Func<TSource, TDestination>>(targetMemberInitializer, sourceParam);
            return new Mapper<TSource, TDestination>(mapFunction.Compile());
        }
    }
}
