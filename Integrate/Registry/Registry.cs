using System.Data;
using System.Dynamic;

namespace Integrate.Registry
{
    public class Registry : Registry<ExpandoObject> { }
    public class TypeRegistry : Registry<Type> { }
    /// <summary>
    /// Data stucture for case-insensitive many-to-one key-value relationships.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Registry<T> where T : notnull
    {
        private readonly Dictionary<string, T> content = [];
        public int Size => content.Count;

        /// <summary>
        /// Adds an item to the registry.
        /// </summary>
        /// <param name="name">Case-insensitive name of the item.</param>
        /// <param name="item">The item to add.</param>
        /// <exception cref="DuplicateNameException"></exception>
        public void Add(string name, T item)
        {
            name = ProcessName(name);
            //Throw an error if the item already exists.
            if (Has(name))
                throw new DuplicateNameException(
                  "Item " +
                    name +
                    " already exists in registry! Consider using a different name."
                );
            //Add to internal Dictionary
            content.Add(name, item);
        }

        public bool Has(string name)
        {
            name = ProcessName(name);
            //Return presence
            return content.ContainsKey(name);
        }
        public T Get(string name)
        {
            name = ProcessName(name);
            //Throw an error if the item doesn't exist.
            if (!Has(name))
                throw new KeyNotFoundException(
                  "Item " +
                    name +
                    " does not exist in registry! Consider checking your spelling."
                );
            //Return item, if it exists.
            T item = content[name];
            if (item is RegistryItem ri)
            {
                ri.registryName = name;
            }
            return item;
        }

        public void Rename(string name, string newName)
        {
            name = ProcessName(name);
            //Get entry
            T current = Get(name);
            //Remove current entry
            content.Remove(name);
            //Add new entry
            Add(newName, current);
        }

        public void Alias(string name, string otherName)
        {
            ProcessName(name);
            //Get current entry
            T current = this.Get(name);
            //Add new entry with the same content
            Add(otherName, current);
        }
        public void ForEach(Action<string, T> action)
        {
            foreach (var keyValue in content)
            {
                action.Invoke(keyValue.Key, keyValue.Value);
            }
        }
        public async void ForEachAsync(Action<string, T> action)
        {
            await Task.Run(
                () =>
                {
                    foreach (var keyValue in content)
                    {
                        action.Invoke(keyValue.Key, keyValue.Value);
                    }
                }
                );
        }
        public T At(int index)
        {
            return content.ElementAt(index).Value;
        }

        private static string ProcessName(string name)
        {
            if (HasNonAscii(name))
                throw new FormatException("Registry names may only contain ASCII characters");
            return name.ToLower();
        }
        public static bool IsValidName(string name)
        {
            try
            {
                ProcessName(name);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static bool HasNonAscii(string str) => str.ToCharArray().Where(c => (int)c > 127).Any();
    }
}
