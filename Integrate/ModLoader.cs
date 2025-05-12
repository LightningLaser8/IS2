using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using Integrate.Loader;
using Integrate.Metadata;
using Integrate.ModContent;
using Integrate.Registry;

namespace Integrate
{
    /// <summary>
    /// C# implementation of the Integrate modloader, following the JS version's specification.
    /// </summary>
    public static class ModLoader
    {
        #region Config
        public static void SetPrefix(bool value) => addsPrefix = value;
        internal static bool addsPrefix = false;
        internal static string prefix = "";
        public static void SetInfoOutput(Action<string> action) => infoOut = action;
        private static Action<string> infoOut = str => { };
        #endregion
        #region Registries
        public static readonly Registry<Type> types = new();
        public static void AddModdableRegistry(Registry<ExpandoObject> registry, string name)
        {
            registries.Add(name, registry);
        }
        internal static readonly Registry<Registry<ExpandoObject>> registries = new();
        #endregion
        #region Constructors
        /// <summary>
        /// Creates an object from an object model (<see cref="ExpandoObject"/>). Use <see cref="IConstructor{T}.Construct"/> for more specialised cases.
        /// </summary>
        /// <typeparam name="T">The (default) type of the object being constructed.</typeparam>
        /// <param name="source">The constructible (model) used to create the object.</param>
        /// <returns></returns>
        public static object Construct(ExpandoObject source, Type type)
        {
            ExpandoObject? cloned;
            try
            {
                cloned = DeepCopy(source);
                //cloned = JsonCopy(source);
            }
            catch (Exception e)
            {
                cloned = source;
                infoOut("Could not clone object:" + e.Message);
            }
            cloned ??= source;
            var instantiated = Activator.CreateInstance(type);
            if (instantiated is null) return new { };
            //Do the assignment
            Assign(instantiated, cloned);
            //PropertyCopier.Copy(cloned, instantiated);
            if (instantiated is IConstructible ic) ic.Init();
            return instantiated;
        }
        public static T? Construct<T>(ExpandoObject source) where T : class
        {
            return Construct(source, typeof(T)) as T;
        }
        public static object Construct(string sourceName, Registry<ExpandoObject> source, Type type)
        {
            return Construct(source.Get(sourceName), type);
        }
        public static T? Construct<T>(string sourceName, Registry<ExpandoObject> source) where T : class
        {
            return Construct(sourceName, source, typeof(T)) as T;
        }
        public static object Construct(string sourceName, string registry, Type type)
        {
            return Construct(registries.Get(registry).Get(sourceName), type);
        }
        public static T? Construct<T>(string sourceName, string registry) where T : class
        {
            return Construct(sourceName, registry, typeof(T)) as T;
        }
        public static object Construct(string sourceName, Type type)
        {
            Registry<ExpandoObject>? chosen = null;
            registries.ForEach((name, reg) =>
            {
                if (reg.Has(sourceName)) chosen = reg;
            });
            if (chosen is null) return new();
            return Construct(chosen.Get(sourceName), type);
        }
        public static T? Construct<T>(string sourceName) where T : class
        {
            return Construct(sourceName, typeof(T)) as T;
        }
        public static void Assign(object target, ExpandoObject source)
        {
            var propdict = source.ToDictionary();
            foreach (var prop in source.Select(x => x.Key))
            {
                Type inType = target.GetType();
                var instProp = inType.GetMember(prop);
                Debug.WriteLine(propdict[prop] + " (" + propdict[prop]?.GetType().Name + ")");
                if (instProp.Length == 0) { infoOut($" ⚠️ '{prop}' does not exist on type '{inType.Name}'"); continue; }
                var toOverwrite = instProp[0];
                //if (!toOverwrite) { infoOut($" ⚠️ Cannot write to '{instProp.Name}', use something else"); continue; }
                if (toOverwrite.MemberType == MemberTypes.Method) { infoOut($" ⚠️ Cannot overwrite a method ({prop})."); continue; }
                if (toOverwrite.MemberType == MemberTypes.Event) { infoOut($" ⚠️ Cannot overwrite an event ({prop}). Use ISL instead."); continue; }
                if (toOverwrite.MemberType == MemberTypes.Constructor) { infoOut($" ⚠️ Very much cannot overwrite a constructor!"); continue; }
                if (toOverwrite.MemberType == MemberTypes.NestedType) { infoOut($" ⚠️ Cannot overwrite a nested type ({prop})."); continue; }
                if (toOverwrite.MemberType == MemberTypes.TypeInfo) { infoOut($" ⚠️ Cannot overwrite a type ({prop})."); continue; }
                if (toOverwrite.MemberType == MemberTypes.Field)
                {
                    var propToOver = inType.GetField(prop);
                    if (propToOver!.IsInitOnly) { infoOut($" ⚠️ Cannot write to a read-only field ({prop})."); continue; }
                    var ptype = propdict[prop]?.GetType();
                    if (propToOver.FieldType != ptype) { infoOut($" ⚠️ {prop} must have type {propToOver.FieldType}, got {ptype}."); continue; }
                    propToOver.SetValue(target, propdict[prop]);
                    infoOut($"  {prop} -> {propdict[prop]}");
                    continue;
                }
                if (toOverwrite.MemberType == MemberTypes.Property)
                {
                    var propToOver = inType.GetProperty(prop);
                    if (!propToOver!.CanWrite) { infoOut($" ⚠️ Cannot write to a read-only property ({prop})."); continue; }
                    var ptype = propdict[prop]?.GetType();
                    if (propToOver.PropertyType != ptype) { infoOut($" ⚠️ {prop} must have type {propToOver.PropertyType}, got {ptype}."); continue; }
                    propToOver.SetValue(target, propdict[prop]);
                    infoOut($"  {prop} -> {propdict[prop]}");
                    continue;
                }
                infoOut($" ⚠️ Cannot overwrite the specified member ({prop}).");
                continue;

            }
        }
        static ExpandoObject DeepCopy(ExpandoObject original)
        {
            var clone = new ExpandoObject();

            var _original = (IDictionary<string, object>)original;
            var _clone = (IDictionary<string, object>)clone;

            foreach (var kvp in _original)
                _clone.Add(kvp.Key, kvp.Value is ExpandoObject ? DeepCopy((ExpandoObject)kvp.Value) : kvp.Value);

            return clone;
        }
        #endregion
        #region Loaders
        public static Mod Load(string path)
        {
            infoOut($"Loading mod '{path}'");
            Directory.CreateDirectory(path);
            infoOut($" Loading base mod files...");
            var modJson = ModFileHandler.LoadModJsonFile(Pathify(path, "mod.json"));
            infoOut("  'mod.json' loaded.");
            prefix = modJson.Name;
            var mod = new Mod
            {
                DisplayName = modJson.DisplayName,
                Name = modJson.Name,
                Version = modJson.Version,
                Author = modJson.Author,
                Tagline = modJson.Tagline,
                Description = modJson.Description,
            };
            infoOut("  'mod.json' processed.");
            string defpath = Pathify(path, modJson.Definitions);
            var defs = ModFileHandler.LoadDefinitionJsonFile(defpath);
            infoOut($"  Definition file '{defpath}' loaded.");
            var content = defs.defs.Select(eobj =>
            {
                var stuff = eobj.Select(x => new KeyValuePair<string, string>(x.Key, x.Value?.ToString() ?? "")).ToDictionary();
                return new DefinitionEntry()
                {
                    name = (addsPrefix ? prefix + ':' : string.Empty) + stuff.GetValueOrDefault("name", "mod"),
                    path = stuff.GetValueOrDefault("path", "./content.json"),
                    registry = stuff.GetValueOrDefault("registry", "content")
                };
            });
            foreach (var item in content)
            {
                infoOut("   Defined content: " + item.ToString());
            }
            infoOut($"  Definition file '{defpath}' processed.");

            infoOut($" Processing content files...");
            List<Content> conts = [];
            foreach (var item in content)
            {
                infoOut("  Loading content: " + item.path);
                var citem = ModFileHandler.LoadContentFile(Pathify(path, item.path));
                infoOut($"   '{item.name}' loaded.");
                var createdContent = new Content()
                {
                    name = item.name,
                    registry = item.registry,
                    JSON = JsonSerializer.Serialize(citem, ModFileHandler.JsonOptions),
                    constructible = citem
                };
                conts.Add(createdContent);
                infoOut($"   '{item.name}' content registered: {createdContent.JSON}");
            }
            mod.content = [.. conts];
            infoOut($" Mod loaded: \n{mod.Describe()}");
            infoOut($"Mod loading complete.");
            return mod;
        }
        public static Mod Add(string path)
        {
            var mod = Load(path);
            infoOut("Implementing " + mod.DisplayName);
            foreach (var item in mod.content)
            {
                infoOut("  Content item " + item.name);
                item.Implement();
            }
            infoOut("Mod implementation complete.");
            return mod;
        }

        private static string Pathify(string basePath, string path)
        {
            return Path.GetFullPath(Path.Combine(basePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar), path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)));
        }
        #endregion
    }
}
