using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Pollux.Helper;
using System.Text.RegularExpressions;

namespace Pollux.Behavior
{
    public class EventToMethod
    {
        public static object GetViewModel(DependencyObject obj)
        {
            return (object)obj.GetValue(ViewModelProperty);
        }

        public static void SetViewModel(DependencyObject obj, object value)
        {
            obj.SetValue(ViewModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.RegisterAttached("ViewModel", typeof(object), typeof(EventToMethod), new PropertyMetadata(null));

        
        public static string GetMessage(DependencyObject obj)
        {
            return (string)obj.GetValue(MessageProperty);
        }

        public static void SetMessage(DependencyObject obj, string value)
        {
            obj.SetValue(MessageProperty, value);
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.RegisterAttached("Message", typeof(string), typeof(EventToMethod), new PropertyMetadata("",
                new PropertyChangedCallback(OnMessageChanged)));

        static void OnMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;
            try
            {
                object current = d.FindViewModel();

                var allTriggers = Interaction.GetTriggers(d);
                var info = Parse(d, GetMessage(d));

                info.AssociatedObject = d;
                info.ViewModel = d.FindViewModel();
                info.View = d.FindVisualParent<UserControl>();

                var trigger = new System.Windows.Interactivity.EventTrigger(info.EventName);
                trigger.Actions.Add(new InvokeMethodAction()
                {
                    MethodName = info.MethodName,
                    MessageInfo = info
                });

                allTriggers.Add(trigger);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[Click]=[MethodName]
        static MessageInfo Parse(DependencyObject d, string message)
        {
            message = message.Replace("Event", string.Empty);
            message = message.Replace("Action", string.Empty);
            message = message.Replace(" ", string.Empty);
            var match = Regex.Match(message, "(?<EVENT>[\\w]+)([\\]])(=)(\\[)(?<METHOD_NAME>[\\w]+)(?<PARAMETER>[\\($\\w,\\)]+)?(\\])");
            var ev = match.Groups["EVENT"].Value;
            var mn = match.Groups["METHOD_NAME"].Value;
            var pm = match.Groups["PARAMETER"].Value
                .Replace("(", string.Empty)
                .Replace(")", string.Empty);


            var info = new MessageInfo
            {
                EventName = ev,
                MethodName = mn,
                Parameters = pm.Split(',').ToArray()//string.IsNullOrWhiteSpace(pm)?null:pm.Split(',').ToArray()
            };

            /*
            if (string.IsNullOrWhiteSpace(pm))
                info.MessageInfo = null;
            else
                info.MessageInfo = pm.Split(',').ToList().Select(s => ParseParameter(d, s)).ToArray();
            */
            return info;

        }


    }
    public class MessageInfo
    {
        public DependencyObject AssociatedObject { get; set; }

        public string EventName { get; set; }
        public string MethodName { get; set; }
        public string[] Parameters { get; set; }

        public object ViewModel { get; set; }
        public object View { get; set; }

    }
    public class InvokeMethodAction : TriggerAction<DependencyObject>
    {
        public string MethodName { get; set; }

        public MessageInfo MessageInfo { get; set; }

        protected override void Invoke(object o)
        {

            try
            {
                var fe = this.AssociatedObject as FrameworkElement;
                var root = fe.FindVisualTreeRoot();
                FrameworkElement view = this.AssociatedObject.FindVisualParent<UserControl>();

                if (view == null)
                    view = this.AssociatedObject.FindVisualParent<Window>();

                //template??
                if (view == null)
                {
                    view = (fe.GetSelfAndAncestors().Last() as FrameworkElement).Parent as FrameworkElement;
                }
                //if (fe.TemplatedParent != null)
                //{
                //    var p = ContentOperations.GetParent(fe.TemplatedParent as ContentElement);
                //}
                if (view == null)
                    return;

                object viewModel = view.DataContext;

                if (viewModel != null)
                {
                    var names = (from method in viewModel.GetType().GetMethods()
                                 select method.Name);
                    var result = (from method in viewModel.GetType().GetMethods()
                                  //let parameters = _method.GetParameters()
                                  //let hasParameters = parameters.Length > 0
                                  where method.Name == MethodName
                                  select method).FirstOrDefault();

                    if (result != null)
                    {
                        var parameters = this.MessageInfo.Parameters
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Select(s => ParseParameter(this.AssociatedObject, (string)s)).ToArray();
                        result.Invoke(viewModel, parameters);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("InvokeMethodAction : \n" + e.Message);
            }
        }
        static object ParseParameter(DependencyObject d, string pm)
        {
            switch (pm)
            {
                case "$this":
                    return d;
                case "$view":
                    return d.FindVisualParent<UserControl>();
                case "$window":
                    return d.FindVisualParent<Window>();
                case "$dataContext":
                    return d.FindViewModel();
                default:
                    return null;
            }
        }
        private object FindViewModel(DependencyObject associatedObject)
        {
            var src = associatedObject as FrameworkElement;
            object viewModel = null;
            while (viewModel == null)
            {
                if (src != null)
                    viewModel = src.DataContext;
                else
                {
                    if (src.Parent == null)
                        break;
                    src = src.Parent as FrameworkElement;
                }
            }
            return viewModel;
        }
    }
}
