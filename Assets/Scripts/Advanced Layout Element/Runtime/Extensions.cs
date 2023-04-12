using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace AP.UI.LayoutProperties
{
    public static class Extensions
    {
        public static void SetProperty(this ILayoutElement element, LayoutPropertyType type, float value)
        {
            if (element is AdvancedLayoutElement custom)
            {
                switch (type)
                {
                    case LayoutPropertyType.MinWidth:
                        custom.minWidth = value;
                        break;
                    case LayoutPropertyType.MinHeight:
                        custom.minHeight = value;
                        break;
                    case LayoutPropertyType.PreferredWidth:
                        custom.preferredWidth = value;
                        break;
                    case LayoutPropertyType.PreferredHeight:
                        custom.preferredHeight = value;
                        break;
                    case LayoutPropertyType.FlexibleWidth:
                        custom.flexibleWidth = value;
                        break;
                    case LayoutPropertyType.FlexibleHeight:
                        custom.flexibleHeight = value;
                        break;
                    default:
                        Debug.LogError($"{nameof(Extensions)}::{nameof(SetProperty) + "+Set"}: {type} is not implemented");
                        break;
                }
            }
            else if (element is LayoutElement layoutElement)
            {
                switch (type)
                {
                    case LayoutPropertyType.MinWidth:
                        layoutElement.minWidth = value;
                        break;
                    case LayoutPropertyType.MinHeight:
                        layoutElement.minHeight = value;
                        break;
                    case LayoutPropertyType.PreferredWidth:
                        layoutElement.preferredWidth = value;
                        break;
                    case LayoutPropertyType.PreferredHeight:
                        layoutElement.preferredHeight = value;
                        break;
                    case LayoutPropertyType.FlexibleWidth:
                        layoutElement.flexibleWidth = value;
                        break;
                    case LayoutPropertyType.FlexibleHeight:
                        layoutElement.flexibleHeight = value;
                        break;
                    default:
                        Debug.LogError($"{nameof(Extensions)}::{nameof(SetProperty) + "+Set"}: {type} is not implemented");
                        break;
                }
            }
        }

        public static float GetProperty(this ILayoutElement element, LayoutPropertyType type)
        {
            switch (type)
            {
                case LayoutPropertyType.MinWidth:
                    return element.minWidth;
                case LayoutPropertyType.MinHeight:
                    return element.minHeight;
                case LayoutPropertyType.PreferredWidth:
                    return element.preferredWidth;
                case LayoutPropertyType.PreferredHeight:
                    return element.preferredHeight;
                case LayoutPropertyType.FlexibleWidth:
                    return element.flexibleWidth;
                case LayoutPropertyType.FlexibleHeight:
                    return element.flexibleHeight;
                default:
                    Debug.LogError($"{nameof(Extensions)}::{nameof(SetProperty) + "+Get"}: {type} is not implemented");
                    return Mathf.Max(element.minWidth, element.minHeight);
            }
        }

    }
}