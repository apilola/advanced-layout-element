using UnityEngine;
using AP.UI.LayoutProperties;
using UnityEngine.UI;
namespace AP.UI
{
    public partial class AdvancedLayoutElement
    {
        [System.Serializable]
        public class Property
        {
            public Property(LayoutPropertyType type)
            {
                m_DefaultType = m_Type = type;
            }

            [SerializeField]
            [HideInInspector]
            AdvancedLayoutElement m_Element;
            RectTransform m_Transform;
            [SerializeField] UnityEngine.Object m_Override;
            [SerializeField, Range(0,1)] float m_Weight = 1;
            //[SerializeField, SerializeReference] ILayoutPropertyOverride m_Override;
            [SerializeField] LayoutPropertyType m_Type;
            [SerializeField, HideInInspector] LayoutPropertyType m_DefaultType;
            [SerializeField] bool m_Enabled = false;
            [SerializeField] bool m_ReadOnly;
            [SerializeField] float m_Value = 0;

            public LayoutPropertyType Type
            {
                get
                {
                    return m_Type;
                }
                set
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

            public void Validate(AdvancedLayoutElement element, LayoutPropertyType type)
            {
                m_Element = element;
                m_Transform = element.transform as RectTransform;
                m_DefaultType = type;

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

            public void CalculateLayout()
            {
                var newVal = GetUnscaledValue();
                if(m_Value != newVal)
                {
                    m_Value = newVal;
                    Element.m_HasChanged = true;
                }
            }
            

            float GetUnscaledValue()
            {
                if (m_Override is RectTransform oTransform)
                {
                    switch (m_Type)
                    {
                        case LayoutPropertyType.MinWidth:
                        case LayoutPropertyType.PreferredWidth:
                            return oTransform.rect.width;
                        case LayoutPropertyType.PreferredHeight:
                        case LayoutPropertyType.MinHeight:
                            return oTransform.rect.height;
                        case LayoutPropertyType.FlexibleWidth:
                            return oTransform.rect.width - Transform.rect.width;
                        case LayoutPropertyType.FlexibleHeight:
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
