#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisplayOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        EditorGUI.EndProperty();
    }
}
#endif