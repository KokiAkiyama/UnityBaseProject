using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(DictionaryEx<,>),true)]
public class DictionaryExEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty fieldProp = property.FindPropertyRelative("_list");
        EditorGUI.PropertyField(position, fieldProp, label, true);
        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //_listÇÃçÇÇ≥ÇéZèo
        SerializedProperty fieldProp = property.FindPropertyRelative("_list");
        float height = 0f;
        if(fieldProp==null)
        {
            height = EditorGUIUtility.singleLineHeight;
        }
        else
        {
            height = EditorGUI.GetPropertyHeight(fieldProp, true);
        }
        return height;
    }
}
