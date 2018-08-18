using LoadingIndicators.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Pollux.Behavior
{
    public class LoadingIndicatorEx
    {
        public static bool GetBinding(DependencyObject obj)
        {
            //System.Diagnostics.Debugger.Break();
            return (bool)obj.GetValue(BindingProperty);
        }

        public static void SetBinding(DependencyObject obj, bool value)
        {
            obj.SetValue(BindingProperty, value);
        }

        // Using a DependencyProperty as the backing store for Binding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.RegisterAttached("Binding", typeof(bool), typeof(LoadingIndicatorEx), new PropertyMetadata(false,
                (o,e)=>
                {
                    //GetBinding(o);

                    //_loadingIndicator.IsActive = (bool)e.NewValue;
                }));



        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShowLoadingIndicator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(LoadingIndicatorEx), new PropertyMetadata(false,
                new PropertyChangedCallback(OnPropertyChanged)));
        private static void OnPropertyChanged(DependencyObject d,DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue == true)
            {
                if(d is ContentControl)
                {
                    var element = d as ContentControl;
                    element.Initialized += Element_Initialized;
                }
            }
        }

        private static void Element_Initialized(object sender, EventArgs e)
        {
            var element = sender as ContentControl;

            var grid = new Grid();
            var rect = new Rectangle() { Fill = Brushes.LightGray,Opacity=0};
            rect.SetValue(Panel.ZIndexProperty, 1000);

            var _loadingIndicator = new LoadingIndicators.WPF.LoadingIndicator() { SpeedRatio = 2};
            grid.Children.Add(rect);
            grid.Children.Add(_loadingIndicator);
            var content = element.Content as FrameworkElement;
            element.Content = null;
            var binding = (sender as FrameworkElement).GetBindingExpression(LoadingIndicatorEx.BindingProperty);
            _loadingIndicator.SetBinding(LoadingIndicator.IsActiveProperty, binding.ParentBinding);

            var binding2 = new Binding("IsActive");
            binding2.Source = _loadingIndicator;
            binding2.Converter = new BooleanToVisibilityConverter();
            rect.SetBinding(FrameworkElement.VisibilityProperty, binding2);

            if (content != null)
            {
                grid.Children.Add(content);
            }
            element.Content = grid;
        }
    }
}
