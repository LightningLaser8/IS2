using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ISLGui
{
    internal class IslFile
    {
        public IslFile() { }
        public bool Dirty { get; set; } = false;
        public bool Exists => File.Exists(FilePath);
        public string FilePath
        {
            get => CurrentDirectory + FileName; init
            {
                CurrentDirectory = value[..value.LastIndexOf('\\')] + '\\';
                FileName = value[(value.LastIndexOf('\\') + 1)..];
            }
        }
        public string CurrentDirectory { get; init; } = "\\";
        public string FileName { get; init; } = "script.isl";
        public string Error { get; private set; } = "";

        private string cache = "";
        public string Source
        {
            get
            {
                if (cache.Length > 0) return cache;
                if (!Exists) return cache;
                cache = LoadFile();
                return cache;
            }
        }

        private bool CheckDir()
        {
            try
            {
                Directory.CreateDirectory(CurrentDirectory);
                if (!File.Exists(FilePath))
                    File.Create(FilePath).Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void SaveFile(string source)
        {
            try
            {
                if (File.Exists(FilePath)) File.Delete(FilePath);
                if (!CheckDir()) return;
                using var file = File.OpenWrite(FilePath);
                file.Position = 0;
                file.Write(Encoding.UTF8.GetBytes(source));
                file.Close();
                Error = "";
                cache = source;
            }
            catch (Exception e)
            {
                Error = e.Message;
            }
        }
        public string LoadFile()
        {
            try
            {
                if (!CheckDir()) return "";
                using var file = File.OpenRead(FilePath);
                byte[] buf = new byte[file.Length];
                _ = file.Read(buf, 0, (int)file.Length);
                var stuff = Encoding.UTF8.GetString(buf);
                file.Close();
                cache = stuff;
                Error = "";
                return stuff;
            }
            catch (Exception e)
            {
                Error = e.Message;
                return "";
            }
        }
    }

    internal class FileManager
    {
        public IslFile File
        {
            get
            {
                if (selected < 0 || selected >= files.Count) selected = 0;
                return files[selected];
            }
        }

        private int selected = 0;
        private readonly List<IslFile> files = [];

        public void AddButtonsToListView(ListView list)
        {
            list.Items.Clear();
            for (int index = 0; index < files.Count; index++)
            {
                IslFile? file = files[index];
                var container = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new(-3),
                    Margin = new(-3)
                };
                var buttonControl = new Button
                {
                    Content = file.FileName.Length > 12 ? file.FileName[..12] + "…" : file.FileName,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 90,
                    Height = 28,
                    FontSize = 12,
                    Padding = new(5)
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
                    Width = 36,
                };
                ToolTipService.SetToolTip(buttonControl, new ToolTip() { Content = "Edit file " + file.FileName });
                ToolTipService.SetToolTip(removeButton, new ToolTip() { Content = "Remove " + file.FileName + " from list" });
                int i = index;
                buttonControl.Click += (s, e) =>
                {
                    selected = i;
                    ResetButtonColours(list);
                    FileButtonClicked.Invoke(file);
                };
                removeButton.Click += (s, e) =>
                {
                    if (files.Count > 1)
                    {
                        files.RemoveAt(i);
                        AddButtonsToListView(list);
                        selected = 0;
                        FileButtonDeleted.Invoke(i);
                    }
                };
                container.Margin = new Thickness(-5, 1, -5, 1);
                container.Children.Add(buttonControl);
                container.Children.Add(removeButton);
                list.Items.Add(container);
                ResetButtonColours(list);
            }
        }
        private void ResetButtonColours(ListView list)
        {
            var items = list.Items;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item is StackPanel panel)
                {
                    if (panel.Children[0] is Button but)
                    {
                        if (i == selected)
                            but.FontWeight = FontWeights.Bold;
                        else
                            but.FontWeight = FontWeights.Normal;
                    }
                }
            }
        }
        public int AddFile(IslFile file)
        {
            if (files.Contains(file)) return files.IndexOf(file);
            files.Add(file);
            return files.Count - 1;
        }
        public void SelectFile(int file)
        {
            selected = file;
        }
        public delegate void FileButtonEventHandler(IslFile representedFile);
        public delegate void ButtonDeleteEventHandler(int fileIndex);
        public event FileButtonEventHandler FileButtonClicked = (f) => { };
        public event ButtonDeleteEventHandler FileButtonDeleted = (i) => { };
    }
}
