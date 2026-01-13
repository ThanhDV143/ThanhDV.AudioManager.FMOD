using UnityEditor;
using UnityEngine;

namespace ThanhDV.AudioManager.FMOD
{
    public class AudioManagerEditorWindow : EditorWindow
    {
        [MenuItem("Tools/ThanhDV/Audio Manager/FMOD Manager", false, 0)]
        public static void ShowWindow()
        {
            AudioManagerEditorWindow window = GetWindow<AudioManagerEditorWindow>();
            window.titleContent = new GUIContent("Audio Manager");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }

        private void OnGUI()
        {
            string title = "AudioManager - FMOD";
            string subtitle = "Created by ThanhDV";
            EditorHelper.CreateHeader(title, subtitle);

            GUILayout.Space(10);

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty busesProp = serializedObject.FindProperty("_buses");
            SerializedProperty eventRefProp = serializedObject.FindProperty("_eventReferences");

            EditorGUILayout.PropertyField(busesProp, new GUIContent("Buses"));
            EditorGUILayout.PropertyField(eventRefProp, new GUIContent("Event References"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
