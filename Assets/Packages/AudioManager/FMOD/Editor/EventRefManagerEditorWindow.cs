#if UNITY_EDITOR
using System.Collections.Generic;
using FMOD;
using FMODUnity;
using UnityEditor;
using UnityEngine;
using FMODStudio = global::FMOD.Studio;

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

        [MenuItem(Common.MENU_ITEM + "EventReference Manager", false, 3)]
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

            Color originalBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button(new GUIContent("Load All EventReferences", "Load all EventReference from the FMOD project. \nNote: this will delete all currently saved EventReferences.")))
            {
                GetAllEventReferences();
            }
            GUI.backgroundColor = originalBackgroundColor;

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

        private void GetAllEventReferences()
        {
            List<EventReferenceEntry> loadedEventReferences = new();
            FMODStudio.System system = CreateEditorSystem();

            if (!system.isValid())
            {
                DebugLog.Error("Failed to initialize the temporary FMOD system.");
                return;
            }

            try
            {
                string bankPath = Settings.Instance.SourceBankPath;
                string curPlatform = Settings.Instance.DefaultPlatform.BuildDirectory;
                string platformBankDirectory = System.IO.Path.Combine(bankPath, curPlatform);

                if (!System.IO.Directory.Exists(platformBankDirectory))
                {
                    DebugLog.Warning($"Bank directory not found at {platformBankDirectory}.");
                    return;
                }

                string[] allBankFiles = System.IO.Directory.GetFiles(platformBankDirectory, "*.bank", System.IO.SearchOption.TopDirectoryOnly);
                List<string> stringsBankPaths = new();
                List<string> contentBankPaths = new();

                for (int i = 0; i < allBankFiles.Length; i++)
                {
                    string bankFilePath = allBankFiles[i];
                    if (bankFilePath.EndsWith(".strings.bank", System.StringComparison.OrdinalIgnoreCase))
                    {
                        stringsBankPaths.Add(bankFilePath);
                    }
                    else
                    {
                        contentBankPaths.Add(bankFilePath);
                    }
                }

                if (contentBankPaths.Count == 0)
                {
                    DebugLog.Warning($"No content banks were found in {platformBankDirectory}.");
                    return;
                }

                List<FMODStudio.Bank> loadedStringsBanks = new();
                try
                {
                    for (int i = 0; i < stringsBankPaths.Count; i++)
                    {
                        string stringsBankPath = stringsBankPaths[i];
                        RESULT result = system.loadBankFile(stringsBankPath, FMODStudio.LOAD_BANK_FLAGS.NORMAL, out FMODStudio.Bank stringsBank);
                        if (result != RESULT.OK)
                        {
                            DebugLog.Warning($"Strings bank not found or failed to load at {stringsBankPath}. Result: {result}");
                            continue;
                        }

                        loadedStringsBanks.Add(stringsBank);
                    }

                    HashSet<string> seenPaths = new(System.StringComparer.OrdinalIgnoreCase);
                    HashSet<string> usedKeys = new(System.StringComparer.Ordinal);

                    for (int i = 0; i < contentBankPaths.Count; i++)
                    {
                        string contentBankPath = contentBankPaths[i];
                        RESULT result = system.loadBankFile(contentBankPath, FMODStudio.LOAD_BANK_FLAGS.NORMAL, out FMODStudio.Bank bank);
                        if (result != RESULT.OK)
                        {
                            DebugLog.Warning($"Bank not found or failed to load at {contentBankPath}. Result: {result}");
                            continue;
                        }

                        try
                        {
                            result = bank.getEventList(out FMODStudio.EventDescription[] eventDescriptions);
                            if (result != RESULT.OK)
                            {
                                DebugLog.Warning($"Failed to query event list from {contentBankPath}. Result: {result}");
                                continue;
                            }

                            for (int eventIndex = 0; eventIndex < eventDescriptions.Length; eventIndex++)
                            {
                                FMODStudio.EventDescription eventDescription = eventDescriptions[eventIndex];

                                result = eventDescription.getPath(out string path);
                                if (result != RESULT.OK || string.IsNullOrWhiteSpace(path) || !seenPaths.Add(path))
                                {
                                    continue;
                                }

                                result = eventDescription.getID(out global::FMOD.GUID guid);
                                if (result != RESULT.OK)
                                {
                                    DebugLog.Warning($"Failed to query event guid for '{path}'. Result: {result}");
                                    continue;
                                }

                                loadedEventReferences.Add(new EventReferenceEntry
                                {
                                    Key = CreateUniqueEventKey(path, usedKeys),
                                    EventReference = new EventReference
                                    {
                                        Path = path,
                                        Guid = guid,
                                    }
                                });
                            }
                        }
                        finally
                        {
                            bank.unload();
                        }
                    }
                }
                finally
                {
                    for (int i = 0; i < loadedStringsBanks.Count; i++)
                    {
                        loadedStringsBanks[i].unload();
                    }
                }

                _eventReferences = loadedEventReferences;
                _so = new SerializedObject(this);
                _eventReferencesProp = _so.FindProperty(nameof(_eventReferences));
                _hasDataUnsaved = loadedEventReferences.Count > 0;
                Repaint();

                if (loadedEventReferences.Count == 0)
                {
                    DebugLog.Warning("Load All EventReferences completed, but no valid event references were collected.");
                }
            }
            finally
            {
                system.release();
            }
        }

        private static string CreateUniqueEventKey(string eventPath, HashSet<string> usedKeys)
        {
            string rawKey = eventPath;
            if (rawKey.StartsWith("event:/", System.StringComparison.OrdinalIgnoreCase))
            {
                rawKey = rawKey.Substring("event:/".Length);
            }

            rawKey = rawKey.Trim('/');
            if (string.IsNullOrWhiteSpace(rawKey)) rawKey = "Event";

            System.Text.StringBuilder builder = new(rawKey.Length);
            for (int i = 0; i < rawKey.Length; i++)
            {
                char character = rawKey[i];
                builder.Append(char.IsLetterOrDigit(character) || character == '_' ? character : '_');
            }

            string key = builder.Length == 0 ? "Event" : builder.ToString();
            if (!char.IsLetter(key[0]) && key[0] != '_')
            {
                key = "_" + key;
            }

            string uniqueKey = key;
            int suffix = 2;
            while (!usedKeys.Add(uniqueKey))
            {
                uniqueKey = key + "_" + suffix;
                suffix++;
            }

            return uniqueKey;
        }

        private static FMODStudio.System CreateEditorSystem()
        {
            RESULT result = FMODStudio.System.create(out FMODStudio.System system);
            if (result != RESULT.OK) return new FMODStudio.System(System.IntPtr.Zero);

            FMODStudio.INITFLAGS flags = FMODStudio.INITFLAGS.ALLOW_MISSING_PLUGINS | FMODStudio.INITFLAGS.SYNCHRONOUS_UPDATE;

            system.initialize(1, flags, INITFLAGS.MIX_FROM_UPDATE, System.IntPtr.Zero);

            return system;
        }
    }
}
#endif