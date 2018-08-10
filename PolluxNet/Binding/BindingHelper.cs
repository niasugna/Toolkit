using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace Pollux
{
    //http://stackoverflow.com/questions/6291201/dependencyproperty-from-string
    public static class BindingHelper
    {
        public static void Bind<TC, TD, TP>(this TC control, Expression<Func<TC, TP>> controlProperty, TD dataSource, Expression<Func<TD, TP>> dataMember, System.Windows.Data.UpdateSourceTrigger trigger = System.Windows.Data.UpdateSourceTrigger.Default) where TC : Control
        {
            var binding = new System.Windows.Data.Binding();
            binding.Source = dataSource;
            binding.Path = new System.Windows.PropertyPath(Name(dataMember));
            binding.UpdateSourceTrigger = trigger;

            var descriptor = DependencyPropertyDescriptor.FromName(Name(controlProperty),control.GetType(),control.GetType());

            control.SetBinding(descriptor.DependencyProperty, binding);//.DataBindings.Add(Name(controlProperty), dataSource, Name(dataMember));
        }
        public static void BindAction<TC, TD, TP>(this TC control, Expression<Func<TC, TP>> controlProperty, TD dataSource, Expression<Func<TD, TP>> dataMember,System.Windows.Data.UpdateSourceTrigger trigger= System.Windows.Data.UpdateSourceTrigger.Default) where TC : Control
        {
            var binding = new System.Windows.Data.Binding();
            binding.Source = dataSource;
            binding.Path = new System.Windows.PropertyPath(Name(dataMember));

            binding.UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.Default;

            var descriptor = DependencyPropertyDescriptor.FromName(Name(controlProperty), control.GetType(), control.GetType());

            control.SetBinding(descriptor.DependencyProperty, binding);//.DataBindings.Add(Name(controlProperty), dataSource, Name(dataMember));
        }
        public static string Name<T1,T2>(Expression<Func<T1, T2>> expression)
        {
            return GetMemberName(expression.Body);
        }
        public static string Name<T>(Expression<Func<T>> expression)
        {
            return GetMemberName(expression.Body);
        }
        private static string GetMemberName(Expression expression)
        {
            // The nameof operator was implemented in C# 6.0 with .NET 4.6
            // and VS2015 in July 2015. 
            // The following is still valid for C# < 6.0

            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression;
                    var supername = GetMemberName(memberExpression.Expression);
                    if (String.IsNullOrEmpty(supername)) return memberExpression.Member.Name;
                    return String.Concat(supername, '.', memberExpression.Member.Name);

                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)expression;
                    return callExpression.Method.Name;

                case ExpressionType.Convert:
                    var unaryExpression = (UnaryExpression)expression;
                    return GetMemberName(unaryExpression.Operand);

                case ExpressionType.Parameter:
                case ExpressionType.Constant: //Change
                    return String.Empty;

                default:
                    throw new ArgumentException("The expression is not a member access or method call expression");
            }
        }
    }
}
