using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pollux.Helper
{
    public static class EnumHelper
    {
        //enum Animal
        //{
        //    [Description("")]
        //    NotSet = 0,
        //    [Description("Giant Panda")]
        //    GiantPanda = 1,
        //    [Description("Lesser Spotted Anteater")]
        //    LesserSpottedAnteater = 2
        //}
        ///usage:
        ///string myAnimal = Animal.GiantPanda.GetDescription();// = "Giant Panda"
        public static string GetDescription(this Enum @enum)
        {
            if (@enum == null)
                return null;

            string description = @enum.ToString();

            FieldInfo fieldInfo = @enum.GetType().GetField(description);
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Any())
                return attributes[0].Description;

            return description;
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name) // I prefer to get attributes this way
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }
    }
}
