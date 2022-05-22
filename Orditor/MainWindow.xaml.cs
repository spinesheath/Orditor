using System.Linq;
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
    var annotations = new Annotations();
    var parser = new PickupGraphParser(text, annotations);
    var graph = parser.Graph;
    SaveLocations(graph, text, file);

    var areas = new AreasOri(file);

    var selection = new Messenger();
    var connectionEditor = new ConnectionEditorViewModel(areas, selection);
    selection.Listen(connectionEditor);
    WorldView.DataContext = new WorldViewModel(graph, areas, selection);
    ConnectionEditorView.DataContext = connectionEditor;
  }

  private static void SaveLocations(PickupGraph graph, string text, FileManager file)
  {
    var modified = graph.Homes.Aggregate(text, SetLocation);
    if (modified != text)
    {
      file.Areas = modified;
    }
  }

  private static string SetLocation(string current, Home home)
  {
    if (home.X != int.MaxValue && home.Y != int.MaxValue)
    {
      return LineParser.SetLocation(current, home.Name, home.X, home.Y);
    }

    return current;
  }
}