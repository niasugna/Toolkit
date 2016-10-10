using LoadingIndicators.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pollux.Behavior
{
    public class LoadingIndicatorEx
    {
        static LoadingIndicator _loadingIndicator = new LoadingIndicators.WPF.LoadingIndicator();

        public static bool GetBinding(DependencyObject obj)
        {
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
                    _loadingIndicator.IsActive = (bool)e.NewValue;
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
            var border = new Border() {IsHitTestVisible = false,IsEnabled= false, Background= new SolidColorBrush(Color.FromArgb(200,255,255,255))};
            border.SetValue(Panel.ZIndexProperty, 98);
            
            grid.Children.Add(_loadingIndicator);
            var content = element.Content as FrameworkElement;
            element.Content = null;
            //border.Child = content;
            grid.Children.Add(content);
            grid.Children.Add(border);
            element.Content = grid;

        }
    }
}
