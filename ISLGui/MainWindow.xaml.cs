using System;
using System.IO;
using System.Threading.Tasks;
using ISL;
using ISL.Interpreter;
using ISL.Language.Types;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using Windows.UI;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ISLGui
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private static readonly string defaultFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ISL\";

        private readonly FileManager manager = new();
        private readonly VariableManager varManager = new();
        private readonly Highlighter islHighlighter = new();
        public MainWindow(string initFilePath)
        {
            this.InitializeComponent();

            //Set title bar icon (broken)
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                IntPtr hWnd = WindowNative.GetWindowHandle(this);
                WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
                AppWindow appWindow = AppWindow.GetFromWindowId(wndId);
                appWindow.SetIcon(@"Assets\infinity-icon.ico");
            }
            //Set highlight colors
            islHighlighter.SetSyntaxColor(TokenType.Numeric, Color.FromArgb(255, 157, 182, 131));
            islHighlighter.SetSyntaxColor(TokenType.String, Color.FromArgb(255, 214, 157, 133));
            islHighlighter.SetSyntaxColor(TokenType.Identifier, Color.FromArgb(255, 139, 212, 235));
            islHighlighter.SetSyntaxColor(TokenType.Getter, Color.FromArgb(255, 214, 157, 235));
            islHighlighter.SetSyntaxColor(TokenType.Operator, Color.FromArgb(255, 128, 128, 128));
            islHighlighter.SetSyntaxColor(TokenType.SpecialOperator, Color.FromArgb(255, 71, 141, 210));
            islHighlighter.SetSyntaxColor(TokenType.Keyword, Color.FromArgb(255, 188, 155, 223));
            islHighlighter.SetSyntaxColor(TokenType.Comment, Color.FromArgb(255, 73, 107, 35));
            islHighlighter.SetSyntaxColor(TokenType.MetaTag, Color.FromArgb(255, 100, 230, 255));
            //Add events
            manager.FileButtonClicked += Manager_FileButtonClicked;
            manager.FileButtonDeleted += Manager_FileButtonDeleted;
            //Open default file, if applicable
            if (initFilePath.Length == 0)
            {
                var defaultFile = new IslFile()
                {
                    CurrentDirectory = defaultFilePath,
                };
                manager.SelectFile(manager.AddFile(defaultFile));
            }
            else
            {
                manager.SelectFile(manager.AddFile(new IslFile()
                {
                    FilePath = Path.GetFullPath(initFilePath)
                }));
            }
            manager.AddButtonsToListView(Files);
            LoadFileIntoTextbox();
            CodeMode(0, new());
        }

        private void Manager_FileButtonDeleted(int fileIndex) => LoadFileIntoTextbox();

        private void Manager_FileButtonClicked(IslFile representedFile) => LoadFileIntoTextbox();

        private void CreateFileTriggered(object sender, RoutedEventArgs e)
        {
            if (FileNameInput.Text == "")
            {
                FileNameInput.Visibility = Visibility.Visible;
                FileNameInput.Focus(FocusState.Pointer);
            }
            else
            {
                FileNameInput.Visibility = Visibility.Collapsed;
                CreateFile();
            }
            CodeMode(sender, e);
        }
        private void CreateFile()
        {
            string fn = FileNameInput.Text;
            FileNameInput.Text = "";
            if (fn.Length == 0) fn = "script";
            if (!fn.EndsWith(".isl")) fn += ".isl";
            var newFile = new IslFile()
            {
                CurrentDirectory = manager.File.CurrentDirectory,
                Dirty = true,
                FileName = fn
            };
            manager.SelectFile(manager.AddFile(newFile));
            manager.AddButtonsToListView(Files);
            LoadFileIntoTextbox();
        }

        private async Task<IslFile> OpenFile()
        {
            // Create the picker object and set options
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            // Users expect to have a filtered view of their folders depending on the scenario.
            // For example, when choosing a documents folder, restrict the filetypes to documents for your application.
            openPicker.FileTypeFilter.Add(".isl");
            InitializeWithWindow.Initialize(openPicker, WinRT.Interop.WindowNative.GetWindowHandle(this));
            var winFile = await openPicker.PickSingleFileAsync();
            if (winFile is not null && winFile.FileType == ".isl")
            {
                return new IslFile()
                {
                    FileName = winFile.Name,
                    CurrentDirectory = winFile.Path[..winFile.Path.LastIndexOf('\\')] + '\\',
                };
            }
            return manager.File;
        }

        private void SyntaxHighlightingTriggered(object sender, Microsoft.UI.Xaml.Controls.TextChangedEventArgs e)
        {
            SyntaxHighlighting();
        }
        private void SyntaxHighlighting()
        {
            string source = TextInput.Text;
            TextOutput.Inlines.Clear();
            var toks = islHighlighter.Tokenise(source);
            var runs = islHighlighter.Runify(toks);
            runs.ForEach(x => TextOutput.Inlines.Add(x));
            UpdateFileInfo();
        }

        private void SaveButtonClicked(object sender, RoutedEventArgs e)
        {
            manager.File.SaveFile(TextInput.Text);
            UpdateFileInfo();
        }
        private void RefreshButtonClicked(object sender, RoutedEventArgs e)
        {
            manager.File.LoadFile();
            LoadFileIntoTextbox();
        }
        private async void OpenButtonClicked(object sender, RoutedEventArgs e)
        {
            var added = await OpenFile();
            int i = manager.AddFile(added);
            manager.SelectFile(i);
            manager.AddButtonsToListView(Files);
            LoadFileIntoTextbox();
            CodeMode(sender, e);
        }

        private void LoadFileIntoTextbox()
        {
            TextInput.Text = manager.File.Source;
            UpdateFileInfo();
            SyntaxHighlighting();
        }
        private void UpdateFileInfo()
        {
            manager.File.Dirty = manager.File.Source != TextInput.Text;
            BottomText.Text = manager.File.FilePath + (manager.File.Dirty ? "*" : "") + (manager.File.Exists ? "" : " [Unsaved]");
            this.Title = "ISL Graphical Editor: " + manager.File.FileName + (manager.File.Dirty ? "*" : "") + (manager.File.Exists ? "" : " [Unsaved]");
        }

        private void CodeMode(object sender, RoutedEventArgs e)
        {
            CodeEditor.Visibility = Visibility.Visible;
            SetupConfigurator.Visibility = Visibility.Collapsed;
            RunWindow.Visibility = Visibility.Collapsed;
            CodeModeButton.FontWeight = FontWeights.Bold;
            SetupModeButton.FontWeight = FontWeights.Normal;
            RunModeButton.FontWeight = FontWeights.Normal;
        }

        private void SetupMode(object sender, RoutedEventArgs e)
        {
            CodeEditor.Visibility = Visibility.Collapsed;
            SetupConfigurator.Visibility = Visibility.Visible;
            RunWindow.Visibility = Visibility.Collapsed;
            CodeModeButton.FontWeight = FontWeights.Normal;
            SetupModeButton.FontWeight = FontWeights.Bold;
            RunModeButton.FontWeight = FontWeights.Normal;
        }

        private void RunMode(object sender, RoutedEventArgs e)
        {
            CodeEditor.Visibility = Visibility.Collapsed;
            SetupConfigurator.Visibility = Visibility.Collapsed;
            RunWindow.Visibility = Visibility.Visible;
            CodeModeButton.FontWeight = FontWeights.Normal;
            SetupModeButton.FontWeight = FontWeights.Normal;
            RunModeButton.FontWeight = FontWeights.Bold;
        }

        private void AddInVariable(object sender, RoutedEventArgs e)
        {
            varManager.AddVariable((string)InVarType.SelectedValue, InVarName.Text, InVarValue.Text);
            varManager.AddButtonsToListView(InVars);
        }

        private void ClearInVariables(object sender, RoutedEventArgs e)
        {
            varManager.RemoveAllVars();
            varManager.AddButtonsToListView(InVars);
        }

        private void CopyInVars(object sender, RoutedEventArgs e)
        {
            varManager.CopyAllImports();
        }

        private void RunTriggered(object sender, RoutedEventArgs e)
        {
            Run();
        }
        private IslProgram? TryCreateProgram(IslInterface iint)
        {
            try
            {
                return iint.CreateProgram(TextInput.Text, DebugToggle.IsOn);
            }
            catch { }
            finally
            {
                DebugOutput.Text = iint.LastDebug;
                ErrorOutput.Text = iint.ErrorMessage;
            }
            return null;
        }
        private void Run()
        {
            var iint = new IslInterface();
            //Setup
            DebugOutputs.Visibility = DebugToggle.IsOn ? Visibility.Visible : Visibility.Collapsed;
            var prog = TryCreateProgram(iint);
            if (prog is null) return;

            //Inputs
            varManager.AddInsToProgram(prog);

            //Run
            var finalResult = prog.SafeExecute();
            RuntimeDebugOutput.Text = iint.CompilerDebug;
            if (finalResult is IslErrorMessage im) ErrorOutput.Text = im.Value;

            //Outputs
            OutVars.Items.Clear();
            foreach (var item in prog.LastOutputs)
            {
                var container = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Height = 40
                };
                container.Children.Add(new TextBlock
                {
                    Text = $"{item.Key}: ({item.Value.Type}) {item.Value.Stringify()}",
                    Padding = new(5),
                    Margin = new(5),
                    FontFamily = new("Consolas")
                });
                OutVars.Items.Add(container);
            }

            //Metas
            Metas.Items.Clear();
            foreach (string item in prog.GetMetaTags())
            {
                var value = prog.GetMeta(item);
                var vals = value.Split(' ');
                var container = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Height = 40
                };
                container.Children.Add(new TextBlock
                {
                    Text = $"{item} {(value.Length == 0 ? "(No values)" : $"= {string.Join(", ", vals)} {(vals.Length > 1 ? $"({vals.Length} values)" : "(1 value)")}")}",
                    Padding = new(5),
                    Margin = new(5),
                    FontFamily = new("Consolas")
                });
                Metas.Items.Add(container);
            }
        }
    }
}
