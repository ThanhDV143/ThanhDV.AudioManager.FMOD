#if UNITY_EDITOR
using System.Collections.Generic;
using FMOD;
using FMODUnity;
using UnityEditor;
using UnityEngine;
using FMODStudio = global::FMOD.Studio;

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

        [MenuItem(Common.MENU_ITEM + "Bus Manager", false, 2)]
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

            Color originalBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button(new GUIContent("Load All Buses", "Load all buses from the FMOD project. \nNote: this will delete all currently saved buses.")))
            {
                GetAllBus();
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

        private void GetAllBus()
        {
            List<string> busPaths = new();
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
                string masterBankPath = System.IO.Path.Combine(bankPath, curPlatform, "Master.bank");
                string stringsBankPath = System.IO.Path.Combine(bankPath, curPlatform, "Master.strings.bank");

                RESULT result = system.loadBankFile(stringsBankPath, FMODStudio.LOAD_BANK_FLAGS.NORMAL, out FMODStudio.Bank stringsBank);

                if (result != RESULT.OK)
                {
                    DebugLog.Warning($"Strings bank not found or failed to load at {stringsBankPath}. Result: {result}");
                    return;
                }

                try
                {
                    result = system.loadBankFile(masterBankPath, FMODStudio.LOAD_BANK_FLAGS.NORMAL, out FMODStudio.Bank masterBank);
                    if (result != RESULT.OK)
                    {
                        DebugLog.Warning($"Master bank not found or failed to load at {masterBankPath}. Result: {result}");
                        return;
                    }

                    try
                    {
                        result = masterBank.getBusCount(out int busCount);
                        if (result != RESULT.OK)
                        {
                            DebugLog.Warning($"Failed to query bus count from {masterBankPath}. Result: {result}");
                            return;
                        }

                        if (busCount <= 0)
                        {
                            DebugLog.Warning($"No buses were found in {masterBankPath}. If you only loaded Master.strings.bank then this is expected, because bus definitions live in Master.bank.");
                            return;
                        }

                        result = masterBank.getBusList(out FMODStudio.Bus[] buses);
                        if (result != RESULT.OK)
                        {
                            DebugLog.Warning($"Failed to query bus list from {masterBankPath}. Result: {result}");
                            return;
                        }

                        foreach (FMODStudio.Bus bus in buses)
                        {
                            result = bus.getPath(out string path);
                            if (result != RESULT.OK) continue;
                            if (!string.IsNullOrWhiteSpace(path))
                            {
                                busPaths.Add(path);
                            }
                        }
                    }
                    finally
                    {
                        masterBank.unload();
                    }
                }
                finally
                {
                    stringsBank.unload();
                }

                _buses = BuildBusEntries(busPaths);
                _so = new SerializedObject(this);
                _busesProp = _so.FindProperty(nameof(_buses));
                _hasDataUnsaved = busPaths.Count > 0;
                Repaint();

                if (busPaths.Count == 0)
                {
                    DebugLog.Warning("Load All Buses completed, but no valid bus paths were collected.");
                }
            }
            finally
            {
                system.release();
            }
        }

        private static List<BusEntry> BuildBusEntries(List<string> busPaths)
        {
            List<BusEntry> busEntries = new();
            HashSet<string> seenPaths = new(System.StringComparer.Ordinal);
            HashSet<string> usedKeys = new(System.StringComparer.Ordinal);

            foreach (string busPath in busPaths)
            {
                if (string.IsNullOrWhiteSpace(busPath) || !seenPaths.Add(busPath)) continue;

                busEntries.Add(new BusEntry
                {
                    Key = CreateUniqueBusKey(busPath, usedKeys),
                    BusPath = busPath,
                });
            }

            return busEntries;
        }

        private static string CreateUniqueBusKey(string busPath, HashSet<string> usedKeys)
        {
            string rawKey = busPath;
            if (rawKey.StartsWith("bus:/", System.StringComparison.OrdinalIgnoreCase))
            {
                rawKey = rawKey.Substring("bus:/".Length);
            }

            rawKey = rawKey.Trim('/');
            if (string.IsNullOrWhiteSpace(rawKey)) rawKey = "Master";

            System.Text.StringBuilder builder = new(rawKey.Length);
            for (int i = 0; i < rawKey.Length; i++)
            {
                char character = rawKey[i];
                builder.Append(char.IsLetterOrDigit(character) || character == '_' ? character : '_');
            }

            string key = builder.Length == 0 ? "Master" : builder.ToString();
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
            ;
            if (result != RESULT.OK) return new FMODStudio.System(System.IntPtr.Zero);

            FMODStudio.INITFLAGS flags = FMODStudio.INITFLAGS.ALLOW_MISSING_PLUGINS | FMODStudio.INITFLAGS.SYNCHRONOUS_UPDATE;

            system.initialize(1, flags, INITFLAGS.MIX_FROM_UPDATE, System.IntPtr.Zero);

            return system;
        }
    }
}
#endif
