using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux.Converters
{
    public static class Converters
    {
        public static EnumBooleanConverter EnumBooleanConverter = new EnumBooleanConverter();
        public static EnumCheckedConverter EnumCheckedConverter = new EnumCheckedConverter();
        public static NullToVisibleConverter NullToVisibleConverter = new NullToVisibleConverter();
        public static NotNullToVisibleConverter NotNullToVisibleConverter = new NotNullToVisibleConverter();
        public static StreamToImageConverter StreamToImageConverter = new StreamToImageConverter();
    }
}
