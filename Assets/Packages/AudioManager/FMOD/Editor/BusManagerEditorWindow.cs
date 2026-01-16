using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ThanhDV.AudioManager.FMOD
{
    public class BusManagerEditorWindow : EditorWindow
    {
        [SerializeField] private List<BusEntry> _buses = new();
        [SerializeField] private string _searchText = "";

        private Vector2 _scroll;

        private SerializedObject _so;
        private SerializedProperty _busesProp;

        private bool _hasDataUnsaved = false;

        private FMODReferences _fMODReferences;
        private bool _isLoadingFMODReferences = false;

        [MenuItem(Common.MENU_ITEM + "Bus Manager", false, 1)]
        public static void ShowWindow()
        {
            BusManagerEditorWindow window = GetWindow<BusManagerEditorWindow>();
            window.titleContent = new GUIContent("Bus Manager");
            window.minSize = new Vector2(500, 300);
            window.Show();
        }

        private void OnEnable()
        {
            _hasDataUnsaved = false;
            _so = new SerializedObject(this);
            _busesProp = _so.FindProperty(nameof(_buses));
            LoadBuses();
        }

        private void OnGUI()
        {
            string title = "AudioManager - FMOD - Bus";
            string subtitle = "Created by ThanhDV";
            EditorHelper.CreateHeader(title, subtitle);

            if (!FMODReferencesEditorAsset.DrawEnsureFMODReferencesUI(_isLoadingFMODReferences, _fMODReferences, LoadBuses)) return;

            EditorGUI.BeginDisabledGroup(_hasDataUnsaved);
            if (GUILayout.Button(new GUIContent("Refresh", "Reload buses from data and refresh displayed data.")))
            {
                LoadBuses();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!_hasDataUnsaved);
            if (GUILayout.Button(new GUIContent("Clean & Save", "Removes buses with empty keys or values, removes duplicates, and saves changes.")))
            {
                CleanBuses();
                SaveBuses();
                GenerateWrapper();
            }

            if (GUILayout.Button(new GUIContent("Discard Changes", "Discard all changes.")))
            {
                LoadBuses();
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
                EditorHelper.DrawListWithoutHeader(_busesProp, "Buses");
            }
            else
            {
                int pendingDeleteIndex = DrawFilteredBuses(_busesProp, _searchText);
                if (pendingDeleteIndex >= 0)
                {
                    termBeforeDelete = _searchText;
                    _busesProp.DeleteArrayElementAtIndex(pendingDeleteIndex);
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
                if (!HasAnyBusMatch(_buses, termBeforeDelete))
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

        private static int DrawFilteredBuses(SerializedProperty busesProp, string searchText)
        {
            if (busesProp == null) return -1;

            string term = searchText?.Trim();
            if (string.IsNullOrEmpty(term))
            {
                EditorHelper.DrawListWithoutHeader(busesProp, "Buses");
                return -1;
            }

            int total = busesProp.arraySize;
            int shown = 0;

            EditorGUILayout.LabelField($"Search results: {term}  (showing matches)", EditorStyles.miniLabel);
            EditorGUILayout.Space(2);

            int pendingDeleteIndex = -1;

            for (int i = 0; i < total; i++)
            {
                SerializedProperty element = busesProp.GetArrayElementAtIndex(i);

                SerializedProperty keyProp = element.FindPropertyRelative(nameof(BusEntry.Key));
                SerializedProperty busPathProp = element.FindPropertyRelative(nameof(BusEntry.BusPath));

                string key = keyProp?.stringValue;
                string busPath = busPathProp?.stringValue;

                if (!ContainsIgnoreCase(key, term) && !ContainsIgnoreCase(busPath, term))
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
                EditorGUILayout.HelpBox("No buses matched your search.", MessageType.Info);
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

        private static bool HasAnyBusMatch(List<BusEntry> buses, string searchText)
        {
            if (buses == null) return false;

            string term = searchText?.Trim();
            if (string.IsNullOrEmpty(term)) return false;

            for (int i = 0; i < buses.Count; i++)
            {
                BusEntry bus = buses[i];
                if (bus == null) continue;

                string key = bus.Key;
                string busPath = bus.BusPath;

                if (ContainsIgnoreCase(key, term) || ContainsIgnoreCase(busPath, term))
                    return true;
            }

            return false;
        }

        private async void LoadBuses()
        {
            if (_isLoadingFMODReferences) return;
            _isLoadingFMODReferences = true;
            _fMODReferences = await FMODReferencesEditorAsset.LoadOrCreateAsync(_fMODReferences);
            _buses = new(_fMODReferences.GetBuses());
            _isLoadingFMODReferences = false;
        }

        private void CleanBuses()
        {
            HashSet<string> uniqueKeys = new();
            List<BusEntry> validBuses = new();

            foreach (BusEntry bus in _buses)
            {
                if (string.IsNullOrWhiteSpace(bus.Key))
                {
                    DebugLog.Warning("Removed bus entry due to missing or empty key.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(bus.BusPath))
                {
                    DebugLog.Warning($"Removed bus entry '{bus.Key}' due to missing or empty bus path.");
                    continue;
                }

                if (!uniqueKeys.Add(bus.Key))
                {
                    DebugLog.Warning($"Removed duplicate bus entry with key '{bus.Key}'.");
                    continue;
                }

                validBuses.Add(bus);
            }

            _buses = validBuses;
        }

        private void SaveBuses()
        {
            if (_fMODReferences != null)
            {
                _fMODReferences.SetBuses(_buses);
                EditorUtility.SetDirty(_fMODReferences);
                AssetDatabase.SaveAssets();
            }

            _hasDataUnsaved = false;
        }

        private void GenerateWrapper()
        {
            WrapperGenerator.GenerateFMODBus(_fMODReferences.GetBuses());
        }
    }
}
