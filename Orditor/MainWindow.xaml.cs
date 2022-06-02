using System.Linq;
using System.Windows;
using NLog;
using NLog.Config;
using NLog.Targets;
using Orditor.Model;
using Orditor.Orchestration;
using Orditor.Properties;
using Orditor.Reachability;
using Orditor.ViewModels;

namespace Orditor;

internal partial class MainWindow
{
  public MainWindow()
  {
    InitializeComponent();

    SetupLogging();

    var file = new FileManager(Settings.Default);
    if (!file.Valid)
    {
      Application.Current.Shutdown();
      return;
    }

    var annotations = new Annotations();
    var parser = new PickupGraphParser(annotations);
    InitializeLocations(parser, file);

    var messenger = new Messenger();
    var areas = new AreasOri(file, messenger);
    var areasEditor = new AreasEditorViewModel(areas, messenger);
    messenger.Listen(areasEditor);

    var graph = new RestrictedGraph(areas, parser, messenger);
    messenger.Listen((IAreasListener)graph);
    messenger.Listen((IInventoryListener)graph);

    var world = new WorldViewModel(graph, messenger, areas);

    var originSelector = new OriginSelectorViewModel(graph);
    messenger.Listen((ISelectionListener)originSelector);
    messenger.Listen((IRestrictedGraphListener)originSelector);

    var inventory = new InventoryViewModel(messenger, originSelector);

    WorldView.DataContext = world;
    AreasEditorView.DataContext = areasEditor;
    InventoryView.DataContext = inventory;
  }

  private static void SetupLogging()
  {
    var config = new LoggingConfiguration();
    var logfile = new FileTarget("logfile") { FileName = "${specialfolder:folder=LocalApplicationData}/Orditor/log.txt" };
    config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
    LogManager.Configuration = config;
  }

  private static void InitializeLocations(PickupGraphParser parser, FileManager file)
  {
    var text = file.Areas;
    var graph = parser.Parse(text);
    SaveLocations(graph, text, file);
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