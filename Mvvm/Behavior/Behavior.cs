using Pollux.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        public static string GetGridDefinitions(DependencyObject obj)
        {
            return (string)obj.GetValue(GridDefinitionsProperty);
        }

        public static void SetGridDefinitions(DependencyObject obj, string value)
        {
            obj.SetValue(GridDefinitionsProperty, value);
        }

        public static readonly DependencyProperty GridDefinitionsProperty =
            DependencyProperty.RegisterAttached("GridDefinitions", typeof(string), typeof(Layout), new FrameworkPropertyMetadata("[*],[*]",
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure|
                FrameworkPropertyMetadataOptions.AffectsParentArrange,
                new PropertyChangedCallback(OnMessageChanged)));
        //[{Auto,*,Auto},{Auto}]
        public static void OnMessageChanged(DependencyObject d,DependencyPropertyChangedEventArgs e)
        {
            Grid grid = d as Grid;

            if (grid == null)
                return;

            if (e.NewValue == e.OldValue)
                return;
            
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            string message = GetGridDefinitions(grid);
            var match = Regex.Match(message, "\\[(?<ROW>[0-9.,Auto\\*]+)\\],\\[(?<COL>[0-9.,Auto\\*]+)\\]");
            var row = match.Groups["ROW"].Value;
            var col = match.Groups["COL"].Value;

            var rowsLength = row.Split(',');
            var colsLength = col.Split(',');

            foreach(string height in rowsLength)
            {
                double result = 0;

                if(height == "Auto")
                    grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                else if (height == "*")
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                else if (height.EndsWith("*"))
                {
                    if (double.TryParse(height.TrimEnd('*'), out result))
                    {
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(result, GridUnitType.Star) });
                    }
                }
                else if (double.TryParse(height, out result))
                {
                    grid.RowDefinitions.Add(new RowDefinition() { Height =new GridLength( result, GridUnitType.Pixel) });
                }
            }

            foreach (string width in colsLength)
            {
                double result = 0;

                if (width == "Auto")
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                else if(width == "*")
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                else if (width.EndsWith("*"))
                {
                    if (double.TryParse(width.TrimEnd('*'), out result))
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(result, GridUnitType.Star) });
                    }
                }
                else if (double.TryParse(width, out result))
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(result, GridUnitType.Pixel) });
                }
            }
        }
    }
}
