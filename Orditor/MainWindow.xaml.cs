using System.Windows;
using Orditor.Model;
using Orditor.Orchestration;
using Orditor.Properties;
using Orditor.ViewModels;

namespace Orditor;

internal partial class MainWindow
{
  public MainWindow()
  {
    InitializeComponent();

    var file = new FileManager(Settings.Default);
    if (!file.Valid)
    {
      Application.Current.Shutdown();
      return;
    }

    var text = file.Areas;
    var parser = new PickupGraphParser(text);
    var world = new World(text, parser.Graph);

    var selection = new Selection();
    var connectionEditor = new ConnectionEditorViewModel(world, selection);
    selection.Listen(connectionEditor);
    WorldView.DataContext = new WorldViewModel(world, selection);
    ConnectionEditorView.DataContext = connectionEditor;
  }
}