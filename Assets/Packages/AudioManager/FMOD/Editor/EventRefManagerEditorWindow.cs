using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ThanhDV.AudioManager.FMOD
{
    public class EventRefEditorWindow : EditorWindow
    {
        [SerializeField] private List<EventReferenceEntry> _eventReferences = new();

        private Vector2 _scroll;

        private SerializedObject _so;
        private SerializedProperty _eventReferencesProp;

        private bool _hasDataUnsaved = false;

        [MenuItem("Tools/ThanhDV/Audio Manager/EventReference Manager", false, 1)]
        public static void ShowWindow()
        {
            EventRefEditorWindow window = GetWindow<EventRefEditorWindow>();
            window.titleContent = new GUIContent("EventReference Manager");
            window.minSize = new Vector2(500, 300);
            window.Show();
        }

        private void OnEnable()
        {
            _so = new SerializedObject(this);
            _eventReferencesProp = _so.FindProperty(nameof(_eventReferences));
        }

        private void OnGUI()
        {
            string title = "AudioManager - FMOD - EventReference";
            string subtitle = "Created by ThanhDV";
            EditorHelper.CreateHeader(title, subtitle);

            EditorGUI.BeginDisabledGroup(_hasDataUnsaved);
            if (GUILayout.Button(new GUIContent("Refresh", "Reload buses from data and refresh displayed data.")))
            {
                // Refresh()
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!_hasDataUnsaved);
            if (GUILayout.Button(new GUIContent("Save", "Save changes")))
            {
                // Save()
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!_hasDataUnsaved);
            if (GUILayout.Button(new GUIContent("Clean", "Remove buses with an empty key or value, and remove duplicate keys.")))
            {
                // Clean()
            }
            EditorGUI.EndDisabledGroup();

            EditorHelper.DrawHorizontalLine();

            _so ??= new SerializedObject(this);
            _so.Update();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            EditorHelper.DrawListWithoutHeader(_eventReferencesProp, "EventReferences");

            EditorGUILayout.EndScrollView();

            _so.ApplyModifiedProperties();
        }
    }
}
