using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AP.UI;

public class SimpleToggle : MonoBehaviour
{
    [SerializeField] bool m_IsEnabled;
    [SerializeField] RectTransform m_ArrowTransform;
    [SerializeField] AdvancedLayoutElement m_AnswerElement;


    public void Toggle()
    {
        m_IsEnabled = !m_IsEnabled;
    }

    void Start()
    {
        m_AnswerElement[LayoutPropertyType.PreferredHeight].Weight = m_IsEnabled ? 1 : 0;
        var angles = m_ArrowTransform.eulerAngles;
        angles.z = m_IsEnabled ? 0 : 90;
        m_ArrowTransform.eulerAngles = angles;
    }
    
    // Probably wouldn't modulate these values in update in a real app
    // but in a demo this is fine
    void Update()
    {
        var heightProp = m_AnswerElement[LayoutPropertyType.PreferredHeight];
        heightProp.Weight = Mathf.MoveTowards(heightProp.Weight, m_IsEnabled ? 1 : 0, .1f);

        var angles = m_ArrowTransform.eulerAngles;
        angles.z = Mathf.MoveTowardsAngle(angles.z, m_IsEnabled ? 0 : 90, 5f);
        m_ArrowTransform.eulerAngles = angles;
    }
}
