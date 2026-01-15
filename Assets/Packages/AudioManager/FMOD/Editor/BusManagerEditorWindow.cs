using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ThanhDV.AudioManager.FMOD
{
    public class BusManagerEditorWindow : EditorWindow
    {
        [SerializeField] private List<BusEntry> _buses = new();

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

            EditorHelper.DrawListWithoutHeader(_busesProp, "Buses");

            EditorGUILayout.EndScrollView();

            if (_so.ApplyModifiedProperties())
            {
                _hasDataUnsaved = true;
            }
        }

        private async void LoadBuses()
        {
            if (_isLoadingFMODReferences) return;
            _isLoadingFMODReferences = true;
            _fMODReferences = await FMODReferencesEditorAsset.LoadOrCreateAsync(_fMODReferences);
            _buses = new(_fMODReferences.Buses);
            _isLoadingFMODReferences = false;
        }
    }
}
