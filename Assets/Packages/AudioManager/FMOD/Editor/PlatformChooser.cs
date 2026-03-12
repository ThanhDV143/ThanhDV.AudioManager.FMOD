using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FMODUnity;
using UnityEditor;

namespace ThanhDV.AudioManager.FMOD
{
    public class PlatformChooser
    {
        [MenuItem(Common.MENU_ITEM + "Platform/Refresh", false, 1)]
        public static void Refresh()
        {
            string[] builtPlatforms = EditorUtils.GetBankPlatforms();
            HashSet<string> usedClassNames = new HashSet<string>(StringComparer.Ordinal);

            DeleteGeneratedPlatformChooserScripts();
            foreach (string p in builtPlatforms)
            {
                GeneratePlatformChooserScript(p, usedClassNames);
            }
        }

        private static void GeneratePlatformChooserScript(string buildDirectory, HashSet<string> usedClassNames)
        {
            Directory.CreateDirectory(Common.FMOD_REF_SCRIPT_FOLDER);

            string className = MakeUniqueIdentifier($"PlatformChooser{MakeSafeIdentifier(buildDirectory)}", usedClassNames);
            string scriptPath = $"{Common.FMOD_REF_SCRIPT_FOLDER}/{className}{Common.SCRIPT_EXTENSION}";
            string escapedBuildDirectory = EscapeForCSharpStringLiteral(buildDirectory);

            var sb = new StringBuilder();

            sb.AppendLine($"using UnityEditor;");
            sb.AppendLine($"");
            sb.AppendLine($"namespace ThanhDV.AudioManager.FMOD");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {className}");
            sb.AppendLine("    {");
            sb.AppendLine($"        [MenuItem(Common.MENU_ITEM + \"Platform/{escapedBuildDirectory}\", false, 0)]");
            sb.AppendLine("        public static void Choose()");
            sb.AppendLine("        {");
            sb.AppendLine($"            PlatformChooser.ChangeDefaultProjectPlatform(\"{escapedBuildDirectory}\");");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        [MenuItem(Common.MENU_ITEM + \"Platform/{escapedBuildDirectory}\", true)]");
            sb.AppendLine("        public static bool ValidateChoose()");
            sb.AppendLine("        {");
            sb.AppendLine($"            string menuPath = Common.MENU_ITEM + \"Platform/{escapedBuildDirectory}\";");
            sb.AppendLine($"            Menu.SetChecked(menuPath, PlatformChooser.IsDefaultPlatform(\"{escapedBuildDirectory}\"));");
            sb.AppendLine("            return true;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(scriptPath, sb.ToString());

            AssetDatabase.ImportAsset(scriptPath);
            AssetDatabase.Refresh();
        }

        public static void ChangeDefaultProjectPlatform(string buildDirectory)
        {
            if (string.IsNullOrWhiteSpace(buildDirectory))
            {
                DebugLog.Error("FMOD build directory cannot be null or empty.");
                return;
            }

            Settings settings = Settings.Instance;
            if (settings == null)
            {
                DebugLog.Error("FMOD settings asset is not available.");
                return;
            }

            Platform defaultPlatform = settings.DefaultPlatform;
            if (defaultPlatform == null)
            {
                DebugLog.Error("FMOD Default platform is not available.");
                return;
            }

            string[] builtPlatforms = EditorUtils.GetBankPlatforms();
            if (!builtPlatforms.Contains(buildDirectory))
            {
                DebugLog.Error("The selected platform has not been built.");
                return;
            }

            SerializedObject serializedPlatform = new(defaultPlatform);
            SerializedProperty buildDirectoryValueProp = serializedPlatform.FindProperty("Properties.BuildDirectory.Value");
            SerializedProperty buildDirectoryHasValueProp = serializedPlatform.FindProperty("Properties.BuildDirectory.HasValue");
            // SerializedProperty speakerModeValueProp = serializedPlatform.FindProperty("Properties.SpeakerMode.Value");
            // SerializedProperty speakerModeHasValueProp = serializedPlatform.FindProperty("Properties.SpeakerMode.HasValue");

            if (buildDirectoryValueProp == null || buildDirectoryHasValueProp == null)
            {
                DebugLog.Error("Could not find FMOD serialized BuildDirectory properties.");
                return;
            }

            Undo.RecordObject(defaultPlatform, "Change FMOD Default Project Platform");

            serializedPlatform.Update();
            buildDirectoryValueProp.stringValue = buildDirectory;
            buildDirectoryHasValueProp.boolValue = true;

            // if (speakerModeValueProp != null && speakerModeHasValueProp != null)
            // {
            //     speakerModeValueProp.intValue = (int)defaultPlatform.SpeakerMode;
            //     speakerModeHasValueProp.boolValue = true;
            // }

            serializedPlatform.ApplyModifiedProperties();

            EditorUtility.SetDirty(defaultPlatform);
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            DebugLog.Success($"FMOD Default Project Platform changed to '{buildDirectory}'.");
        }

        public static bool IsDefaultPlatform(string buildDirectory)
        {
            if (string.IsNullOrWhiteSpace(buildDirectory)) return false;

            Settings settings = Settings.Instance;
            if (settings == null || settings.DefaultPlatform == null) return false;

            return string.Equals(settings.DefaultPlatform.BuildDirectory, buildDirectory, StringComparison.Ordinal);
        }

        private static string MakeUniqueIdentifier(string baseIdentifier, HashSet<string> usedIdentifiers)
        {
            if (usedIdentifiers.Add(baseIdentifier)) return baseIdentifier;

            int suffix = 2;
            while (true)
            {
                string candidate = $"{baseIdentifier}_{suffix}";
                if (usedIdentifiers.Add(candidate)) return candidate;
                suffix++;
            }
        }

        private static string MakeSafeIdentifier(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "Unnamed";

            raw = raw.Trim();
            var sb = new StringBuilder(raw.Length);

            for (int i = 0; i < raw.Length; i++)
            {
                char c = raw[i];
                sb.Append(char.IsLetterOrDigit(c) || c == '_' ? c : '_');
            }

            if (sb.Length == 0) sb.Append("Unnamed");
            if (!char.IsLetter(sb[0]) && sb[0] != '_') sb.Insert(0, '_');

            return sb.ToString();
        }

        private static string EscapeForCSharpStringLiteral(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t");
        }

        private static void DeleteGeneratedPlatformChooserScripts()
        {
            if (!AssetDatabase.IsValidFolder(Common.FMOD_REF_SCRIPT_FOLDER)) return;

            string[] generatedScripts = Directory.GetFiles(Common.FMOD_REF_SCRIPT_FOLDER, "PlatformChooser*.cs", SearchOption.TopDirectoryOnly);
            foreach (string scriptFile in generatedScripts)
            {
                string assetPath = scriptFile.Replace('\\', '/');
                AssetDatabase.DeleteAsset(assetPath);
            }
        }
    }
}
