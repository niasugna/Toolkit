using Pollux.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xaml;

namespace Pollux.Behavior
{
    //usage
    //<TextBox date:InputBehaviour.IsDigitOnly="True" />
    public static class InputDigitOnlyBehavior
    {
        public static bool GetIsDigitOnly(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDigitOnlyProperty);
        }

        public static void SetIsDigitOnly(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDigitOnlyProperty, value);
        }

        public static readonly DependencyProperty IsDigitOnlyProperty =
          DependencyProperty.RegisterAttached("IsDigitOnly",
          typeof(bool), typeof(InputDigitOnlyBehavior),
          new UIPropertyMetadata(false, OnIsDigitOnlyChanged));

        private static void OnIsDigitOnlyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // ignoring error checking
            TextBox textBox = (TextBox)sender;
            bool isDigitOnly = (bool)(e.NewValue);

            if (isDigitOnly)
                textBox.PreviewTextInput += BlockNonDigitCharacters;
            else
                textBox.PreviewTextInput -= BlockNonDigitCharacters;
        }

        private static void BlockNonDigitCharacters(object sender, TextCompositionEventArgs e)
        {
            foreach (char ch in e.Text)
                if (!Char.IsDigit(ch))
                    e.Handled = true;
        }
    }
    ///<summary
    /////http://www.codeproject.com/Articles/35721/WPF-Blend-Interactions-Behaviours
     /// </summary>
    //Example : System.Windows.Interactivity.WPF
    //xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity" 
    //xmlns:ei="http://schemas.microsoft.com/expression/2010/interactions"
    //usage
    //<i:Interaction.Behaviors>
    //          <date:DragBehavior/>
    //          <date:ResizeBehavior/>
    //</i:Interaction.Behaviors>
    public class FocusBehavior : Behavior<TextBox>
    {
        TextBox AssociatedTextBox { get { return AssociatedObject; } }
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedTextBox.Focus();
            AssociatedTextBox.Loaded += OnAssociatedTextBoxLoaded;
        }
        protected override void OnDetaching()
        {
            AssociatedTextBox.Loaded -= OnAssociatedTextBoxLoaded;
            base.OnDetaching();
        }
        void OnAssociatedTextBoxLoaded(object sender, EventArgs e)
        {
            AssociatedTextBox.Focus();
        }
    }

    //usage
    //xmlns:mvvm="clr-namespace:Wpf.Behavior"
    //<ListView b:GridViewSortBehavior.AutoSort="True">
    //<GridViewColumn b:GridViewSortBehavior.SortDescription="ServerName">
    public class GridViewSortBehavior
    {
        #region Attached properties




        public static string GetSortDescription(DependencyObject obj)
        {
            return (string)obj.GetValue(SortDescriptionProperty);
        }

        public static void SetSortDescription(DependencyObject obj, string value)
        {
            obj.SetValue(SortDescriptionProperty, value);
        }

        // Using a DependencyProperty as the backing store for SortDescription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortDescriptionProperty =
            DependencyProperty.RegisterAttached("SortDescription", typeof(string), typeof(GridViewSortBehavior), new PropertyMetadata(null));



        public static bool GetAutoSort(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoSortProperty);
        }

        public static void SetAutoSort(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoSortProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoSort.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoSortProperty =
            DependencyProperty.RegisterAttached("AutoSort", typeof(bool), typeof(GridViewSortBehavior), new PropertyMetadata(false,
                new PropertyChangedCallback(RegisterHeaderClickEvent)));

        public static void RegisterHeaderClickEvent(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl listView = o as ItemsControl;
            if (listView != null)
            {
                if (GetAutoSort(listView) == true) // Don't change click handler if AutoSort enabled
                {
                    listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                }
                else
                {
                    listView.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                }
            }
        }
        #endregion

        private static void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked != null)
            {
                string propertyName = GetSortDescription(headerClicked.Column);
                if (!string.IsNullOrEmpty(propertyName))
                {
                    ListView listView = VisualTreeHelperEx.FindVisualParent<ListView>(headerClicked);
                    if (listView != null)
                    {
                        if (GetAutoSort(listView))
                        {
                            ApplySort(listView.Items, propertyName);
                        }
                    }
                }
            }
        }

        public static void ApplySort(ICollectionView view, string propertyName)
        {
            ListSortDirection direction = ListSortDirection.Ascending;
            if (view.SortDescriptions.Count > 0)
            {
                SortDescription currentSort = view.SortDescriptions[0];
                if (currentSort.PropertyName == propertyName)
                {
                    if (currentSort.Direction == ListSortDirection.Ascending)
                        direction = ListSortDirection.Descending;
                    else
                        direction = ListSortDirection.Ascending;
                }
                view.SortDescriptions.Clear();
            }
            if (!string.IsNullOrEmpty(propertyName))
            {
                view.SortDescriptions.Add(new SortDescription(propertyName, direction));
            }
        }
    }
    public static class Layout
    {


        public static string GetGrids(DependencyObject obj)
        {
            return (string)obj.GetValue(GridsProperty);
        }

        public static void SetGrids(DependencyObject obj, string value)
        {
            obj.SetValue(GridsProperty, value);
        }

        public static readonly DependencyProperty GridsProperty =
            DependencyProperty.RegisterAttached("Grids", typeof(string), typeof(Layout), new PropertyMetadata("[*],[*]",
                new PropertyChangedCallback(OnMessageChanged)));
        //[{Auto,*,Auto},{Auto}]
        public static void OnMessageChanged(DependencyObject d,DependencyPropertyChangedEventArgs e)
        {
            Grid grid = d as Grid;

            if (grid == null)
                return;

            if (e.NewValue == e.OldValue)
                return;

            string message = GetGrids(grid);
            var match = Regex.Match(message, "\\[(?<ROW>[0-9.,Auto\\*]+)\\],\\[(?<COL>[0-9.,Auto\\*]+)\\]");
            var row = match.Groups["ROW"].Value;
            var col = match.Groups["COL"].Value;

            var rowsLength = row.Split(',');
            var colsLength = col.Split(',');

            foreach(string l in rowsLength)
            {
                double result = 0;

                if(l == "Auto")
                    grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                else if (l == "*")
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                else if (l.EndsWith("*"))
                {
                    if (double.TryParse(l.TrimEnd('*'), out result))
                    {
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(result, GridUnitType.Star) });
                    }
                }
                else if (double.TryParse(l, out result))
                {
                    grid.RowDefinitions.Add(new RowDefinition() { Height =new GridLength( result, GridUnitType.Pixel) });
                }
            }

            foreach (string l in colsLength)
            {
                double result = 0;

                if (l == "Auto")
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                else if(l == "*")
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                else if (l.EndsWith("*"))
                {
                    if (double.TryParse(l.TrimEnd('*'), out result))
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(result, GridUnitType.Star) });
                    }
                }
                else if (double.TryParse(l, out result))
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(result, GridUnitType.Pixel) });
                }
            }
            
            

        }
    }

    public class EventToMethod
    {
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

        static void OnMessageChanged(DependencyObject d,DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            object current = d.FindViewModel();

            var allTriggers = Interaction.GetTriggers(d);
            var info = Parse(d,GetMessage(d));

            info.AssociatedObject = d;
            info.ViewModel = d.FindViewModel();
            info.View = d.FindVisualParent<UserControl>();

            var trigger = new System.Windows.Interactivity.EventTrigger(info.EventName);
            trigger.Actions.Add(new InvokeMethodAction()
                {
                     MethodName=info.MethodName,
                     MessageInfo = info
                });

            allTriggers.Add(trigger);
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
                FrameworkElement view = this.AssociatedObject.FindVisualParent<UserControl>();

                if(view == null)
                    view = this.AssociatedObject.FindVisualParent<Window>();

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
                            .Where(s=>!string.IsNullOrWhiteSpace(s))
                            .Select(s=>ParseParameter(this.AssociatedObject, (string)s)).ToArray();
                        result.Invoke(viewModel, parameters);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
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


    public class ConverterExtension : MarkupExtension
    {
        private Binding Binding;
        private string _method;

        public ConverterExtension(string method)
        {
            this.Binding = new Binding();
            //int pi = method.IndexOf('.');
            //this.Binding.ElementName = method.Substring(0, pi);
            //this.Binding.Path = new System.Windows.PropertyPath(
            //method.Substring(pi + 1, method.Length - pi - 1));

            _method = method;
    
        }
 
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var rootObjectProvider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;
            var root = rootObjectProvider.RootObject as FrameworkElement;
            //root.DataContextChanged += root_DataContextChanged;
            //var isDesignMode = ServiceProvider.IsDesignMode;

            /*
            var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var target = provideValueTarget.TargetObject as FrameworkElement;

            var depObj = target as DependencyObject;
            if (depObj == null)
                return null;
            */
 
            

            //var isDesignMode = DesignerProperties.GetIsInDesignMode(target);


            return new Pollux.Converter.CallMethodConverter(root,_method);

            //return this.Binding.ProvideValue(serviceProvider);
        }

        void root_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var vm = fe.GetValue(FrameworkElement.DataContextProperty) ?? fe.GetValue(FrameworkContentElement.DataContextProperty);
        }
 
    

    }
}
