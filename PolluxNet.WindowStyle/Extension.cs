using System.Windows;

namespace PolluxNet.WindowStyle
{
    public static class Extension
    {
        public static T FindVisualParent<T>(this DependencyObject obj) where T : DependencyObject
        {
            while (obj != null && !(obj is T))
                obj = System.Windows.Media.VisualTreeHelper.GetParent(obj);

            return (T)obj;
        }
    }
}
