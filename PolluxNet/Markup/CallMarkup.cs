using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using Pollux.Helper;
using System.Windows.Controls;


namespace Pollux.Mvvm
{
    //https://www.thomaslevesque.com/tag/markup-extension/
    //<Button Content="Click me" Click="{my:EventBinding OnClick}" />

    //http://www.jonathanantoine.com/2011/09/23/wpf-4-5s-markupextension-invoke-a-method-on-the-viewmodel-datacontext-when-an-event-is-raised/
    //<Grid PreviewMouseDown="{custMarkup:Call MyMethodToCallOnTheViewModel}" />
    public class EventHandlerMarkup : MarkupExtension
    {
        public string ActionName { get; set; }
        public Type ViewType { get; set; }

        public EventHandlerMarkup(string actionName) { ActionName = actionName;}
        public EventHandlerMarkup(string actionName, Type viewType) { ActionName = actionName; ViewType = viewType; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget targetProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (targetProvider == null)
                throw new InvalidOperationException(@"The CallAction extension can't retrieved the IProvideValueTarget service.");

            var target = targetProvider.TargetObject as DependencyObject;
            if (target == null)
                //throw new InvalidOperationException(@"The CallAction extension can only be used on a FrameworkElement.");
                throw new InvalidOperationException(@"This CallAction extension can only be used on a DependencyObject.");

            var targetEventAddMethod = targetProvider.TargetProperty as MethodInfo;
            Delegate returnedDelegate = null;
            if (targetEventAddMethod != null)
                returnedDelegate = CreateDelegateForMethodInfo(targetEventAddMethod);

            var targetEventInfo = targetProvider.TargetProperty as EventInfo;
            if (targetEventInfo != null)
                returnedDelegate = CreateDelegateForEventInfo(targetEventInfo);

            return returnedDelegate;
            //throw new InvalidOperationException(@"The CallAction extension can only be used on a event/method.");
        }

        private Delegate CreateDelegateForMethodInfo(MethodInfo targetEventAddMethod)
        {
            //Retrieve the handler of the event
            ParameterInfo[] pars = targetEventAddMethod.GetParameters();
            Type delegateType = pars[1].ParameterType;

            //Retrieves the method info of the proxy handler
            MethodInfo methodInfo = this.GetType().GetMethod("MyProxyHandler", BindingFlags.NonPublic | BindingFlags.Instance);

            //Create a delegate to the proxy handler on the markupExtension
            Delegate returnedDelegate = Delegate.CreateDelegate(delegateType, this, methodInfo);
            return returnedDelegate;
        }
        private Delegate CreateDelegateForEventInfo(EventInfo targetEventInfo)
        {
            if (targetEventInfo == null)
                throw new InvalidOperationException(@"The CallAction extension can only be used on a event.");

            Type delegateType = targetEventInfo.EventHandlerType;

            //Retrieves the method info of the proxy handler
            MethodInfo methodInfo = this.GetType().GetMethod("MyProxyHandler",BindingFlags.NonPublic | BindingFlags.Instance);

            //Create a delegate to the proxy handler on the markupExtension
            Delegate returnedDelegate = Delegate.CreateDelegate(delegateType, this, methodInfo);
            return returnedDelegate;
        }
        void MyProxyHandler(object sender, EventArgs e)
        {
            DependencyObject target = sender as DependencyObject;

            if (target == null)
                return;

            var dataContext = GetDataContext(target,ViewType);

            if (dataContext == null)
                throw new Exception(string.Format("DataContext on {0} is null", target));

            MethodInfo methodInfo = dataContext.GetType()
                .GetMethod(ActionName, BindingFlags.Public | BindingFlags.Instance);
            if (methodInfo == null)
                throw new Exception(string.Format("Method({1}) is not found on ViewModel({0})", dataContext.GetType(), ActionName));
            methodInfo.Invoke(dataContext, new object[]{sender,e});
        }
        static Type[] GetParameterTypes(EventInfo eventInfo)
        {
            var invokeMethod = eventInfo.EventHandlerType.GetMethod("Invoke");
            return invokeMethod.GetParameters().Select(p => p.ParameterType).ToArray();
        }

        static object GetDataContext(object target,Type type)
        {
            var depObj = target as DependencyObject;
            if (depObj == null)
                return null;
            if(type==null)
                return depObj.GetValue(FrameworkElement.DataContextProperty) ?? depObj.GetValue(FrameworkContentElement.DataContextProperty);

            var method = typeof(VisualTreeHelperEx).GetMethods().Where(m => m.Name == "FindVisualParent").First();
            var gm = method.MakeGenericMethod(type);
            var parent = gm.Invoke(depObj, new object[]{depObj}) as FrameworkElement;

            //var view = depObj.FindVisualParent<UserControl>();
            if(parent!=null)
                return parent.DataContext;
            return null;
            
        }

    }
}
