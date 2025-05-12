using System.Dynamic;
using Integrate.Registry;

namespace Integrate.ModContent
{
    public class Content
    {
        public string registry = "content";
        public string name = "item";
        public ExpandoObject constructible = [];
        public string JSON = "{}";
        public void Implement()
        {
            if (!ModLoader.registries.Has(registry)) throw new InvalidOperationException("There is no moddable registry named '" + registry + "'");
            ModLoader.registries.Get(registry).Add(name, constructible);
        }
        public T? Construct<T>() where T : class, IConstructible, new()
        {
            return ModLoader.Construct<T>(constructible);
        }
    }
}
