using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using KEI.Infrastructure.Validation;
using KEI.Infrastructure.Validation.Attributes;

namespace KEI.Infrastructure.Helpers
{
    public static class ReflectionExtentions
    {
        /// <summary>
        /// Checks whether the Type is a subclass of a generic type
        /// </summary>
        /// <param name="toCheck">type</param>
        /// <param name="generic">generic type</param>
        /// <returns></returns>
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        /// <summary>
        /// Check whether type is Primitive type.
        /// returns true for (int, string, float, double, bool and enums)
        /// </summary>
        /// <param name="t"></param>
        /// <returns>Is primitive type</returns>
        public static bool IsPrimitiveType(this Type t) => typeof(IConvertible).IsAssignableFrom(t) || typeof(string) == t;

        /// <summary>
        /// Checks whether a type is a generic list
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Is generic list</returns>
        public static bool IsGenericList(this Type t) => t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(List<>));

        public static bool IsList(this Type t) => typeof(IList).IsAssignableFrom(t);

        /// <summary>
        /// Checks whether you can convert given string to this type
        /// </summary>
        /// <param name="t">Type</param>
        /// <param name="value">value to check</param>
        /// <returns>Can Convert</returns>
        public static bool CanConvert(this Type t, string value) => TypeDescriptor.GetConverter(t).IsValid(value);

        /// <summary>
        /// Converts given string to this type
        /// </summary>
        /// <param name="t">Type</param>
        /// <param name="value">string value</param>
        /// <returns>converted object</returns>
        public static object ConvertFrom(this Type t, string value) => TypeDescriptor.GetConverter(t).ConvertFrom(value);
        public static T ConvertFrom<T>(this Type t, string value) => (T)TypeDescriptor.GetConverter(t).ConvertFrom(value);

        public static ValidatorGroup GetValidators(this PropertyInfo info)
        {
            var attributes = info.GetCustomAttributes<ValidationAttribute>();

            if (attributes.Any() == false)
            {
                return null;
            }

            var builder = ValidationBuilder.Create();

            foreach (var validator in attributes)
            {
                builder.Custom(validator.GetValidator());
            }

            return builder.Validator;
        }

        public static string GetDescription(this PropertyInfo info) => info.GetCustomAttribute<DescriptionAttribute>()?.Description;

        public static string GetCategory(this PropertyInfo info) => info.GetCustomAttribute<CategoryAttribute>()?.Category;

        public static string GetDisplayName(this PropertyInfo info) => info.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

        public static BrowseOptions GetBrowseOption(this PropertyInfo info)
        {
            BrowseOptions browseOption = BrowseOptions.Browsable;

            if (info.GetCustomAttribute<BrowsableAttribute>() is BrowsableAttribute ba)
                browseOption = ba.Browsable ? BrowseOptions.Browsable : BrowseOptions.NonBrowsable;
            else if (info.GetCustomAttribute<ReadOnlyAttribute>() is ReadOnlyAttribute ra)
                browseOption = ra.IsReadOnly ? BrowseOptions.NonEditable : BrowseOptions.Browsable;

            return browseOption;
        }
    }
}
