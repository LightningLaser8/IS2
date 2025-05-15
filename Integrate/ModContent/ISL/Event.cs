using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Types;

namespace Integrate.ModContent.ISL
{
    /// <summary>
    /// A trigger for a group of scripts to run at once.
    /// </summary>
    public sealed class Event
    {
        public Event(string name)
        {
            Name = name;
        }
        public Event() { }
        public Event(string name, Dictionary<string, object?> defaultInputs)
        {
            Name = name;
            Inputs = defaultInputs;
        }
        /// <summary>
        /// The ISL-able name: What ISL will refer to this event as.
        /// </summary>
        public string Name { get; init; } = "event";
        /// <summary>
        /// Fires this event on a specific mod.
        /// </summary>
        public IslExecutionResult Fire(Mod mod)
        {
            var res = new IslExecutionResult();
            foreach (var script in mod.scripts)
            {
                var ev = script.GetMetadata("event");
                if (ev.Length == 0) throw new ScriptException(script, new("Metatag 'event' must specify at least one event."));
                if (ev.Contains(Name))
                {
                    script.SetInputs(Inputs);
                    res += script.Execute();
                }
            }
            return res;
        }
        /// <summary>
        /// All inputs to send to scripts in the current execution.
        /// </summary>
        public Dictionary<string, object?> Inputs { get; private set; } = [];
        /// <summary>
        /// Quickly sets many inputs.
        /// Each input can be <see cref="bool"/>, <see cref="int"/>, <see cref="long"/>, <see cref="float"/>, <see cref="double"/>, <see cref="string"/>, <see cref="Complex"/>, <see cref="null"/> or any <see cref="IslValue"/>. Other inputs are ignored.<br/>
        /// Will not duplicate inputs, instead will overwrite them.
        /// </summary>
        /// <param name="inputs"></param>
        public void SetInputs(IDictionary<string, object?> inputs)
        {
            foreach (var input in inputs)
            {
                SetInput(input.Key, input.Value);
            }
        }
        /// <summary>
        /// Sets one input.
        /// The input can be <see cref="bool"/>, <see cref="int"/>, <see cref="long"/>, <see cref="float"/>, <see cref="double"/>, <see cref="string"/>, <see cref="Complex"/>, <see cref="null"/> or any <see cref="IslValue"/>. Other inputs are ignored.<br/>
        /// Will not duplicate inputs, instead will overwrite them.
        /// </summary>
        /// <param name="inputs"></param>
        public void SetInput(string name, object? value)
        {
            if (!Inputs.TryAdd(name, value)) Inputs[name] = value;
        }
    }
}
