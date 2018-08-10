using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
//using System.Data.Objects;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using System.Threading;

namespace Pollux.EntityFramework
{
    public static class EntityFrameworkQueryableExtensions
    {
        public static MethodInfo IncludeMethodInfo
        {
            get
            {
                var res = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods("Include")
                .Single(mi =>
                    mi.GetGenericArguments().Count() == 2
                    && mi.GetParameters().Any(
                        pi => pi.Name == "navigationPropertyPath" && pi.ParameterType != typeof(string)));

                return res;
            }
        }
        public static MethodInfo ThenIncludeAfterReferenceMethodInfo
        {
            get
            {
                var res = typeof(EntityFrameworkQueryableExtensions).GetTypeInfo().GetDeclaredMethods("ThenInclude").Single(mi => mi.GetParameters()[0].ParameterType.GenericTypeArguments[1].IsGenericParameter);
                return res;
            }
        }

        public static MethodInfo ThenIncludeAfterCollectionMethodInfo
        {
            get
            {
                var res = 
                    typeof(EntityFrameworkQueryableExtensions).GetTypeInfo()
                    .GetDeclaredMethods("ThenInclude")
                    .Single(mi => !mi.GetParameters()[0].ParameterType.GenericTypeArguments[1].IsGenericParameter);
                return res;
            }
        }

        public static IIncludableQueryable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
            this IIncludableQueryable<TEntity, TPreviousProperty> source,
            Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            where TEntity : class
        {
            return new IncludableQueryable<TEntity, TProperty>(
                (source.Provider.GetType().Name == "DbQueryProvider")
                    ? source.Provider.CreateQuery<TEntity>(
                        Expression.Call(
                            null,
                            ThenIncludeAfterReferenceMethodInfo.MakeGenericMethod(typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty)),
                            new[] { source.Expression, Expression.Quote(navigationPropertyPath) }))
                    : source);
        }

        public static IIncludableQueryable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
            this IIncludableQueryable<TEntity, IEnumerable<TPreviousProperty>> source,
            Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            where TEntity : class
        {
            //IQueryable<TEntity> src = null;

            //if(source.Provider.GetType().Name == "DbQueryProvider")
            //{
            //    Type elementType = TypeSystem.GetElementType(expression.Type);
            //    try
            //    {
            //        return (IQueryable)Activator.CreateInstance(typeof(QueryableTerraServerData<>).MakeGenericType(elementType), new object[] { this, expression });
            //    }
            //    catch (System.Reflection.TargetInvocationException tie)
            //    {
            //        throw tie.InnerException;
            //    }

            //    //src = source.Provider.CreateQuery<TEntity>(
            //    //    Expression.Call(
            //    //        null,
            //    //        ThenIncludeAfterCollectionMethodInfo.MakeGenericMethod(typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty)),
            //    //        new[] 
            //    //        {
            //    //            source.Expression, Expression.Quote(navigationPropertyPath) 
            //    //        }
            //    //        ));
            //}
            //else
            //{
            //    src = source;
            //}

            return new IncludableQueryable<TEntity, TProperty>(source.ThenInclude(navigationPropertyPath));
        }

        public static IIncludableQueryable<TEntity, TProperty> IncludeEx<TEntity, TProperty>(this IQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> navigationPropertyPath) where TEntity : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            return new IncludableQueryable<TEntity, TProperty>(
              QueryableExtensions.Include(source, navigationPropertyPath));
            //return new IncludableQueryable<TEntity, TProperty>(
            //    (source.Provider.GetType().Name == "DbQueryProvider")
            //        ? source.Provider.CreateQuery<TEntity>(
            //            Expression.Call(
            //                null,
            //                IncludeMethodInfo.MakeGenericMethod(typeof(TEntity), typeof(TProperty)),
            //                new[] { source.Expression, Expression.Quote(navigationPropertyPath) }))
            //        : source);
        }
    }

    public interface IIncludableQueryable<out TEntity, out TProperty> : IQueryable<TEntity>
    {
    }
    internal class IncludableQueryable<TEntity, TProperty> : IIncludableQueryable<TEntity, TProperty>
    {
        private readonly IQueryable<TEntity> _queryable;

        public IncludableQueryable(IQueryable<TEntity> queryable)
        {
            _queryable = queryable;
        }

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }

        Type IQueryable.ElementType
        {
            get
            {
                return _queryable.ElementType;
            }
        }

        Expression IQueryable.Expression
        {
            get
            {
                return _queryable.Expression;
            }
        }

        IQueryProvider IQueryable.Provider
        {
            get
            {
                return _queryable.Provider;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }
    }

    public static class ObjectQueryExtensions
    {
        //public static IQueryable<TEntity> IncludeEx<TEntity, TProperty>(
        //    this IQueryable<TEntity> source, 
        //    Expression<Func<TEntity, TProperty>> path) 
        //    where TProperty : class
        //{
        //        //return QueryableExtensions.Include(source, path);
        //    var includedSource = QueryableExtensions.Include(source, path);
        //    var propertyName = GetPropertyName(path);
        //    var s = new IncludableQueryable<TEntity, TProperty>(includedSource, propertyName);
        //    return s ;
        //}
   



        //public static IIncludableQueryable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IQueryable<TEntity> source,
        //   Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
        //   where TEntity : class
        //{
        //    var propertyNameChildLambda = GetPropertyName(navigationPropertyPath);
        //    if (source.Provider.GetType().Name == "DbQueryProvider")
        //    {
        //        var sourceChild = QueryableExtensions.Include(source, s.PropertyName + "." + propertyNameChildLambda);
        //        return new IncludableQueryable<TEntity, TProperty>(sourceChild, s.PropertyName + "." + propertyNameChildLambda);
        //    }
        //    return new IncludableQueryable<TEntity, TProperty>(source, propertyNameChildLambda);
        //}









      

        private static string GetPropertyName<TModel, TValue>(this Expression<Func<TModel, TValue>> propertySelector, char delimiter = '.', char endTrim = ')')
        {

            var asString = propertySelector.ToString(); // gives you: "o => o.Whatever"
            var firstDelim = asString.IndexOf(delimiter); // make sure there is a beginning property indicator; the "." in "o.Whatever" -- this may not be necessary?

            return firstDelim < 0
                ? asString
                : asString.Substring(firstDelim + 1).TrimEnd(endTrim);
        }

    }

    //public static class ObjectQueryExtensions
    //{
    //    public static ObjectQuery<T> Includes<T>(this ObjectQuery<T> query, Action<IncludeObjectQuery<T, T>> action)
    //    {
    //        var sb = new StringBuilder();
    //        var queryBuilder = new IncludeObjectQuery<T, T>(query, sb);
    //        action(queryBuilder);
    //        return queryBuilder.Query;
    //    }

    //    public static ObjectQuery<TEntity> Include<TEntity, TProperty>(this ObjectQuery<TEntity> query, Expression<Func<TEntity, TProperty>> expression)
    //    {
    //        var sb = new StringBuilder();
    //        return IncludeAllLevels(expression, sb, query);
    //    }

    //    static ObjectQuery<TQuery> IncludeAllLevels<TEntity, TProperty, TQuery>(Expression<Func<TEntity, TProperty>> expression, StringBuilder sb, ObjectQuery<TQuery> query)
    //    {
    //        foreach (var name in expression.GetPropertyLevels())
    //        {
    //            sb.Append(name);
    //            query = query.Include(sb.ToString());
    //            Debug.WriteLine(string.Format("Include(\"{0}\")", sb));
    //            sb.Append('.');
    //        }
    //        return query;
    //    }

    //    static IEnumerable<string> GetPropertyLevels<TClass, TProperty>(this Expression<Func<TClass, TProperty>> expression)
    //    {
    //        var namesInReverse = new List<string>();

    //        var unaryExpression = expression as UnaryExpression;
    //        var body = unaryExpression != null ? unaryExpression.Operand : expression.Body;

    //        while (body != null)
    //        {
    //            var memberExpression = body as MemberExpression;
    //            if (memberExpression == null)
    //                break;

    //            namesInReverse.Add(memberExpression.Member.Name);
    //            body = memberExpression.Expression;
    //        }

    //        namesInReverse.Reverse();
    //        return namesInReverse;
    //    }

    //    public class IncludeObjectQuery<TQuery, T>
    //    {
    //        readonly StringBuilder _pathBuilder;
    //        public ObjectQuery<TQuery> Query { get; private set; }

    //        public IncludeObjectQuery(ObjectQuery<TQuery> query, StringBuilder builder)
    //        {
    //            _pathBuilder = builder;
    //            Query = query;
    //        }

    //        public IncludeObjectQuery<TQuery, U> Include<U>(Expression<Func<T, U>> expression)
    //        {
    //            Query = ObjectQueryExtensions.IncludeAllLevels(expression, _pathBuilder, Query);
    //            return new IncludeObjectQuery<TQuery, U>(Query, _pathBuilder);
    //        }

    //        public IncludeObjectQuery<TQuery, U> Include<U>(Expression<Func<T, EntityCollection<U>>> expression) where U : class
    //        {
    //            Query = ObjectQueryExtensions.IncludeAllLevels(expression, _pathBuilder, Query);
    //            return new IncludeObjectQuery<TQuery, U>(Query, _pathBuilder);
    //        }
    //    }
    //}
}
