using ISL;
using ISL.Interpreter;
using ISL.Language.Types;
using ISL.Language.Types.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;

namespace ISLGui
{
    public readonly struct InVariable
    {
        public InVariable() { }
        public string Name { get; init; } = "";
        public IslType Type { get; init; } = IslType.Null;
        public IslValue Value { get; init; } = IslValue.Null;
    }
    internal class VariableManager
    {
        private readonly List<InVariable> ins = [];
        public void RemoveAllVars()
        {
            ins.Clear();
        }
        public void AddVariable(string type, string name, string value)
        {
            if (type is null || type.Length == 0) return;
            var def = IslInterface.GetNativeIslValue(type);
            var itype = def.Type;
            if (name.Trim().Length == 0) return;
            if (value.Trim().Length == 0) value = def.Stringify();
            try
            {
                ins.Add(new()
                {
                    Name = name,
                    Type = itype,
                    Value = AnyFromString(value, itype)
                });
            }
            catch (Exception e)
            {
                ins.Add(new() { Name = e.Message, Value = new IslString(e.GetType().Name) });
            }
        }
        public void AddButtonsToListView(ListView list)
        {
            list.Items.Clear();
            for (int index = 0; index < ins.Count; index++)
            {
                var variable = ins[index];
                var container = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new(-3),
                    Margin = new(-3)
                };
                var textBlock = new TextBlock
                {
                    Text = $"({variable.Type}) {variable.Name} = {variable.Value}",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Height = 28,
                    FontSize = 12,
                    Padding = new(5),
                    FontFamily = new FontFamily("Consolas")
                };
                var symbol = new SymbolIcon(Symbol.Clear)
                {
                    Margin = new(-5, 0, -5, 0)
                };
                var removeButton = new Button
                {
                    Content = symbol,
                    FontSize = 6,
                    Height = 28,
                    Width = 36
                };
                symbol = new SymbolIcon(Symbol.Copy)
                {
                    Margin = new(-5, 0, -5, 0)
                };
                var importButton = new Button
                {
                    Content = symbol,
                    FontSize = 6,
                    Height = 28,
                    Width = 36,
                    Padding = new(0)
                };
                ToolTipService.SetToolTip(removeButton, new ToolTip() { Content = "Remove " + variable.Name + " from list" });
                ToolTipService.SetToolTip(importButton, new ToolTip() { Content = "Copy ISL import statement for " + variable.Name });
                int i = index;
                removeButton.Click += (s, e) =>
                {
                    ins.RemoveAt(i);
                    AddButtonsToListView(list);
                };
                importButton.Click += (s, e) =>
                {
                    Copy(GetIslImportStatement(variable));
                    VarButtonClicked.Invoke(variable);
                };
                container.Margin = new Thickness(-5, 1, -5, 1);
                container.Children.Add(textBlock);
                container.Children.Add(removeButton);
                container.Children.Add(importButton);
                list.Items.Add(container);
            }
        }
        public void AddInsToProgram(IslProgram prog)
        {
            ins.ForEach(i => prog.AddInput(i.Name, i.Value));
        }
        public void CopyAllImports()
        {
            string import = "";
            ins.ForEach(v => import += (GetIslImportStatement(v) + '\n'));
            Copy(import);
        }
        private static void Copy(string text)
        {
            DataPackage dataPackage = new()
            {
                RequestedOperation = DataPackageOperation.Copy
            };
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }
        private static string GetIslImportStatement(InVariable variable)
        {
            return $"in {variable.Type.ToString().ToLower()} {variable.Name};";
        }
        public delegate void VariableButtonEventHandler(InVariable var);
        public event VariableButtonEventHandler VarButtonClicked = (v) => { };

        public static IslValue AnyFromString(string str, IslType type)
        {
            return type switch
            {
                IslType.Null => IslValue.Null,
                IslType.Int => IslInt.FromString(str),
                IslType.Float => IslFloat.FromString(str),
                IslType.Complex => IslComplex.FromString(str),
                IslType.String => IslString.FromString($"\"{str}\""),
                IslType.Group => IslGroup.FromString(str),
                IslType.Bool => IslBool.FromString(str),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
