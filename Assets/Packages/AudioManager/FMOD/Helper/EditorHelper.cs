#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ThanhDV.AudioManager.FMOD
{
    public static class EditorHelper
    {
        public static void CreateHeader(string title, string subtitle)
        {
            // Header
            GUIStyle titleStyle = new(EditorStyles.boldLabel)
            {
                fontSize = 30,
                alignment = TextAnchor.MiddleCenter
            };

            GUIStyle subtitleStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(title, titleStyle, GUILayout.Height(50));
            EditorGUILayout.LabelField(subtitle, subtitleStyle);
            DrawHorizontalLine();
        }

        public static void DrawHorizontalLine(int thickness = 1, int padding = 10, Color? color = null)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2f;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color ?? new Color(0.5f, 0.5f, 0.5f, 1));
        }

        public static void DrawListWithoutHeader(SerializedProperty listProp, string countLabel)
        {
            if (listProp == null)
                return;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                int currentSize = listProp.arraySize;
                int newSize = currentSize;

                float rowHeight = EditorGUIUtility.singleLineHeight;
                Rect row = EditorGUILayout.GetControlRect(false, rowHeight);

                const float labelWidth = 150f;
                const float buttonWidth = 20f;
                const float spacing = 4f;

                Rect labelRect = new Rect(row.x, row.y + 1f, labelWidth, rowHeight);
                Rect minusRect = new Rect(row.xMax - buttonWidth, row.y + 2f, buttonWidth, rowHeight);
                Rect plusRect = new Rect(minusRect.x - spacing - buttonWidth, row.y + 2f, buttonWidth, rowHeight);
                Rect fieldRect = new Rect(labelRect.xMax + spacing, row.y + 2.75f, plusRect.x - spacing - (labelRect.xMax + spacing), rowHeight);

                EditorGUI.LabelField(labelRect, countLabel);
                newSize = EditorGUI.DelayedIntField(fieldRect, currentSize);
                if (GUI.Button(plusRect, "+")) newSize = currentSize + 1;
                if (GUI.Button(minusRect, "-")) newSize = Mathf.Max(0, currentSize - 1);

                newSize = Mathf.Max(0, newSize);
                if (newSize != currentSize)
                    listProp.arraySize = newSize;

                EditorGUILayout.Space(4);

                for (int i = 0; i < listProp.arraySize; i++)
                {
                    SerializedProperty element = listProp.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(element, includeChildren: true);
                }
            }
        }

        public static void EnsureFolderPath(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || AssetDatabase.IsValidFolder(folderPath)) return;

            string[] parts = folderPath.Split('/');
            if (parts.Length == 0) return;

            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
#endif