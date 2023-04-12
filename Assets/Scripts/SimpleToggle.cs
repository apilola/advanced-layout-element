using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AP.UI;

public class SimpleToggle : MonoBehaviour
{
    [SerializeField] bool m_IsEnabled;
    [SerializeField] RectTransform m_ArrowTransform;
    [SerializeField] AdvancedLayoutElement m_AnswerElement;


    //Called by a the OnClick event of a Button component
    public void Toggle()
    {
        m_IsEnabled = !m_IsEnabled;
    }
    

    void Start()
    {
        //Setting up the GUI elements to match the default enabled state.
        m_AnswerElement[LayoutProperty.PreferredHeight].Weight = m_IsEnabled ? 1 : 0;
        var angles = m_ArrowTransform.eulerAngles;
        angles.z = m_IsEnabled ? 0 : 90;
        m_ArrowTransform.eulerAngles = angles;
    }
    
    // Probably wouldn't modulate these values in update in a real app.
    // Would probably use a tweening library
    // but in a demo this is fine
    void Update()
    {
        var heightProp = m_AnswerElement[LayoutProperty.PreferredHeight];
        heightProp.Weight = Mathf.MoveTowards(heightProp.Weight, m_IsEnabled ? 1 : 0, .1f);

        var angles = m_ArrowTransform.eulerAngles;
        angles.z = Mathf.MoveTowardsAngle(angles.z, m_IsEnabled ? 0 : 90, 5f);
        m_ArrowTransform.eulerAngles = angles;
    }
}
