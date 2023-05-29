using UnityEngine;
using AP.UI.LayoutProperties;
using UnityEngine.UI;
namespace AP.UI
{
    public partial class AdvancedLayoutElement
    {
        /// <summary>
        /// A property is a value that can be used to control the size of the layout element
        /// </summary>
        [System.Serializable]
        public class Property
        {
            public Property(LayoutProperty type)
            {
                m_DefaultType = m_Type = type;
            }

            [SerializeField]
            [HideInInspector]
            AdvancedLayoutElement m_Element;
            RectTransform m_Transform;
            [Tooltip("If an override is set the value of this property \n" +
                    "will be sourced from the override. \n" +
                    "Common override types include:\n" +
                    "- Text Mesh Pro Elements \n" +
                    "- Rect Transforms \n" +
                    "- Images \n" +
                    "- Layout Groups" +
                    "- Other Layout Elements")]
            [SerializeField] UnityEngine.Object m_Override;
            [Tooltip("When a value is retrieved from a property, it is multiplied by it's weight")]
            [SerializeField, Range(0,1)] float m_Weight = 1;
            //[SerializeField, SerializeReference] ILayoutPropertyOverride m_Override;
            [Tooltip("Where the element should source its value from. If there is no override, the element will source the value from itself")]
            [SerializeField] LayoutProperty m_Type;
            [SerializeField, HideInInspector] LayoutProperty m_DefaultType;

            [SerializeField] bool m_Enabled = false;

            [Tooltip("Whether this value can be set durring runtime")]
            [SerializeField] bool m_ReadOnly;

            [Tooltip("The current value that this property is set to")]
            [SerializeField] float m_Value = 0;

            public LayoutProperty Type
            {
                get
                {
                    return m_Type;
                }
                internal set
                {
                    m_Type = value;
                }
            }

            public UnityEngine.Object Override => m_Override;
            public AdvancedLayoutElement Element => m_Element;
            public RectTransform Transform => m_Transform;

            public bool Enabled
            {
                get
                {
                    return m_Enabled;
                }
                set
                {
                    m_Enabled = value;
                }
            }

            public float Value => (m_Enabled) ? m_Value * m_Weight : -1;

            /// <summary>
            /// the raw value refers to the value of the property without any weight applied
            /// </summary>
            public float RawValue
            {
                get
                {
                    return m_Value;
                }
                set
                {
                    if(m_ReadOnly)
                    {
                        return;
                    }
                    if(m_Value != value)
                    {
                        m_Value = value;
                        Element.m_HasChanged = true;
                        //LayoutRebuilder.MarkLayoutForRebuild(Transform.parent as RectTransform);
                    }
                }
            }

            /// <summary>
            /// the weight of the property. This is used to scale the value of the property
            /// </summary>
            public float Weight { 
                get => m_Weight;
                set 
                {
                    if(m_Weight != value)
                    {
                        m_Weight = value;
                        Element.m_HasChanged = true;
#if UNITY_EDITOR
                        if (!UnityEditor.EditorApplication.isPlaying)
                        {
                            LayoutRebuilder.ForceRebuildLayoutImmediate(Transform.parent as RectTransform);
                        }
#endif
                        //LayoutRebuilder.MarkLayoutForRebuild(Transform.parent as RectTransform);
                    }
                }
            }

            /// <summary>
            /// Validates the property and sets the value of the property
            /// </summary>
            /// <param name="element"></param>
            /// <param name="property"></param>
            public void Validate(AdvancedLayoutElement element, LayoutProperty property)
            {
                m_Element = element;
                m_Transform = element.transform as RectTransform;
                m_DefaultType = property;

                if (m_Override)
                {
                    switch (m_Override)
                    {
                        case GameObject gameObject:
                            if (gameObject.GetComponent<ILayoutElement>() is UnityEngine.Object obj)
                            {
                                m_Override = obj;
                            }
                            else if (gameObject.GetComponent<RectTransform>() is UnityEngine.Object rT)
                            {
                                m_Override = rT;
                            }
                            else
                            {
                                m_Override = null;
                            }
                            break;
                        case RectTransform rectTransform:
                        case ILayoutElement layoutElement:
                            break;
                        default:
                            Debug.LogError($"{nameof(Property)}::{nameof(Validate)}: {m_Override} Overrides Must be a {nameof(RectTransform)} or implement {nameof(ILayoutElement)}", Element);
                            m_Override = null;
                            break;
                    }
                    if(m_Override == Element)
                    {
                        return;
                    }
                }

                CalculateLayout();
            }

            /// <summary>
            /// Called by Unities LayoutRebuilder to calculate the layout of the element
            /// </summary>
            public void CalculateLayout()
            {
                var newVal = GetUnscaledValue();
                if(m_Value != newVal)
                {
                    m_Value = newVal;
                    Element.m_HasChanged = true;
                }
            }
            
            /// <summary>
            /// Internal method to get the value of the property
            /// </summary>
            /// <returns></returns>
            /// <exception cref="System.NotImplementedException"></exception>
            float GetUnscaledValue()
            {
                if (m_Override is RectTransform oTransform)
                {
                    switch (m_Type)
                    {
                        case LayoutProperty.MinWidth:
                        case LayoutProperty.PreferredWidth:
                            return oTransform.rect.width;
                        case LayoutProperty.PreferredHeight:
                        case LayoutProperty.MinHeight:
                            return oTransform.rect.height;
                        case LayoutProperty.FlexibleWidth:
                            return oTransform.rect.width - Transform.rect.width;
                        case LayoutProperty.FlexibleHeight:
                            return oTransform.rect.height - Transform.rect.height;
                        default:
                            throw new System.NotImplementedException();
                    }
                }
                else if(m_Override is ILayoutElement @override)
                {
                    return @override.GetProperty(m_Type);
                }

                if (Element[Type].m_Type != m_DefaultType)
                {
                    return Element[Type].Value;
                }
                else
                {
                    return Element[Type].RawValue;
                }
            }
        }
    }
}
