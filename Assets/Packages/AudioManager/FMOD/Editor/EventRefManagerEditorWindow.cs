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

        private FMODReferences _fMODReferences;
        private bool _isLoadingFMODReferences = false;

        [MenuItem(Common.MENU_ITEM + "EventReference Manager", false, 2)]
        public static void ShowWindow()
        {
            EventRefEditorWindow window = GetWindow<EventRefEditorWindow>();
            window.titleContent = new GUIContent("EventReference Manager");
            window.minSize = new Vector2(500, 300);
            window.Show();
        }

        private void OnEnable()
        {
            _hasDataUnsaved = false;
            _so = new SerializedObject(this);
            _eventReferencesProp = _so.FindProperty(nameof(_eventReferences));

            LoadEventReferences();
        }

        private void OnGUI()
        {
            string title = "AudioManager - FMOD - EventReference";
            string subtitle = "Created by ThanhDV";
            EditorHelper.CreateHeader(title, subtitle);

            if (!FMODReferencesEditorAsset.DrawEnsureFMODReferencesUI(_isLoadingFMODReferences, _fMODReferences, LoadEventReferences)) return;

            EditorGUI.BeginDisabledGroup(_hasDataUnsaved);
            if (GUILayout.Button(new GUIContent("Refresh", "Reload buses from data and refresh displayed data.")))
            {
                LoadEventReferences();
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

            if (_so.ApplyModifiedProperties())
            {
                _hasDataUnsaved = true;
            }
        }

        private async void LoadEventReferences()
        {
            if (_isLoadingFMODReferences) return;
            _isLoadingFMODReferences = true;
            _fMODReferences = await FMODReferencesEditorAsset.LoadOrCreateAsync(_fMODReferences);
            _eventReferences = new(_fMODReferences.EventReferences);
            _isLoadingFMODReferences = false;
        }
    }
}
