using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Pollux.Mvvm
{
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object),
            typeof(BindingProxy), new UIPropertyMetadata(null));
    }
}

namespace Utilities
{

    /// <summary>
    /// Allows binding to a viewmodel from within a data/contenttemplate that is bound to something else.
    ///
    /// Example:
    ///     <UserControl.Resources>
    ///         <DataContextProxy x:Key="dataContextProxy" />
    ///     </UserControl.Resources>    
    ///     ...
    ///     <DataTemplate>
    ///         <Button Content="Delete"
    ///                 Command="{Binding Source={StaticResource dataContextProxy}, Path=DataSource.DeleteClass}" 
    ///                 CommandParameter="{Binding}" />
    ///     </DataTemplate>
    ///
    /// </summary>
    public class DataContextProxy : FrameworkElement
    {
        public DataContextProxy()
        {
            this.Loaded += OnLoaded;
        }

        public string BindingPropertyName { get; set; }

        public BindingMode BindingMode { get; set; }

        public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register("DataSource", typeof(object), typeof(DataContextProxy), null);

        public object DataSource
        {
            get { return GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var binding = new Binding();

            if (!string.IsNullOrEmpty(BindingPropertyName))
            {
                binding.Path = new PropertyPath(BindingPropertyName);
            }

            binding.Source = DataContext;

            binding.Mode = BindingMode;

            SetBinding(DataSourceProperty, binding);
        }
    }

    /// <summary>

    /// Allows binding to a container (or other framework element) height or width

    ///

    /// Example:

    ///     <UserControl.Resources>

    ///          <utils:ActualSizePropertyProxy Element="{Binding ElementName=SellPanel}"  x:Name="SellPanelSizeProxy"/>

    ///     </UserControl.Resources>    

    ///

    ///     <TextBlock Width="{Binding ElementName=SellPanelSizeProxy, Path=ActualWidthValue}" />

    ///

    /// </summary>

    public class ActualSizePropertyProxy : FrameworkElement, INotifyPropertyChanged
    {

        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(

            "Element",

            typeof(FrameworkElement),

            typeof(ActualSizePropertyProxy),

            new PropertyMetadata(null, OnElementPropertyChanged));

        public event PropertyChangedEventHandler PropertyChanged;

        public FrameworkElement Element
        {

            get { return (FrameworkElement)this.GetValue(ElementProperty); }

            set { this.SetValue(ElementProperty, value); }

        }

        public double ActualHeightValue
        {

            get { return this.Element == null ? 0 : this.Element.ActualHeight; }

        }

        public double ActualWidthValue
        {

            get { return this.Element == null ? 0 : this.Element.ActualWidth; }

        }

        private static void OnElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            ((ActualSizePropertyProxy)d).OnElementChanged(e);

        }

        private void OnElementChanged(DependencyPropertyChangedEventArgs e)
        {

            var oldElement = (FrameworkElement)e.OldValue;

            var newElement = (FrameworkElement)e.NewValue;

            newElement.SizeChanged += this.ElementSizeChanged;

            if (oldElement != null)
            {

                oldElement.SizeChanged -= this.ElementSizeChanged;

            }

            this.NotifyPropChange();

        }

        private void ElementSizeChanged(object sender, SizeChangedEventArgs e)
        {

            this.NotifyPropChange();

        }

        private void NotifyPropChange()
        {

            if (this.PropertyChanged != null)
            {

                this.PropertyChanged(this, new PropertyChangedEventArgs("ActualWidthValue"));

                this.PropertyChanged(this, new PropertyChangedEventArgs("ActualHeightValue"));

            }

        }

    }
}
