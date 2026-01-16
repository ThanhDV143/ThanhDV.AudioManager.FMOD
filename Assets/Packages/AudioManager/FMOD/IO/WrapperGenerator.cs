#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace ThanhDV.AudioManager.FMOD
{
    public static class WrapperGenerator
    {
        public static void GenerateFMODBus(List<BusEntry> busEntries)
        {
            Directory.CreateDirectory(Common.FMOD_REF_SCRIPT_FOLDER);

            string content = BuildFMODBus(busEntries);

            File.WriteAllText(Common.FMOD_BUS_SCRIPT_PATH, content);

            AssetDatabase.ImportAsset(Common.FMOD_BUS_SCRIPT_PATH);
            AssetDatabase.Refresh();
        }

        public static void GenerateFMODEventReference(List<EventReferenceEntry> eventReferenceEntries)
        {
            Directory.CreateDirectory(Common.FMOD_REF_SCRIPT_FOLDER);

            string content = BuildFMODEventReference(eventReferenceEntries);

            File.WriteAllText(Common.FMOD_EVENT_REF_SCRIPT_PATH, content);

            AssetDatabase.ImportAsset(Common.FMOD_EVENT_REF_SCRIPT_PATH);
            AssetDatabase.Refresh();
        }

        private static string BuildFMODBus(List<BusEntry> busEntries)
        {
            if (busEntries == null || busEntries.Count == 0) return string.Empty;

            var usedIdentifiers = new HashSet<string>(StringComparer.Ordinal);
            var sb = new StringBuilder();

            sb.AppendLine($"using FMOD.Studio;");
            sb.AppendLine($"");
            sb.AppendLine($"namespace ThanhDV.AudioManager.FMOD");
            sb.AppendLine("{");
            sb.AppendLine($"    public static class FMODBus");
            sb.AppendLine("    {");
            foreach (BusEntry entry in busEntries)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.Key)) continue;

                string identifier = MakeUniqueIdentifier(MakeSafeIdentifier(entry.Key), usedIdentifiers);
                string keyLiteral = EscapeForCSharpStringLiteral(entry.Key);

                sb.AppendLine($"        private static Bus? _cached{identifier};");
                sb.AppendLine($"        public static Bus {identifier}");
                sb.AppendLine("        {");
                sb.AppendLine("            get");
                sb.AppendLine("            {");
                sb.AppendLine($"                if (_cached{identifier}.HasValue && _cached{identifier}.Value.isValid()) return _cached{identifier}.Value;");
                sb.AppendLine($"                return LoadAndCache(\"{keyLiteral}\", ref _cached{identifier});");
                sb.AppendLine("            }");
                sb.AppendLine("        }");
                sb.AppendLine();
            }
            sb.AppendLine($"        private static Bus LoadAndCache(string key, ref Bus? cacheField)");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (!AudioManager.IsExist || AudioManager.Instance.FMODReferences == null)");
            sb.AppendLine("            {");
            sb.AppendLine("                DebugLog.Warning($\"AudioManager is not initialized or FMODReferences is null. Cannot resolve bus key '{key}'.\");");
            sb.AppendLine($"                return default;");
            sb.AppendLine("            }");
            sb.AppendLine($"");
            sb.AppendLine($"            Bus result = AudioManager.Instance.FMODReferences.GetBus(key);");
            sb.AppendLine($"");
            sb.AppendLine($"            if (result.isValid()) cacheField = result;");
            sb.AppendLine($"");
            sb.AppendLine($"            return result;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static string BuildFMODEventReference(List<EventReferenceEntry> eventRefEntries)
        {
            if (eventRefEntries == null || eventRefEntries.Count == 0) return string.Empty;

            var usedIdentifiers = new HashSet<string>(StringComparer.Ordinal);
            var sb = new StringBuilder();

            sb.AppendLine($"using FMODUnity;");
            sb.AppendLine($"");
            sb.AppendLine($"namespace ThanhDV.AudioManager.FMOD");
            sb.AppendLine("{");
            sb.AppendLine($"    public static class FMODEventReference");
            sb.AppendLine("    {");
            foreach (EventReferenceEntry entry in eventRefEntries)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.Key)) continue;

                string identifier = MakeUniqueIdentifier(MakeSafeIdentifier(entry.Key), usedIdentifiers);
                string keyLiteral = EscapeForCSharpStringLiteral(entry.Key);

                sb.AppendLine($"        private static EventReference? _cached{identifier};");
                sb.AppendLine($"        public static EventReference {identifier}");
                sb.AppendLine("        {");
                sb.AppendLine("            get");
                sb.AppendLine("            {");
                sb.AppendLine($"                if (_cached{identifier}.HasValue && !_cached{identifier}.Value.IsNull) return _cached{identifier}.Value;");
                sb.AppendLine($"                return LoadAndCache(\"{keyLiteral}\", ref _cached{identifier});");
                sb.AppendLine("            }");
                sb.AppendLine("        }");
                sb.AppendLine();
            }
            sb.AppendLine($"        private static EventReference LoadAndCache(string key, ref EventReference? cacheField)");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (!AudioManager.IsExist || AudioManager.Instance.FMODReferences == null)");
            sb.AppendLine("            {");
            sb.AppendLine("                DebugLog.Warning($\"AudioManager is not initialized or FMODReferences is null. Cannot resolve EventReference key '{key}'.\");");
            sb.AppendLine($"                return default;");
            sb.AppendLine("            }");
            sb.AppendLine($"");
            sb.AppendLine($"            EventReference result = AudioManager.Instance.FMODReferences.GetEventReference(key);");
            sb.AppendLine($"");
            sb.AppendLine($"            if (!result.IsNull) cacheField = result;");
            sb.AppendLine($"");
            sb.AppendLine($"            return result;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static string MakeUniqueIdentifier(string baseIdentifier, HashSet<string> used)
        {
            if (used.Add(baseIdentifier)) return baseIdentifier;

            int suffix = 2;
            while (true)
            {
                string candidate = baseIdentifier + "_" + suffix;
                if (used.Add(candidate)) return candidate;
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

            string identifier = sb.ToString();
            return IsCSharpKeyword(identifier) ? "_" + identifier : identifier;
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

        private static bool IsCSharpKeyword(string identifier)
        {
            return identifier is
                "abstract" or "as" or "base" or "bool" or "break" or "byte" or "case" or "catch" or "char" or "checked" or
                "class" or "const" or "continue" or "decimal" or "default" or "delegate" or "do" or "double" or "else" or "enum" or
                "event" or "explicit" or "extern" or "false" or "finally" or "fixed" or "float" or "for" or "foreach" or "goto" or
                "if" or "implicit" or "in" or "int" or "interface" or "internal" or "is" or "lock" or "long" or "namespace" or "new" or
                "null" or "object" or "operator" or "out" or "override" or "params" or "private" or "protected" or "public" or "readonly" or
                "ref" or "return" or "sbyte" or "sealed" or "short" or "sizeof" or "stackalloc" or "static" or "string" or "struct" or "switch" or
                "this" or "throw" or "true" or "try" or "typeof" or "uint" or "ulong" or "unchecked" or "unsafe" or "ushort" or "using" or
                "virtual" or "void" or "volatile" or "while";
        }
    }
}
#endif