using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AP.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public partial class AdvancedLayoutElement : UIBehaviour, ILayoutElement, ILayoutIgnorer
    {
        /// <summary>
        /// Gets the property with the specified type
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Property this[LayoutProperty property]
        {
            get
            {
                switch (property)
                {
                    case LayoutProperty.MinWidth:
                        return m_MinWidthProp;
                    case LayoutProperty.MinHeight:
                        return m_MinHeightProp;
                    case LayoutProperty.PreferredWidth:
                        return m_PreferredWidthProp;
                    case LayoutProperty.PreferredHeight:
                        return m_PreferredHeightProp;
                    case LayoutProperty.FlexibleWidth:
                        return m_FlexibleWidthProp;
                    case LayoutProperty.FlexibleHeight:
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
        [SerializeField] Property m_MinWidthProp = new(LayoutProperty.MinWidth);
        [Tooltip("Controls the minimum height for the ILayoutElement interface")]
        [SerializeField] Property m_MinHeightProp = new(LayoutProperty.MinHeight);
        [Tooltip("Controls the preffered width for the ILayoutElement interface")]
        [SerializeField] Property m_PreferredWidthProp = new(LayoutProperty.PreferredWidth);
        [Tooltip("Controls the preffered height for the ILayoutElement interface")]
        [SerializeField] Property m_PreferredHeightProp = new(LayoutProperty.PreferredHeight);
        [Tooltip("Controls the flexible width for the ILayoutElement interface")]
        [SerializeField] Property m_FlexibleWidthProp = new(LayoutProperty.FlexibleWidth);
        [Tooltip("Controls the flexibile for the ILayoutElement interface")]
        [SerializeField] Property m_FlexibleHeightProp = new(LayoutProperty.FlexibleHeight);
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
            m_MinWidthProp.Validate(this, LayoutProperty.MinWidth);
            m_MinHeightProp.Validate(this, LayoutProperty.MinHeight);
            m_PreferredWidthProp.Validate(this, LayoutProperty.PreferredWidth);
            m_PreferredHeightProp.Validate(this, LayoutProperty.PreferredHeight);
            m_FlexibleWidthProp.Validate(this, LayoutProperty.FlexibleWidth);
            m_FlexibleHeightProp.Validate(this, LayoutProperty.FlexibleHeight);
            if(transform.hasChanged)
            {
                m_HasChanged = true;
            }
        }
#endif

        protected override void Awake()
        {
            // Set properties with references to this component and the property type
            m_MinWidthProp.Validate(this, LayoutProperty.MinWidth);
            m_MinHeightProp.Validate(this, LayoutProperty.MinHeight);
            m_PreferredWidthProp.Validate(this, LayoutProperty.PreferredWidth);
            m_PreferredHeightProp.Validate(this, LayoutProperty.PreferredHeight);
            m_FlexibleWidthProp.Validate(this, LayoutProperty.FlexibleWidth);
            m_FlexibleHeightProp.Validate(this, LayoutProperty.FlexibleHeight);

            if (transform.hasChanged)
            {
                m_HasChanged = true;
            }
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

        //Called by Unity's layout rebuilder when the horizontal layout needs to be recalculated
        public void CalculateLayoutInputHorizontal()
        {
            m_MinWidthProp.CalculateLayout();
            m_PreferredWidthProp.CalculateLayout();
            m_FlexibleWidthProp.CalculateLayout();
        }

        //Called by Unity's layout rebuilder when the vertical layout needs to be recalculated
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

        //If the layout has changed, mark the parent for a layout rebuild or rebuild the layout if this component is layout independent
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

        //Called by Unity's animation system when the animation properties have been changed
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



    public enum LayoutProperty
    {
        MinWidth,
        MinHeight,
        PreferredWidth,
        PreferredHeight,
        FlexibleWidth,
        FlexibleHeight
    }
}
