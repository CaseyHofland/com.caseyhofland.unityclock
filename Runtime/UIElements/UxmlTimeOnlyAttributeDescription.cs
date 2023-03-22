using System;
using UnityEngine.UIElements;

namespace UnityClock.UIElements
{
    //
    // Summary:
    //     Describes a XML TimeOnly attribute.
    public class UxmlTimeOnlyAttributeDescription : TypedUxmlAttributeDescription<TimeOnly>
    {
        //
        // Summary:
        //     Constructor.
        public UxmlTimeOnlyAttributeDescription()
        {
            base.type = "TimeOnly";
            //base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
            base.defaultValue = TimeOnly.MinValue;
        }

        //
        // Summary:
        //     Retrieves the value of this attribute from the attribute bag. Returns it if it
        //     is found, otherwise return defaultValue.
        //
        // Parameters:
        //   bag:
        //     The bag of attributes.
        //
        //   cc:
        //     The context in which the values are retrieved.
        //
        // Returns:
        //     The value of the attribute.
        public override TimeOnly GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
        {
            return GetValueFromBag(bag, cc, (string s, TimeOnly t) => ConvertValueToLong(s, t), base.defaultValue);
        }

        public bool TryGetValueFromBag(IUxmlAttributes bag, CreationContext cc, ref TimeOnly value)
        {
            return TryGetValueFromBag(bag, cc, (string s, TimeOnly t) => ConvertValueToLong(s, t), base.defaultValue, ref value);
        }

        private static TimeOnly ConvertValueToLong(string v, TimeOnly defaultValue)
        {
            if (v == null || !TimeSpan.TryParse(v, out var result))
            {
                return defaultValue;
            }

            return TimeOnly.FromTimeSpan(result);
        }
    }
}
