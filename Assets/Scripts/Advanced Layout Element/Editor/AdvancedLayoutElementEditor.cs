using UnityEditor;
using UnityEngine;
using AP.UI;
using UnityEngine.UI;
namespace AP.Editor.UI
{
    using Editor = UnityEditor.Editor;
    [CustomEditor(typeof(AP.UI.AdvancedLayoutElement))]
    public class AdvancedLayoutElementEditor : Editor
    {
        SerializedProperty ignoreLayout;
        SerializedProperty layoutIndependent;
        SerializedProperty minWidth;
        SerializedProperty minHeight;
        SerializedProperty preferredWidth;
        SerializedProperty preferredHeight;
        SerializedProperty flexibleWidth;
        SerializedProperty flexibleHeight;
        SerializedProperty layoutPriority;
        SerializedProperty overridePriority;

        private void OnEnable()
        {
            ignoreLayout = serializedObject.FindProperty("m_IgnoreLayout");
            layoutIndependent = serializedObject.FindProperty("m_LayoutIndependent");
            minWidth = serializedObject.FindProperty("m_MinWidthProp");
            minHeight = serializedObject.FindProperty("m_MinHeightProp");
            preferredWidth = serializedObject.FindProperty("m_PreferredWidthProp");
            preferredHeight = serializedObject.FindProperty("m_PreferredHeightProp");
            flexibleWidth = serializedObject.FindProperty("m_FlexibleWidthProp");
            flexibleHeight = serializedObject.FindProperty("m_FlexibleHeightProp");
            overridePriority = serializedObject.FindProperty("m_OverridePriority");
            layoutPriority = serializedObject.FindProperty("m_LayoutPriority");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(ignoreLayout);
            EditorGUI.BeginDisabledGroup(!ignoreLayout.boolValue);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(layoutIndependent);
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(minWidth, new GUIContent("Min Width"));
            EditorGUILayout.PropertyField(minHeight, new GUIContent("Min Height"));
            EditorGUILayout.PropertyField(preferredWidth, new GUIContent("Preferred Width"));
            EditorGUILayout.PropertyField(preferredHeight, new GUIContent("Preferred Height"));
            EditorGUILayout.PropertyField(flexibleWidth, new GUIContent("Flexible Width"));
            EditorGUILayout.PropertyField(flexibleHeight, new GUIContent("Flexible Height"));
            EditorGUILayout.PropertyField(overridePriority, new GUIContent("Override Priority"));
            if(overridePriority.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(layoutPriority, new GUIContent("Layout Priority"));
                EditorGUI.indentLevel--;
            }
            serializedObject.ApplyModifiedProperties();
        }

    }

    [CustomPropertyDrawer(typeof(AdvancedLayoutElement.Property))]
    public class LayoutPropertyDrawer : PropertyDrawer
    {
        public static class Style
        {
            public static readonly float EnabledWidth = 20;
            public static readonly float FoldOutWidth = 20;
            public static readonly GUIContent ModeContent = new GUIContent(EditorGUIUtility.FindTexture("_Popup@2x"));

            static readonly GUIContent DummyContent = new GUIContent(" ");

            public static Vector2 s_HeaderSize = Vector2.zero;

            public static readonly float HeaderHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 4;


            public static Vector2 HeaderSize
            {
                get
                {
                    if (s_HeaderSize == Vector2.zero)
                    {
                        s_HeaderSize = GUI.skin.window.CalcSize(DummyContent);
                    }
                    return s_HeaderSize;
                }
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            var element = property.serializedObject.targetObject as AdvancedLayoutElement;
            var defaultType = (LayoutProperty)property.FindPropertyRelative("m_DefaultType").enumValueIndex;

            var enabledProp = property.FindPropertyRelative("m_Enabled");
            var readOnlyProp = property.FindPropertyRelative("m_ReadOnly");
            var typeProp = property.FindPropertyRelative("m_Type");
            var valueProp = property.FindPropertyRelative("m_Value");
            var weightProp = property.FindPropertyRelative("m_Weight");
            var overrideProp = property.FindPropertyRelative("m_Override");

            string correctionFieldName = AdvancedLayoutElement.Property.CorrectionFieldName;
            var correctionProperty = property.FindPropertyRelative(correctionFieldName);

            var currentType = (LayoutProperty)typeProp.enumValueIndex;

            Rect marchingRect = position;
            marchingRect.height = Style.HeaderHeight;

            Rect enabledRect = marchingRect;
            enabledRect.width = Style.EnabledWidth;
            enabledProp.boolValue = EditorGUI.ToggleLeft(enabledRect, GUIContent.none, enabledProp.boolValue);

            Rect windowRect = marchingRect;
            windowRect.x += Style.EnabledWidth;
            windowRect.width -= Style.EnabledWidth;
            GUI.Box(windowRect, "", GUI.skin.window);


            Rect valueRect = windowRect;
            valueRect.y += EditorGUIUtility.standardVerticalSpacing * 2;
            valueRect.height = EditorGUIUtility.singleLineHeight;
            valueRect.x += EditorGUIUtility.standardVerticalSpacing * 2 + Style.FoldOutWidth;
            valueRect.width -= EditorGUIUtility.standardVerticalSpacing * 4 + Style.FoldOutWidth;
            //if (currentType != defaultType || overrideProp.objectReferenceValue == null || !enabledProp.boolValue)

            GUI.Label(valueRect, label);
            var fieldRect = EditorGUI.PrefixLabel(valueRect, new GUIContent(" "));
            EditorGUI.BeginDisabledGroup(false);
            if (element[defaultType].Override == null && enabledProp.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                var val = EditorGUI.DelayedFloatField(fieldRect, element[defaultType].RawValue);
                if(EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(element, "float fieldChanged");
                    element[defaultType].RawValue = val;
                }
            }
            else
            {
                EditorGUI.FloatField(fieldRect, element[defaultType].Value);
            }
            EditorGUI.EndDisabledGroup();

            Rect foldOutRect = windowRect;
            foldOutRect.y += EditorGUIUtility.standardVerticalSpacing * 2;
            foldOutRect.height = EditorGUIUtility.singleLineHeight;
            foldOutRect.x += EditorGUIUtility.standardVerticalSpacing * 2;
            foldOutRect.width -= EditorGUIUtility.standardVerticalSpacing * 4;
            var foldOutContent = new GUIContent((property.isExpanded) ? "\u2296" : "\u2295");
            property.isExpanded = GUI.Toggle(foldOutRect, property.isExpanded, foldOutContent, EditorStyles.label);

            if (property.isExpanded)
            {
                marchingRect.y += marchingRect.height + EditorGUIUtility.standardVerticalSpacing;
                marchingRect.height = EditorGUIUtility.singleLineHeight;
                marchingRect.x += Style.EnabledWidth;
                marchingRect.width -= Style.EnabledWidth;
                marchingRect.x += EditorGUIUtility.standardVerticalSpacing * 2 + Style.FoldOutWidth;
                marchingRect.width -= EditorGUIUtility.standardVerticalSpacing * 4 + Style.FoldOutWidth;

                DoProperty(typeProp, ref marchingRect);
                DoProperty(weightProp, ref marchingRect);
                DoOverrideProperty(overrideProp, ref marchingRect);
                DoProperty(valueProp, ref marchingRect);
                DoProperty(readOnlyProp, ref marchingRect);
                DoProperty(correctionProperty, ref marchingRect);
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = Style.HeaderHeight;
            if (property.isExpanded)
            {
                height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 6;
            }
            return height;
        }

        void DoProperty(SerializedProperty property, ref Rect rect)
        {
            var label = new GUIContent(property.displayName);
            label = EditorGUI.BeginProperty(rect, label, property);
            EditorGUI.PropertyField(rect, property, label);
            EditorGUI.EndProperty();
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

        }

        void DoOverrideProperty(SerializedProperty property, ref Rect rect)
        {
            float height = rect.height + EditorGUIUtility.standardVerticalSpacing;
            var label = new GUIContent(property.displayName);
            label = EditorGUI.BeginProperty(rect, label, property);
            var current = property.objectReferenceValue;
            if (current == null || (DragAndDrop.objectReferences.Length > 0))
            {
                EditorGUI.PropertyField(rect, property, label);
            }
            else
            {
                var srcRect = EditorGUI.PrefixLabel(rect, label);
                var dropdownRect = srcRect;
                dropdownRect.width -= 18 * 2;
                var pingRect = srcRect;
                pingRect.width = 18;
                pingRect.x += dropdownRect.width;
                var removeRect = pingRect;
                removeRect.width = 18;
                removeRect.x += pingRect.width;
                if (EditorGUI.DropdownButton(dropdownRect, GetContentForObj(current), FocusType.Passive, EditorStyles.miniPullDown))
                {
                    GenericMenu menu = new GenericMenu();
                    switch (current)
                    {
                        case GameObject gameObject:
                            FillMenu(menu, gameObject);
                            break;
                        case Component component:
                            FillMenu(menu, component.gameObject);
                            break;
                        case ILayoutElement layoutElement:
                            break;
                        default:
                            break;
                    }
                    menu.DropDown(dropdownRect);
                }
                if (GUI.Button(pingRect, "\u2299", EditorStyles.miniButtonMid))
                {
                    EditorGUIUtility.PingObject(current);
                }
                var o = GUI.contentColor;
                GUI.contentColor = Color.red * Color.gray;
                if (GUI.Button(removeRect, new GUIContent("X"), EditorStyles.miniButtonRight))
                {
                    property.objectReferenceValue = null;
                }
                GUI.contentColor = o;
            }
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.EndProperty();
            void FillMenu(GenericMenu menu, GameObject src)
            {
                var components = src.GetComponents<ILayoutElement>();
                var rectTransform = src.transform as RectTransform;

                if (rectTransform)
                {
                    AddItem(rectTransform);
                }

                foreach (var item in components)
                {
                    AddItem((Component)item);
                }

                void AddItem(Component obj)
                {
                    if (obj == property.serializedObject.targetObject)
                        return;

                    var isOn = Object.ReferenceEquals(current, obj);
                    menu.AddItem(GetDropdownContentForObj(obj), isOn, () =>
                    {
                        if (isOn)
                            return;
                        property.objectReferenceValue = obj;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
            }

            GUIContent GetContentForObj(UnityEngine.Object obj)
            {
                switch (current)
                {
                    case GameObject gameObject:
                        return new GUIContent($"{gameObject.name}  (GameObject)");
                    case Component component:
                        return new GUIContent($"{component.gameObject.name}  ({obj.GetType().Name})");
                    case ILayoutElement layoutElement:
                        return new GUIContent($"{obj.name}  ({obj.GetType().Name})");
                    default:
                        throw new System.ArgumentOutOfRangeException(nameof(obj));
                }
            }

            GUIContent GetDropdownContentForObj(Component obj)
            {
                return new GUIContent($"{obj.GetType().Name}");
            }

        }
    }
}