using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AP.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public partial class AdvancedLayoutElement : UIBehaviour, ILayoutElement, ILayoutIgnorer
    {
        public Property this[LayoutPropertyType type]
        {
            get
            {
                switch (type)
                {
                    case LayoutPropertyType.MinWidth:
                        return m_MinWidthProp;
                    case LayoutPropertyType.MinHeight:
                        return m_MinHeightProp;
                    case LayoutPropertyType.PreferredWidth:
                        return m_PreferredWidthProp;
                    case LayoutPropertyType.PreferredHeight:
                        return m_PreferredHeightProp;
                    case LayoutPropertyType.FlexibleWidth:
                        return m_FlexibleWidthProp;
                    case LayoutPropertyType.FlexibleHeight:
                        return m_FlexibleHeightProp;
                    default:
                        throw new System.NotImplementedException();
                }

            }
        }
        [Tooltip("Whether or not the element will ignore the parent layout group")]
        [SerializeField] bool m_IgnoreLayout = false;
        [Tooltip("Setting this value to true will allow this component to" +
                " use the Min Width and Min Height to control the size of this component")]
        [SerializeField] bool m_LayoutIndependent = false;

        [Tooltip("Controls the minimum width for the ILayoutElement interface")]
        [SerializeField] Property m_MinWidthProp = new(LayoutPropertyType.MinWidth);
        [Tooltip("Controls the minimum height for the ILayoutElement interface")]
        [SerializeField] Property m_MinHeightProp = new(LayoutPropertyType.MinHeight);
        [Tooltip("Controls the preffered width for the ILayoutElement interface")]
        [SerializeField] Property m_PreferredWidthProp = new(LayoutPropertyType.PreferredWidth);
        [Tooltip("Controls the preffered height for the ILayoutElement interface")]
        [SerializeField] Property m_PreferredHeightProp = new(LayoutPropertyType.PreferredHeight);
        [Tooltip("Controls the flexible width for the ILayoutElement interface")]
        [SerializeField] Property m_FlexibleWidthProp = new(LayoutPropertyType.FlexibleWidth);
        [Tooltip("Controls the flexibile for the ILayoutElement interface")]
        [SerializeField] Property m_FlexibleHeightProp = new(LayoutPropertyType.FlexibleHeight);
        [Tooltip("Override advanced layout element's default priority")]
        [SerializeField] bool m_OverridePriority = false;
        [Tooltip("Layout Priority")]
        [SerializeField] int m_LayoutPriority = int.MaxValue;

        bool m_HasChanged = false;
        RectTransform m_Transform;
        public new RectTransform transform
        {
            get
            {
                if(m_Transform == null)
                {
                    m_Transform = base.transform as RectTransform;
                }

                return m_Transform;
            }
        }


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if(UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
            {
                return;
            }
            m_MinWidthProp.Validate(this, LayoutPropertyType.MinWidth);
            m_MinHeightProp.Validate(this, LayoutPropertyType.MinHeight);
            m_PreferredWidthProp.Validate(this, LayoutPropertyType.PreferredWidth);
            m_PreferredHeightProp.Validate(this, LayoutPropertyType.PreferredHeight);
            m_FlexibleWidthProp.Validate(this, LayoutPropertyType.FlexibleWidth);
            m_FlexibleHeightProp.Validate(this, LayoutPropertyType.FlexibleHeight);
            if(transform.hasChanged)
            {
                m_HasChanged = true;
            }
        }
#endif

        protected override void Awake()
        {
            m_MinWidthProp.Validate(this, LayoutPropertyType.MinWidth);
            m_MinHeightProp.Validate(this, LayoutPropertyType.MinHeight);
            m_PreferredWidthProp.Validate(this, LayoutPropertyType.PreferredWidth);
            m_PreferredHeightProp.Validate(this, LayoutPropertyType.PreferredHeight);
            m_FlexibleWidthProp.Validate(this, LayoutPropertyType.FlexibleWidth);
            m_FlexibleHeightProp.Validate(this, LayoutPropertyType.FlexibleHeight);

            if (transform.hasChanged)
            {
                m_HasChanged = true;
            }
        }



        public float minWidth
        {
            get => m_MinWidthProp.RawValue;
            set => m_MinWidthProp.RawValue = value;
        }

        public float minHeight
        {
            get => m_MinHeightProp.RawValue;
            set => m_MinHeightProp.RawValue = value;
        }

        public float preferredWidth
        {
            get => m_PreferredWidthProp.RawValue;
            set => m_PreferredWidthProp.RawValue = value;
        }

        public float preferredHeight
        {
            get => m_PreferredHeightProp.RawValue;
            set => m_PreferredHeightProp.RawValue = value;
        }

        public float flexibleWidth
        {
            get => m_FlexibleWidthProp.RawValue;
            set => m_FlexibleWidthProp.RawValue = value;
        }

        public float flexibleHeight
        {
            get => m_FlexibleHeightProp.RawValue;
            set => m_FlexibleHeightProp.RawValue = value;
        }

        public int layoutPriority { 
            get => m_LayoutPriority;
            set => m_LayoutPriority = value;
        }

        public bool overridePriority
        {
            get => m_OverridePriority;
            set => m_OverridePriority = value;
        }

        public bool ignoreLayout { get => m_IgnoreLayout; set => m_IgnoreLayout = value; }

        float ILayoutElement.minWidth => m_MinWidthProp.Value;

        float ILayoutElement.minHeight => m_MinHeightProp.Value;

        float ILayoutElement.preferredWidth => m_PreferredWidthProp.Value;

        float ILayoutElement.preferredHeight => m_PreferredHeightProp.Value;

        float ILayoutElement.flexibleWidth => m_FlexibleWidthProp.Value;

        float ILayoutElement.flexibleHeight => m_FlexibleHeightProp.Value;

        int ILayoutElement.layoutPriority => (m_OverridePriority) ? m_LayoutPriority : int.MaxValue;

        public void CalculateLayoutInputHorizontal()
        {
            m_MinWidthProp.CalculateLayout();
            m_PreferredWidthProp.CalculateLayout();
            m_FlexibleWidthProp.CalculateLayout();
        }

        public void CalculateLayoutInputVertical()
        {
            m_MinHeightProp.CalculateLayout();
            m_PreferredHeightProp.CalculateLayout();
            m_FlexibleHeightProp.CalculateLayout();
        }

        private void LateUpdate()
        {
            CleanDirty();
        }

        private void CleanDirty()
        {
            if (m_HasChanged)
            {
                if (ignoreLayout && m_LayoutIndependent)
                {
                    if(m_MinHeightProp.Enabled)
                    {
                        m_MinHeightProp.CalculateLayout();
                    }

                    if(m_MinWidthProp.Enabled)
                    {
                        m_MinWidthProp.CalculateLayout();
                    }

                    transform.sizeDelta = new Vector2(m_MinWidthProp.Value, m_MinHeightProp.Value);
                    LayoutRebuilder.MarkLayoutForRebuild(transform.parent as RectTransform);
                }
                else
                {
                    LayoutRebuilder.MarkLayoutForRebuild(transform.parent as RectTransform);
                    m_HasChanged = false;
                }
            }
        }

        protected override void OnDidApplyAnimationProperties()
        {
            m_HasChanged = true;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                if (transform.hasChanged)
                {
                    m_HasChanged = true;
                }
                CleanDirty();
            }
        }
#endif
    }



    public enum LayoutPropertyType
    {
        MinWidth,
        MinHeight,
        PreferredWidth,
        PreferredHeight,
        FlexibleWidth,
        FlexibleHeight
    }
}
