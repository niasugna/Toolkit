using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;

namespace Pollux.Mvvm
{
    //使用ViewModel上的 Convert Function => Converter 
    public class FunctionConverterExtension : MarkupExtension
    {
        private string _method;

        public FunctionConverterExtension(string method)
        {
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
            //var isDesignMode = serviceProvider.IsDesignMode;

            /*
            var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var target = provideValueTarget.TargetObject as FrameworkElement;

            var depObj = target as DependencyObject;
            if (depObj == null)
                return null;
            */



            //var isDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(root);

            return new Pollux.Converter.CallMethodConverter(root, _method);

            //return this.Binding.ProvideValue(serviceProvider);
        }

        void root_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var vm = fe.GetValue(FrameworkElement.DataContextProperty) ?? fe.GetValue(FrameworkContentElement.DataContextProperty);
        }
    }


}
