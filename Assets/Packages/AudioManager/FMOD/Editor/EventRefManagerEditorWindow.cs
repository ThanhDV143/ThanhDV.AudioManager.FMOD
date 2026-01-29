#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ThanhDV.AudioManager.FMOD
{
    public class EventRefManagerEditorWindow : EditorWindow
    {
        [SerializeField] private List<EventReferenceEntry> _eventReferences = new();
        [SerializeField] private string _searchText = "";

        private Vector2 _scroll;

        private SerializedObject _so;
        private SerializedProperty _eventReferencesProp;

        private bool _hasDataUnsaved = false;

        private FMODReferences _fMODReferences;
        private bool _isLoadingFMODReferences = false;

        [MenuItem(Common.MENU_ITEM + "EventReference Manager", false, 2)]
        public static void ShowWindow()
        {
            EventRefManagerEditorWindow window = GetWindow<EventRefManagerEditorWindow>();
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
            if (GUILayout.Button(new GUIContent("Refresh", "Reload eventReferences from data and refresh displayed data.")))
            {
                LoadEventReferences();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!_hasDataUnsaved);
            if (GUILayout.Button(new GUIContent("Clean & Save", "Removes eventReferences with empty keys or values, removes duplicates, and saves changes.")))
            {
                CleanEventReferences();
                SaveEventReferences();
                GenerateWrapper();
            }

            if (GUILayout.Button(new GUIContent("Discard Changes", "Discard all changes.")))
            {
                LoadEventReferences();
            }
            EditorGUI.EndDisabledGroup();

            EditorHelper.DrawHorizontalLine();

            DrawSearchToolbar();

            _so ??= new SerializedObject(this);
            _so.Update();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            bool deletedInFilteredView = false;
            string termBeforeDelete = null;

            if (string.IsNullOrWhiteSpace(_searchText))
            {
                EditorHelper.DrawListWithoutHeader(_eventReferencesProp, "EventReferences");
            }
            else
            {
                int pendingDeleteIndex = DrawFilteredEventReferences(_eventReferencesProp, _searchText);
                if (pendingDeleteIndex >= 0)
                {
                    termBeforeDelete = _searchText;
                    _eventReferencesProp.DeleteArrayElementAtIndex(pendingDeleteIndex);
                    deletedInFilteredView = true;
                }
            }

            EditorGUILayout.EndScrollView();

            bool changed = _so.ApplyModifiedProperties();
            if (changed)
            {
                _hasDataUnsaved = true;
            }

            if (deletedInFilteredView)
            {
                if (!HasAnyEventRefMatch(_eventReferences, termBeforeDelete))
                    ExitSearchMode();
                else
                    Repaint();
            }
        }

        private void DrawSearchToolbar()
        {
            GUIStyle searchFieldStyle = GUI.skin.FindStyle("ToolbarSearchTextField") ?? GUI.skin.textField;
            GUIStyle cancelButtonStyle = GUI.skin.FindStyle("ToolbarSearchCancelButton") ?? GUI.skin.button;

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                string newSearch = GUILayout.TextField(_searchText, searchFieldStyle, GUILayout.ExpandWidth(true));
                if (newSearch != _searchText)
                {
                    _searchText = newSearch;
                    Repaint();
                }

                if (GUILayout.Button(GUIContent.none, cancelButtonStyle))
                    ExitSearchMode();
            }
        }

        private void ExitSearchMode()
        {
            _searchText = string.Empty;
            EditorGUIUtility.editingTextField = false;
            GUIUtility.keyboardControl = 0;
            GUI.FocusControl(null);
            Repaint();
        }

        private static int DrawFilteredEventReferences(SerializedProperty eventRefsProp, string searchText)
        {
            if (eventRefsProp == null) return -1;

            string term = searchText?.Trim();
            if (string.IsNullOrEmpty(term))
            {
                EditorHelper.DrawListWithoutHeader(eventRefsProp, "EventReferences");
                return -1;
            }

            int total = eventRefsProp.arraySize;
            int shown = 0;

            EditorGUILayout.LabelField($"Search results: {term}  (showing matches)", EditorStyles.miniLabel);
            EditorGUILayout.Space(2);

            int pendingDeleteIndex = -1;

            for (int i = 0; i < total; i++)
            {
                SerializedProperty element = eventRefsProp.GetArrayElementAtIndex(i);

                SerializedProperty keyProp = element.FindPropertyRelative(nameof(EventReferenceEntry.Key));
                SerializedProperty eventRefProp = element.FindPropertyRelative(nameof(EventReferenceEntry.EventReference));

                string key = keyProp?.stringValue;
                string eventPath = eventRefProp?.FindPropertyRelative("Path")?.stringValue;

                if (!ContainsIgnoreCase(key, term) && !ContainsIgnoreCase(eventPath, term))
                    continue;

                shown++;
                EditorGUILayout.PropertyField(element, includeChildren: true);

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Delete", EditorStyles.miniButton, GUILayout.Width(70)))
                    {
                        pendingDeleteIndex = i;
                    }
                }

                EditorHelper.DrawHorizontalLine(thickness: 1, padding: 6);
            }

            if (shown == 0)
            {
                EditorGUILayout.HelpBox("No event references matched your search.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.LabelField($"Showing {shown}/{total}", EditorStyles.miniLabel);
            }

            return pendingDeleteIndex;
        }

        private static bool ContainsIgnoreCase(string source, string term)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(term)) return false;
            return source.IndexOf(term, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool HasAnyEventRefMatch(List<EventReferenceEntry> eventRefs, string searchText)
        {
            if (eventRefs == null) return false;

            string term = searchText?.Trim();
            if (string.IsNullOrEmpty(term)) return false;

            for (int i = 0; i < eventRefs.Count; i++)
            {
                EventReferenceEntry entry = eventRefs[i];

                string key = entry.Key;
                string eventPath = entry.EventReference.Path;

                if (ContainsIgnoreCase(key, term) || ContainsIgnoreCase(eventPath, term))
                    return true;
            }

            return false;
        }

        private async void LoadEventReferences()
        {
            if (_isLoadingFMODReferences) return;
            _isLoadingFMODReferences = true;
            _fMODReferences = await FMODReferencesEditorAsset.LoadOrCreateAsync(_fMODReferences);
            _eventReferences = new(_fMODReferences.GetEventReferences());
            _isLoadingFMODReferences = false;
        }

        private void CleanEventReferences()
        {
            HashSet<string> uniqueKeys = new();
            List<EventReferenceEntry> validEventReferences = new();

            foreach (EventReferenceEntry er in _eventReferences)
            {
                if (string.IsNullOrWhiteSpace(er.Key))
                {
                    DebugLog.Warning("Removed eventReference entry due to missing or empty key.");
                    continue;
                }

                if (er.EventReference.IsNull)
                {
                    DebugLog.Warning($"Removed eventReference entry '{er.Key}' due to missing or empty eventReference path.");
                    continue;
                }

                if (!uniqueKeys.Add(er.Key))
                {
                    DebugLog.Warning($"Removed duplicate eventReference entry with key '{er.Key}'.");
                    continue;
                }

                validEventReferences.Add(er);
            }

            _eventReferences = validEventReferences;
        }

        private void SaveEventReferences()
        {
            if (_fMODReferences != null)
            {
                _fMODReferences.SetEventReferences(_eventReferences);
                EditorUtility.SetDirty(_fMODReferences);
                AssetDatabase.SaveAssets();
            }

            _hasDataUnsaved = false;
        }

        private void GenerateWrapper()
        {
            WrapperGenerator.GenerateFMODEventReference(_fMODReferences.GetEventReferences());
        }
    }
}
#endif