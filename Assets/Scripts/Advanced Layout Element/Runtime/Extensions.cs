using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace AP.UI.LayoutProperties
{
    public static class Extensions
    {
        /// <summary>
        /// Set the value of a property with the given type.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void SetProperty(this ILayoutElement element, LayoutProperty property, float value)
        {
            if (element is AdvancedLayoutElement custom)
            {
                custom[property].RawValue = value;
            }
            else if (element is LayoutElement layoutElement)
            {
                switch (property)
                {
                    case LayoutProperty.MinWidth:
                        layoutElement.minWidth = value;
                        break;
                    case LayoutProperty.MinHeight:
                        layoutElement.minHeight = value;
                        break;
                    case LayoutProperty.PreferredWidth:
                        layoutElement.preferredWidth = value;
                        break;
                    case LayoutProperty.PreferredHeight:
                        layoutElement.preferredHeight = value;
                        break;
                    case LayoutProperty.FlexibleWidth:
                        layoutElement.flexibleWidth = value;
                        break;
                    case LayoutProperty.FlexibleHeight:
                        layoutElement.flexibleHeight = value;
                        break;
                    default:
                        Debug.LogError($"{nameof(Extensions)}::{nameof(SetProperty) + "+Set"}: {property} is not implemented");
                        break;
                }
            }
        }

        /// <summary>
        /// Get the value of a property with the given type.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static float GetProperty(this ILayoutElement element, LayoutProperty type)
        {
            switch (type)
            {
                case LayoutProperty.MinWidth:
                    return element.minWidth;
                case LayoutProperty.MinHeight:
                    return element.minHeight;
                case LayoutProperty.PreferredWidth:
                    return element.preferredWidth;
                case LayoutProperty.PreferredHeight:
                    return element.preferredHeight;
                case LayoutProperty.FlexibleWidth:
                    return element.flexibleWidth;
                case LayoutProperty.FlexibleHeight:
                    return element.flexibleHeight;
                default:
                    Debug.LogError($"{nameof(Extensions)}::{nameof(SetProperty) + "+Get"}: {type} is not implemented");
                    return Mathf.Max(element.minWidth, element.minHeight);
            }
        }

    }
}