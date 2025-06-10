using Integrate.Metadata;
using Integrate.ModContent.ISL;
using System.Diagnostics;
using System.Dynamic;
using System.Text.Json;

namespace Integrate.Loader
{
    internal static class ModFileHandler
    {
        internal static readonly JsonSerializerOptions JsonOptions = new()
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
            ReadCommentHandling = JsonCommentHandling.Skip
        };
        internal static ModDotJsonFile LoadModJsonFile(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("Mod has no 'mod.json' file in its base directory.");
            string contents = File.ReadAllText(path);
            var json = JsonSerializer.Deserialize<ExpandoObject>(contents, JsonOptions) ?? new();
            var dict = JsonElementsToCLR(json).ToDictionary();
            var fle = new ModDotJsonFile();
            Debug.WriteLine("found file "+string.Join(", ", json.ToDictionary()));
            //Set values, ignore missing or invalid values
            if (dict.TryGetValue("name", out var v) && v is string s) fle.Name = s;
            if (dict.TryGetValue("displayName", out var v2) && v2 is string s2) fle.DisplayName = s2;
            if (dict.TryGetValue("version", out var v3) && v3 is string s3) fle.Version = s3;
            if (dict.TryGetValue("author", out var v4) && v4 is string s4) fle.Author = s4;
            if (dict.TryGetValue("tagline", out var v5) && v5 is string s5) fle.Tagline = s5;
            if (dict.TryGetValue("description", out var v6) && v6 is string s6) fle.Description = s6;
            if (dict.TryGetValue("definitions", out var v7) && v7 is string s7) fle.Definitions = s7;
            if (dict.TryGetValue("scripts", out var v8) && v8 is string s8) fle.Scripts = s8;
            return fle;
        }
        internal static DefinitionDotJsonFile LoadDefinitionJsonFile(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException($"Mod has no definition file at the specified path ({path}).");
            string contents = File.ReadAllText(path);
            var json = JsonSerializer.Deserialize<ExpandoObject[]>(contents, JsonOptions) ?? [];
            return new DefinitionDotJsonFile() { defs = json };
        }
        internal static string[] LoadScriptJsonFile(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException($"Mod has no scripts file at the specified path ({path}).");
            string contents = File.ReadAllText(path);
            return JsonSerializer.Deserialize<string[]>(contents, JsonOptions) ?? [];
        }
        internal static ExpandoObject LoadContentFile(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException($"There is no file at the specified path ({path}).");
            string contents = File.ReadAllText(path);
            var json = JsonSerializer.Deserialize<ExpandoObject>(contents, JsonOptions) ?? [];
            return JsonElementsToCLR(json);
        }
        internal static Script LoadScriptFile(string path)
        {
            if (!path.EndsWith(".isl")) path += ".isl";
            if (!File.Exists(path)) throw new FileNotFoundException($"There is no file at the specified path ({path}).");
            string contents = File.ReadAllText(path);
            return new Script(contents, path);
        }
        private static ExpandoObject JsonElementsToCLR(ExpandoObject eo)
        {
            var obj = eo as IDictionary<string, object?>;
            for (int i = 0; i < obj.Count; i++)
            {
                KeyValuePair<string, object?> kvp = obj.ElementAt(i);
                if (kvp.Value is JsonElement jse)
                {
                    obj[kvp.Key] = JsonElementToCLR(jse);
                    Debug.WriteLine(obj[kvp.Key]?.GetType().Name ?? "null");
                }
            }
            return eo;
        }
        private static object? JsonElementToCLR(JsonElement jse)
        {
            if (jse.ValueKind == JsonValueKind.Object) return JsonElementsToCLR(jse.Deserialize<ExpandoObject>() ?? new());
            if (jse.ValueKind == JsonValueKind.True) return true;
            if (jse.ValueKind == JsonValueKind.False) return false;
            if (jse.ValueKind == JsonValueKind.Null) return null;
            if (jse.ValueKind == JsonValueKind.Undefined) return null;
            if (jse.ValueKind == JsonValueKind.True) return true;
            if (jse.ValueKind == JsonValueKind.Number)
            {
                if (jse.GetRawText().Contains('.')) return jse.GetDouble();
                else return jse.GetInt64();
            }
            if (jse.ValueKind == JsonValueKind.String) return jse.GetString();
            if (jse.ValueKind == JsonValueKind.Array) return jse.EnumerateArray().Select(JsonElementToCLR).ToList();
            return null;
        }
    }
}
