using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class PreviewAttribute : PropertyAttribute
{
    public float height = 50.0f;
    public float width = 50.0f;
    public int margin = 10;
}


[CustomPropertyDrawer(typeof(PreviewAttribute))]
public class PreviewAttributeDrawer : PropertyDrawer
{
	public override void OnGUI(Rect rect, SerializedProperty serializedProperty,
		GUIContent label)
	{
		EditorGUI.PropertyField(rect, serializedProperty, label);

		var previewAttribute = (PreviewAttribute)attribute;

		Texture2D texture = AssetPreview.GetAssetPreview(serializedProperty.objectReferenceValue);

		GUIStyle style = new GUIStyle(GUI.skin.label)
		{
			margin = new RectOffset(
				previewAttribute.margin,
				previewAttribute.margin,
				previewAttribute.margin,
				previewAttribute.margin)
		};

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(new GUIContent(texture), style,
			GUILayout.Width(previewAttribute.width),
			GUILayout.Height(previewAttribute.height)
			);
		GUILayout.EndHorizontal();
	}
}