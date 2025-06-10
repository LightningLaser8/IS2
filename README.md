<img src="./is2.png" width="25%">

# IS2: ISL Reimagined
<strong style="color: red; font-size: 120%">⚠️ This project is in development, and this documentation may not provide fully accurate information.<br>The APIs shouldn't change, but don't rely on it too much.</strong>

## Module: ISL

IS2 is primarily a C# implementation of Integrate, but has made some improvements to its scripting language, ISL.  
Basic Hello World (even though it's fairly useless in a scripting language):

```
out string hw = "Hello World!";
```

### Syntactical Differences

The new ISL bears little resemblance to the original, being closer to high-level languages such as JavaScript in syntax.

---

This is an example of a program to calculate the distance from the origin to a point, in the old ISL:

```
number x
set x 100
number y
set y 250
number dist

number temp1
set temp1 \x\
exponent temp1 2

number temp2
set temp2 \y\
exponent temp2 2

add temp1 \temp2\
root temp1 2
set dist \temp1\
delete temp1
delete temp2

log \dist\
```

That's very verbose, and hard to read. In contrast, here's the same program in IS2:

```
int x = 100;
int y = 250;
out float dist = ((\x\** 2)+(\y\** 2))** 0.5;
```

Much easier to read.

---

Instead of using keywords for _everything_, IS2 uses **Expression Trees**. Put simply, it's an abstract way of representing an operation in terms of nested expressions (operators with operands).  
This makes it possible to inline almost everything in the language, substantially reducing the amount of code required.

### Feature List

#### Keywords

`if <condition> <statement>` If \<condition> returns true, evaluate \<statement>.  
`else <statement>` If the last `if`'s \<condition> was false, evaluate \<statement>.  
`elseif <condition> <statement>` Combines `if` and `else`.

#### Operators

##### Basic Mathematics

`<augend> + <addend>` Returns the result of the addition of \<addend> to \<augend>.  
`<minuend> - <subtrahend>` Returns the result of the subtraction of \<subtrahend> from \<minuend>.  
`<multiplicand> * <multiplier>` Returns the result of the multiplication of \<multiplicand> by \<multiplier>.  
`<dividend> / <divisor>` Returns the result of the division of \<dividend> by \<divisor>.  
`<dividend> % <divisor>` Returns the remainder of the division of \<dividend> by \<divisor>. The (integer) quotient can be obtained with `(<dividend> / <divisor>) -> Int`.  
`<base> ** <power>` Returns the result of the exponentiation of \<base> to the power of \<power>.

##### Trigonometry:

`sin <angle>` Returns the sine of \<angle> (where \<angle> is measured in radians.)  
`cos <angle>` Returns the cosine of \<angle> (where \<angle> is measured in radians.)  
`tan <angle>` Returns the tangent of \<angle> (where \<angle> is measured in radians.)

##### Logical Operators:

`!<value>` Returns the (bitwise) inverse of \<value>.  
`<left> == <right>` Returns `true` if \<left> and \<right> have the same value, otherwise `false`.

##### Binary Manipulation:

`binmant <float>` Returns the binary mantissa of the floating-point number \<float>.  
`binexp <float>` Returns the binary exponent of the floating-point number \<float>.

##### Variable Manipulation:

`<var> = <value>` Sets the value of the variable at \<var> to \<value>.  
`<var> += <value>` Sets the value of the variable at \<var> to its current value + \<value>.  
`<var> -= <value>` Sets the value of the variable at \<var> to its current value - \<value>.  
`<var> *= <value>` Sets the value of the variable at \<var> to its current value \* \<value>.  
`<var> /= <value>` Sets the value of the variable at \<var> to its current value / \<value>.  
`<var> %= <value>` Sets the value of the variable at \<var> to its current value % \<value>.  
`<var> **= <value>` Sets the value of the variable at \<var> to its current value \*\* \<value>.  
`<var> <~ <value>` Appends \<value> to the end of \<var>. Returns the variable's value. (No, this isn't a mistake, the operator's overloaded)

##### Collection Manipulation:

**Works on groups and strings.**  
`<col> <~ <value>` Appends \<value> to the end of \<col>. Returns the collection.  
`<col> at <index>`, Returns the element at index \<index> of the collection \<col>. Can be used to search strings.

##### Type Conversion:

`<value> -> <type>`, Returns \<value> as an instance of type \<type>. \<type> must be a valid type name, such as `string` or `int`. Will throw an error if the conversion is nonsense, such as `12.5 -> group`.  
`<value> ~> <type>`, The same as `<value> -> <type>`, except invalid operations will not throw errors, and will instead use the default value for a variable of type \<type>. (e.g. `12.5 ~> group` returns `[]`, instead of throwing an error.)  

##### Variables:

`out <declaration>` Sets a variable to be output on program end.  
`in <declaration>` Sets a variable to an input of the same name.

#### Constructs :

`( ... )` Bracket expression: Encapsulates an expression, causing it to be evaluated first.  
`[ ... , ... ]` Collection expression: Combines multiple expressions into a group structure.  
`{ ... }` Code block: Evaluates multiple expressions, and returns the result of the final one. Creates a new scope for variables.  
`\ ... \` Variable getter: Returns the value of the variable at the result of the contained expression.  
`<modifier?> <type> <name>` Variable declaration. Adds a variable with name \<name> and type \<type> to the current scope, optionally modified by \<modifier>.  

##### Native Types:

`string` A collection of characters.  
`int` A 64-bit integral number.  
`float` A double-precision floaring point number.  
`complex` A complex number, where the real and imaginary parts are both `float`s.  
`bool` A Boolean value (`true` or `false`).  
`group`A generic untyped collection of values.  
`infer` Nothing, until any value is set, at which point it becomes that type (`infer foo = 3` makes `foo` an `int`, as `3` is an `int`, so has the same effect as `int foo`).

##### Modifiers

`imply` Modifies a variable to automatically cast values to its type on assignment. Cannot be used with `infer`.  
`const` Makes a variable read-only. Cannot be used with `infer` or `imply`.  

#### Comments:

`// ...` Line comment: Stops the whole line after it from being read by the interpreter.  
`/* ... */` Block comment: Stops its contents from being read by the interpreter.  
`[tag value1 value2 ... valueN]` Metadata Tag: Provides information about the program to the interpreter, and the surrounding project. Projects can obtain `value1 ...valueN` as an array with `IslProgram.GetMeta(tagName)`.

### Restrictions and Flexibility

All operators and keywords must be surrounded by spaces.  
All statements must be separated by a semicolon.  
However, **Those are pretty much the only constraints.**

Almost anything can be inlined:

```
in infer health;
out imply int newHealth;
newHealth = \health\;
newHealth -= 1;
```

Is identical to:

```
in infer health;
out imply int newHealth = \health\ - 1;
```

And even:

```
out imply int newHealth = \in infer health\ - 1;
```

They all produce the result:

```
(Int) health = 100
--------\/--------
(Int) newHealth = 99
```

## Module: Integrate

IS2 contains a C# version of Integrate, a JavaScript library for mod loading into serialisable registries.  
It can be used in conjunction with ISL to perform complex operations using scripts.

### Terminology

- A _registry_ is a data structure for holding case-insensitive key-value pairs. Simply, it matches names to objects, without caring about capitalisation. They are instances of `Integrate.Registry.Registry`.
- *Registry name*s or *Registry location*s are strings which are keys in a _registry_. They can be used to refer to a _constructible object_.
- *Constructible object*s are basic, serialisable objects with a `type` property, holding a _registry name_ of a class.
- _Content_ refers to any _constructible object_ with a _registry name_, defined by the _mod_. Any content is an instance of `Integrate.Content`.
- A _mod_ is a directory of files, each one adding _content_.
- A _content file_ is a JSON file holding a _constructible object_.

#### ISL and Scripts

- _ISL_, or _Integrate Scripting Language_ is an interpreted scripting language for use with this modloader, to create complex events.
- A _script_ is a `.isl` file to be executed when an _event_ is fired. For more info, see the **Script Files** section, under **Example (Mod Structure)**.
- An _event_ is a signal from the game that Integrate needs to run some scripts. Every script needs to define which events they fire on. For more info, see the **Events** section, under **Example (C# Implementation)**.

---

### Example (C# Implementation)

_Adding Integrate mods to your project_

```c#
using System;
using Integrate;
using Integrate.Registry.Registry;
//Game Setup
class Entity {}
class Block {
  public long width;
  public long height;
  public double health;
  public override string ToString() =>
    $"Block {width}x{height}: {health}"
}
class Item {}
/*
ModLoader.types.Add(entity, typeof(Entity));
ModLoader.types.Add(block, typeof(Block));
ModLoader.types.Add(item, typeof(Item));
*/

//Modloader Setup
var content = new Registry(); //Use the non-generic ExpandoObject registry
ModLoader.AddModdableRegistry("content", content);
ModLoader.SetPrefix(true);

//Tests
ModLoader.Add("./mod");
content.ForEach(x => Console.WriteLine(ModLoader.Construct\<Block>(x).ToString()));
```

On execution, logs:

```
> Block 30x30: 100.0
```

---

### Example (Mod Structure)

_The directory structure for Integrate mods_

```
(mod root)
 |-> mod.json
 |-> definition file
 |-> scripts file
 |=> (content)
 |=> (scripts)
```

#### mod.json

Holds the basic information for the mod:

```json
{
  "name": "example",
  "displayName": "Example Mod",
  "definitions": "./definitions.json",
  "scripts": "./scripts.json",
  "tagline": "Basic mod to show functionality.",
  "description": "This mod exists only to show functionality of the modloader, and is not intended to be played with in any game. It is purely for demonstrative purposes.",
  "author": "LightningLaser8",
  "version": "0.1.0"
}
```

`name` defines the _mod identifier_ - a string used to differentiate this mod's content form another's.  
`displayName` defines the name shown, both in info and possibly other parts of the program.  
`tagline` defines a _short_ description of the mod, usually a single line.  
`description` defines a longer description, which can be multiple lines, and should describe the type of content, or the premise of the mod.  
`author` defines the name that should be shown to have made the mod.  
`version` defines the _mod's version_, should be used to detect updated mods in saves, for example.

`definitions` gives the path _from the mod.json file_ to the definition file.
`scripts` gives the path _from the mod.json file_ to the script definition file.

#### Definition File

This is the most important file in any Integrate mod, defining paths and registry names of _content_.

```json
[
  {
    "path": "./wall.json",
    "name": "wall",
    "registry": "content"
  }
]
```

It consists of a _single array_, each entry being an object with these three properties:  
`path` defining the _relative location_ of the _content file_ being described.  
`name` being the _registry name_ of this content.  
`registry` being optional, defining the registry this content will be added to. By default, this will be `content`. **This registry does not exist by default, and will throw errors if not defined using `ModLoader.AddModdableRegistry()`.**

The `scripts` version is defined similarly, but instead as an array of strings:

```json
[
  "./load.isl", 
  "./player/damage.isl"
]
```

where each line is a path to a script file. The path may exclude the `.isl` file extension, in which case one will be appended for you.

#### Content Files

These describe the actual content itself, not metadata.
They can be anywhere, even outside the mod directory, as long as the definition file points to them, and the program can reach them.  
This is to leave organisation up to the mod developer, so you can organise the files hovever you like.

```json
{
  "type": "block",
  "width": 20,
  "height": 20,
  "health": 200
}
```

`type` is mandatory, it defines the _registry name_ of the class this object will be an instance of.  
`width`, `height` and `health` are specific to this type, and are not necessary in content files. They are properties of the class stored at `block` in the (private) Registry `ModLoader.types`.

All class properties must be public to be overwritten, and can be either a field or property. Attempting to overwrite any other member type will log an error message, and continue as if the property didn't exist.

#### Script Files

These are `.isl` files which define complex events.  
They follow basic IS2 syntax, with some extras for Integrate:

```
// This tag means the script will run
// when the event 'load' is fired.
[event load]
// Use the event's 'modcount' input
in int modcount;

//Do something with the input
out string _ = "There are " + (\modcount\ -> String) + " mods loaded.";
```

The `event` tag is handled by Integrate itself, project devs don't need to add this.  
It's required for the script to fire by itself, but project devs can get the object and execute it manually (not recommended due to lack of inputs.)

---

### Interface (Integrate.ModContent.ModLoader)

Integrate has several functions to customise modloading, which are documented here.  
Most are properties of the static class `ModLoader`, in the namespace `Integrate`.

#### ModLoader.Add()

`ModLoader.Add()` loads, constructs and implements a mod all in one go.

```c#
public static Integrate.ModContent.Mod Add(string path)
```

`path` is the relative path from the current window location to the mod's _root directory_, **not** the mod.json.

#### ModLoader.Load()

`ModLoader.Load()` loads a mod from a path, and returns the `Integrate.ModContent.Mod` object.

```c#
public static Integrate.ModContent.Mod Load(string path)
```

`path` is the relative path from the current window location to the mod's _root directory_, **not** the mod.json.  
Returns an `Integrate.ModContent.Mod` object, holding all the info about the imported mod. Once loaded, this object is all that's needed.

#### ModLoader.AddModdableRegistry()

`ModLoader.AddModdableRegistry()` adds a registry to the list of modifiable registries. This list defines which registries mods can add content to.

```c#
public static void AddModdableRegistry(Integrate.Registry.Registry reg, string name)
```

`reg` is the `Integrate.Registry.Registry` (or subclass thereof) to allow modification of.  
`name` is the string that this registry will be referred to by.

#### ModLoader.SetPrefix()

`ModLoader.SetPrefix()` changes whether or not mod content's registry names should be prefixed with the mod's `name`.

```c#
public static void SetPrefix(bool value)
```

`value` is the new Boolean value of this flag. `true` means prefixes on, `false` means prefixes off. By default this is `false`.

#### ModLoader.SetInfoOutput()

`ModLoader.SetInfoOutput()` changes the way Integrate shows status messages.

```c#
public static void SetInfoOutput(Action\<string> func)
```

`func` callback for each status message. The parameter `info` contains the message, as a string. By default, Integrate does nothing with the log messages.

#### ModLoader.types

`ModLoader.types` is an `Integrate.Registry.Registry\<Type>` holding all types mod content can be an instance of.

```c#
public static readonly Registry\<Type> types
```

#### ModLoader.Construct()

`ModLoader.Construct()` is a helpful function that combines `Integrate.Registry.Registry.Create()` and `Integrate.Registry.Registry.Construct()` for mod content. It constructs an object either literally or from any moddable registry, using types from `ModLoader.types`, or using types directly.

```c#
public static object Construct(ExpandoObject source, Type type);

public static T? Construct<T>(ExpandoObject source) where T : class;

public static object Construct(string sourceName, Registry<ExpandoObject> source, Type type);

public static T? Construct<T>(string sourceName, Registry<ExpandoObject> source) where T : class;

public static object Construct(string sourceName, string registry, Type type);

public static T? Construct<T>(string sourceName, string registry) where T : class;

public static object Construct(string sourceName, Type type);

public static T? Construct<T>(string sourceName) where T : class;
```

### Classes

#### Integrate.ModContent.ModContent.Content

```c#
public class Content
{
  public string registry;
  public string name;
  public ExpandoObject constructible;
  public string JSON;
  public void Implement();
  public T? Construct<T>() where T : class, IConstructible, new();
}
```

`registry` Name of the registry this content is to be added to.
`name` Name of this content in registry.  
`constructible` The JSON serialisable constructible object used to create instances of this content.  
`JSON` The JSON equivalent of the constructible.  
`Implement()` Adds this content to its designated registry.
`Create()` Returns a constructed instance of this content directly.

#### Integrate.ModContent.ModContent.ISL.Script

```c#
public class Script(string source, string location = "<anonymous>")
{
  public string Location;
  public string[] GetMetadata(string tag);
  public void Compile();
  public void Execute();
}
```

`Location` Original file path of the script. Equivalent to `Content.name`.
`GetMetadata()` Returns an array of values of the specified tag, or an empty array if there are none.  
`Compile()` Forces the program to be 'compiled' earlier. THis is automatically called if any member other than `Location` is invoked.  
`Execute()` Executes the script's ISL program. Events will invoke this themelves, and will specify inputs.

#### Integrate.ModContent.Mod

```c#
public class Mod
{
  public string DisplayName { get; init; }
  public string Name { get; init; }
  public string Version { get; init; }
  public string Author { get; init; }
  public string Tagline { get; init; }
  public string Description { get; init; }
  public Content[] content = [];
  public Script[] scripts = [];
  public string Describe();
}
```

`DisplayName` Display name of the mod.  
`Name` Internal ID for the mod. Used for registry items.  
`Version` Mod version.  
`Author` Who made this mod.  
`Tagline` Short, one-line description of the mod.  
`Description` Longer description of the mod.  
`content` Array of all content in this mod.  
`scripts` Array of all scripts in this mod.

#### Integrate.Registry.Registry

```c#
public class Registry<T> where T : notnull
{
    private readonly Dictionary<string, T> content;
    public int Size;
    public void Add(string name, T item);
    public bool Has(string name);
    public T Get(string name);
    public void Rename(string name, string newName);
    public void Alias(string name, string otherName);
    public void ForEach(Action<string, T> action);
    public async void ForEachAsync(Action<string, T> action);
    public T At(int index);
    public static bool IsValidName(string name);
}
```

`size` Returns the size of the registry.  
`Add()` Adds an item to registry.  
`Has()` Checks for an item in registry.
`Get()` Gets an item from registry name.  
`Rename()` Renames a registry item. Neither parameter is case-sensitive.  
`Alias()` Adds another registry item with the same content as the specified one.  
`ForEach()` Executes a function for each element in the registry.  
`ForEachAsync()` Executes a function for each element in the registry asynchronously.  
`At()` Returns an item at the Nth index of the Registry.  
`IsValidName()` Checks if the provided string would be a valid registry name.

##### Not-yet-implemented:

`Create()` Constructs an item from this registry. Note that this only works with object entries. The parameter `registry` should be the registry holding all types, such as `ModLoader.types`.  
`Construct()` Constructs an item using a type from this registry. Note that this only works with object parameters.  
`NameOf()` Searches the registry for any entries with matching content. Equivalence follows `==` rules.

## Module: ISLTest

Targets: **Mod Developers**, **ISL Contributors**  
A command-line interface for running simple ISL programs. Doesn't allow input customisation, but can load from a file or take input directly.  
Mostly to help beginners learn the syntax of ISL.  
Run

```shell
ISLTest -h
```

for help.

## Module: ISLGui

Targets: **Mod Developers**  
A graphical interface for developing ISL, in the form of an IDE (Integrated Development Environment).  
It provides multi-file support through a tabbed sidebar, an integrated runtime with an input variable customiser and a rich code editor with syntax highlighing accurate to the interpreter.  
It's most helpful for mod developers, but it can be helpful to check extension syntax.

## Module: IntegrateTest

Targets: **Project Developers**, **Integrate Contributors**  
Simply a CLI tester for the Integrate modloader. Run it, and it provides debug information for one instance of most Integrate operations. Not very useful for mod developers, but may help in developing the projects to mod.
