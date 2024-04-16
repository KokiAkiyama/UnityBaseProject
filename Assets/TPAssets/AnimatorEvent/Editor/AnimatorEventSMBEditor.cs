using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Reflection;

namespace AnimatorEvents
{
	[CustomEditor(typeof(AnimatorEventSMB))]
	public class AnimatorEventSMBEditor : Editor
	{
//		private ReorderableList list_onStateEnterTransitionStart;
//		private ReorderableList list_onStateEnterTransitionEnd;
//		private ReorderableList list_onStateExitTransitionStart;
//		private ReorderableList list_onStateExitTransitionEnd;
		private ReorderableList list_onNormalizedTimeReached;
//		private ReorderableList list_onStateUpdated;

//		private readonly List<int> eventsAvailable = new List<int>();
//		private readonly List<AnimatorEvent> matchingAnimatorEvent = new List<AnimatorEvent>();

		private UnityEditor.Animations.AnimatorController controller;

		private static GUIStyle eventNameStyle;

		private void InitializeIfNeeded()
		{
			if (controller != null) return;

			if (eventNameStyle == null) {
				eventNameStyle = GUI.skin.label;
				eventNameStyle.richText = true;
			}

			Animator ignore;
			ScrubAnimatorUtil.GetCurrentAnimatorAndController(out controller, out ignore);

//			UpdateMatchingAnimatorEventList();

			/*
			CreateReorderableList("On State Enter Transition Start", 20, ref list_onStateEnterTransitionStart, serializedObject.FindProperty("onStateEnterTransitionStart"),
				(rect, index, isActive, isFocused) => {
					DrawCallbackField(rect, serializedObject.FindProperty("onStateEnterTransitionStart").GetArrayElementAtIndex(index));
				});
			CreateReorderableList("On State Enter Transition End", 20, ref list_onStateEnterTransitionEnd, serializedObject.FindProperty("onStateEnterTransitionEnd"),
				(rect, index, isActive, isFocused) => {
					DrawCallbackField(rect, serializedObject.FindProperty("onStateEnterTransitionEnd").GetArrayElementAtIndex(index));
				});
			CreateReorderableList("On State Exit Transition Start", 20, ref list_onStateExitTransitionStart, serializedObject.FindProperty("onStateExitTransitionStart"),
				(rect, index, isActive, isFocused) => {
					DrawCallbackField(rect, serializedObject.FindProperty("onStateExitTransitionStart").GetArrayElementAtIndex(index));
				});
			CreateReorderableList("On State Exit Transition End", 20, ref list_onStateExitTransitionEnd, serializedObject.FindProperty("onStateExitTransitionEnd"),
				(rect, index, isActive, isFocused) => {
					DrawCallbackField(rect, serializedObject.FindProperty("onStateExitTransitionEnd").GetArrayElementAtIndex(index));
				});
			CreateReorderableList("On State Update", 20, ref list_onStateUpdated, serializedObject.FindProperty("onStateUpdated"),
				(rect, index, isActive, isFocused) => {
					DrawCallbackField(rect, serializedObject.FindProperty("onStateUpdated").GetArrayElementAtIndex(index));
				});
			*/

			CreateReorderableList2("On Normalized Time Reached", 60, ref list_onNormalizedTimeReached, serializedObject.FindProperty("onNormalizedTimeReached"),
				(index, height) => {
					var property = serializedObject.FindProperty("onNormalizedTimeReached");//.GetArrayElementAtIndex(index);
					if (property == null || property.arraySize == 0) return height;

					property = property.GetArrayElementAtIndex(index);

					var argProp = property.FindPropertyRelative("_argParam");

					return height + EditorGUI.GetPropertyHeight(argProp);
				},
				(rect, index, isActive, isFocused) => {
					var property = serializedObject.FindProperty("onNormalizedTimeReached").GetArrayElementAtIndex(index);
					// 
					var argProp = property.FindPropertyRelative("_argParam");


					// コピー
					if (GUI.Button(new Rect(rect.x + rect.width - 80, rect.y, 40, 18), "Copy"))
					{
						var eventSMB = serializedObject.targetObject as AnimatorEventSMB;
						s_singleCopyJson = EditorJsonUtility.ToJson(eventSMB.onNormalizedTimeReached[index]);
					}
					// 貼り付け
					if (GUI.Button(new Rect(rect.x + rect.width - 40, rect.y, 40, 18), "Paste"))
					{
						if (string.IsNullOrEmpty(s_singleCopyJson) == false)
						{
							var eventSMB = serializedObject.targetObject as AnimatorEventSMB;

							object data = JsonUtility.FromJson<AnimatorEventSMB.TimedEvent>(s_singleCopyJson);
							EditorJsonUtility.FromJsonOverwrite(s_singleCopyJson, data);
							eventSMB.onNormalizedTimeReached[index] = (AnimatorEventSMB.TimedEvent)data;
						}
					}

					try
					{
						// クラス名とアセンブリ名に分割
						string[] strTypes = argProp.managedReferenceFullTypename.Split(" ");
						// 型情報取得
						System.Type t = System.Type.GetType($"{strTypes[1]}, {strTypes[0]}");

						GUI.backgroundColor = (Color)t.InvokeMember("GetEditorColor", BindingFlags.InvokeMethod, null, null, null);
					}
					catch (System.Exception)
					{
						GUI.backgroundColor = Color.white;
					}
					/*
					var colorProp = argProp.FindPropertyRelative("_color");
					if (colorProp != null)
					{
						GUI.backgroundColor = colorProp.colorValue;
					}
                    else
                    {
						GUI.backgroundColor = Color.white;
					}
					*/

					DrawCallbackField(rect, property);

					rect.y += 20;

					ScrubAnimatorUtil.DrawScrub(rect,
						(StateMachineBehaviour) target,
						property.FindPropertyRelative("normalizedTime"),
						property.FindPropertyRelative("repeat"),
						property.FindPropertyRelative("atLeastOnce"),
						property.FindPropertyRelative("neverWhileExit"));


					EditorGUI.PropertyField(new Rect(rect.x, rect.y + 40, rect.width, 20), argProp, true);

					GUI.backgroundColor = Color.white;
				});

		}

		/*
		void UpdateMatchingAnimatorEventList()
		{
			AnimatorEvent[] animatorEvents;
#if UNITY_2018_0 || UNITY_2018_1 || UNITY_2018_2 || UNITY_2017 || UNITY_5
		animatorEvents = FindObjectsOfType<AnimatorEvent>();
#else
			var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
			if (prefabStage != null)
			{
				animatorEvents = prefabStage.stageHandle.FindComponentsOfType<AnimatorEvent>();
			}
			else
			{
				animatorEvents = FindObjectsOfType<AnimatorEvent>();
			}
#endif

			matchingAnimatorEvent.Clear();
			foreach (var ae in animatorEvents)
			{
				var runtimeController = ae.GetComponent<Animator>().runtimeAnimatorController;
				var overrided = runtimeController as AnimatorOverrideController;
				if (runtimeController == controller || (overrided != null && overrided.runtimeAnimatorController == controller))
				{
					matchingAnimatorEvent.Add(ae);


					List<int> sortedEventIndices = new List<int>();
					for (int i = 0; i < ae.events.Length; i++) {
						sortedEventIndices.Add(i);
					}
					sortedEventIndices.Sort((a, b) => ae.events[a].name.CompareTo(ae.events[b].name));


					foreach (var i in sortedEventIndices) {
						var ev = ae.events[i];
						if (!eventsAvailable.Contains(ev.id)) {
							eventsAvailable.Add(ev.id);
						}
					}
				}
			}
		}
		*/

		private void DrawCallbackField(Rect rect, SerializedProperty property)
		{
			const float idWidth = 120;
			/*
			var callbackProperty = property.FindPropertyRelative("callback");
			var callbackIdProperty = property.FindPropertyRelative("callbackId");
			if (callbackIdProperty.intValue == 0)
			{
				callbackIdProperty.intValue = Animator.StringToHash(callbackProperty.stringValue);
			}
			if (matchingAnimatorEvent.Count == 0)
			{
				GUI.Label(new Rect(rect.x, rect.y, rect.width - idWidth, 18), "<color=red><b>No AnimatorEvent component found</b></color>", eventNameStyle);
				return;
			}
			*/

//			var ev = matchingAnimatorEvent[0].GetEventById(callbackIdProperty.intValue);
//			GUI.Label(new Rect(rect.x, rect.y, rect.width - idWidth, 18), ev != null ? ev.name : "<color=red><b>EVENT ID NOT FOUND</b></color>", eventNameStyle);
			GUI.Label(new Rect(rect.x, rect.y, rect.width - idWidth, 18), "AnimationEvent", eventNameStyle);

			/*
			// コピー
			if (GUI.Button(new Rect(rect.x + rect.width - 80, rect.y, 40, 18), "Copy"))
			{
				s_singleCopyJson = EditorJsonUtility.ToJson(property.objectReferenceValue);
			}
			// 貼り付け
			if (GUI.Button(new Rect(rect.x + rect.width - 40, rect.y, 40, 18), "Paste"))
			{
				if (string.IsNullOrEmpty(s_singleCopyJson) == false)
				{
					EditorJsonUtility.FromJsonOverwrite(s_singleCopyJson, property.serializedObject.targetObject);
				}
			}
			*/

			//			var prevLabelWidth = EditorGUIUtility.labelWidth;
			//			EditorGUIUtility.labelWidth = 20;
			//			EditorGUI.PropertyField(new Rect(rect.x + rect.width - idWidth, rect.y, idWidth, 18), callbackIdProperty, new GUIContent("ID ", "ID of the event. Change it manually only if needed."));
			//			EditorGUIUtility.labelWidth = prevLabelWidth;
		}

		/*
		private void CreateReorderableList(string title, int height, ref ReorderableList reorderableList, SerializedProperty soList, ReorderableList.ElementCallbackDelegate drawCallback)
		{
			reorderableList = new ReorderableList(serializedObject, soList, true, false, true, true);
			reorderableList.elementHeight = height;
			reorderableList.drawHeaderCallback = (rect) =>
			{
				GUI.Label(rect, title);
			};
			reorderableList.drawElementCallback = drawCallback;
			reorderableList.onAddDropdownCallback = (buttonRect, list) =>
			{
				if (matchingAnimatorEvent.Count == 0) return;

				var menu = new GenericMenu();
				for (int i = 0; i < eventsAvailable.Count; i++)
				{
					int j = i;
					menu.AddItem(new GUIContent(matchingAnimatorEvent[0].GetEventById(eventsAvailable[i]).name),
					false, (data) =>
					{
						serializedObject.Update();
						soList.InsertArrayElementAtIndex(soList.arraySize);
						soList.GetArrayElementAtIndex(soList.arraySize - 1).FindPropertyRelative("callbackId").intValue = eventsAvailable[j];
						serializedObject.ApplyModifiedProperties();
					}, eventsAvailable[i]);
				}
				menu.ShowAsContext();
			};
		}
		*/

		private void CreateReorderableList2(string title, int height, ref ReorderableList reorderableList, SerializedProperty soList,
			System.Func<int, int, float> heightCallback,
//			ReorderableList.ElementHeightCallbackDelegate heightCallback,
			ReorderableList.ElementCallbackDelegate drawCallback)
		{
			reorderableList = new ReorderableList(serializedObject, soList, true, false, true, true);

			reorderableList.elementHeight = height;

			reorderableList.elementHeightCallback = index =>
			{
				return heightCallback(index, height);
			};
			reorderableList.drawHeaderCallback = (rect) =>
			{
				GUI.Label(rect, title);
			};
			reorderableList.drawElementCallback = drawCallback;
			reorderableList.onAddDropdownCallback = (buttonRect, list) =>
			{
				serializedObject.Update();
				soList.InsertArrayElementAtIndex(soList.arraySize);
				//soList.GetArrayElementAtIndex(soList.arraySize - 1).FindPropertyRelative("callbackId").intValue = eventsAvailable[j];
				//						soList.GetArrayElementAtIndex(soList.arraySize - 1).FindPropertyRelative("repeat").boolValue = true;
				// SerializeReferenceはnullに
				soList.GetArrayElementAtIndex(soList.arraySize - 1).FindPropertyRelative("_argParam").managedReferenceValue = null;
//				soList.GetArrayElementAtIndex(soList.arraySize - 1).DuplicateCommand();
				serializedObject.ApplyModifiedProperties();

				/*
				if (matchingAnimatorEvent.Count == 0) return;

				var menu = new GenericMenu();
				for (int i = 0; i < eventsAvailable.Count; i++)
				{
					int j = i;
					menu.AddItem(new GUIContent(matchingAnimatorEvent[0].GetEventById(eventsAvailable[i]).name),
					false, (data) => {
						serializedObject.Update();
						soList.InsertArrayElementAtIndex(soList.arraySize);
						soList.GetArrayElementAtIndex(soList.arraySize - 1).FindPropertyRelative("callbackId").intValue = eventsAvailable[j];
//						soList.GetArrayElementAtIndex(soList.arraySize - 1).FindPropertyRelative("repeat").boolValue = true;
						serializedObject.ApplyModifiedProperties();
					}, eventsAvailable[i]);
				}
				menu.ShowAsContext();
				*/
			};
		}

		//		static SerializedProperty s_copy;

		static string s_allCopyJson;
		static string s_singleCopyJson;

		public override void OnInspectorGUI()
		{
			//DrawDefaultInspector();

			InitializeIfNeeded();

//			if (Application.isPlaying) GUI.enabled = false;

			serializedObject.Update();

			// コピー
			if (GUILayout.Button("Copy"))
			{
				string json = EditorJsonUtility.ToJson(serializedObject.targetObject);
//				EditorPrefs.SetString("AnimatorEventSMBEditor_Copy", json);
				s_allCopyJson = json;
			}
			// 貼り付け
			if (GUILayout.Button("Paste"))
			{
//				string strJson = EditorPrefs.GetString("AnimatorEventSMBEditor_Copy");
				if(string.IsNullOrEmpty(s_allCopyJson) == false)
                {
					EditorJsonUtility.FromJsonOverwrite(s_allCopyJson, serializedObject.targetObject);
				}
			}

//			GUILayout.Label("BlendTreeの場合は、そのIndex");
//			EditorGUILayout.PropertyField(serializedObject.FindProperty("_blendTreeIndex"));

//			list_onStateEnterTransitionStart.DoLayoutList();
//			list_onStateEnterTransitionEnd.DoLayoutList();
//			list_onStateExitTransitionStart.DoLayoutList();
//			list_onStateExitTransitionEnd.DoLayoutList();
//			list_onStateUpdated.DoLayoutList();
			list_onNormalizedTimeReached.DoLayoutList();

			serializedObject.ApplyModifiedProperties();

//			if (Application.isPlaying) GUI.enabled = true;
		}

		private void OnDestroy()
		{
			if (AnimationMode.InAnimationMode())
				AnimationMode.StopAnimationMode();
		}
	}
}
