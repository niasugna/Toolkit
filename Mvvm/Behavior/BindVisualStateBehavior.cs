using System.Windows;
using System.Windows.Interactivity;
using Microsoft.Expression.Interactivity;
using Microsoft.Expression.Interactivity.Core;

namespace Pollux.Behavior
{
    public class VisualState
    {
        public static ExtendedVisualStateManager ExtendedVisualStateManager = new ExtendedVisualStateManager();
        public static bool GetInitialized(DependencyObject obj)
        {
            return (bool)obj.GetValue(InitializedProperty);
        }

        public static void SetInitialized(DependencyObject obj, bool value)
        {
            obj.SetValue(InitializedProperty, value);
        }

        // Using a DependencyProperty as the backing store for Initialized.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InitializedProperty =
            DependencyProperty.RegisterAttached("Initialized", typeof(bool), typeof(VisualState), new PropertyMetadata(true));

        public static string GetStateName(DependencyObject obj)
        {
            return (string)obj.GetValue(StateNameProperty);
        }

        public static void SetStateName(DependencyObject obj, string value)
        {
            obj.SetValue(StateNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for StateName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateNameProperty =
            DependencyProperty.RegisterAttached("StateName", typeof(string), typeof(VisualState), new PropertyMetadata(VisualStatePropertyChanged));
        
        private static void VisualStatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var AssociatedObject = obj as FrameworkElement;
            if (AssociatedObject == null) return;

            VisualStateManager.SetCustomVisualStateManager(AssociatedObject, ExtendedVisualStateManager);
            
            var groups = VisualStateManager.GetVisualStateGroups(AssociatedObject);

            foreach(var g in groups)
            {
                ExtendedVisualStateManager.SetUseFluidLayout(g as VisualStateGroup, true);
            }

            FrameworkElement stateTarget;
            if (!VisualStateUtilities.TryFindNearestStatefulControl(AssociatedObject, out stateTarget)) return;

            bool useTransitions = GetInitialized(obj);
            VisualStateUtilities.GoToState(stateTarget, (string)args.NewValue, useTransitions);
            SetInitialized(obj,true);
        }
    }
    public class BindVisualStateBehavior : Behavior<FrameworkElement>
    {
        private bool _initialized;

        public static DependencyProperty StateNameProperty = DependencyProperty.Register("StateName", typeof(string), typeof(BindVisualStateBehavior), new PropertyMetadata(VisualStatePropertyChanged));
        public string StateName { get { return (string)GetValue(StateNameProperty); } set { SetValue(StateNameProperty, value); } }

        public void UpdateVisualState(string visualState)
        {
            if (AssociatedObject == null) return;
            
            FrameworkElement stateTarget;
            if (!VisualStateUtilities.TryFindNearestStatefulControl(AssociatedObject, out stateTarget)) return;

            bool useTransitions = _initialized;
            VisualStateUtilities.GoToState(stateTarget, visualState, useTransitions);
            _initialized = true;
        }

        private static void VisualStatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((BindVisualStateBehavior)obj).UpdateVisualState((string)args.NewValue);
        }
    }
}
