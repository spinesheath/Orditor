using System.Linq;
using System.Text.Json;
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

internal partial class MainWindow : IInventoryListener
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
    var originSelector = new OriginSelectorViewModel(graph, Settings.Default.Origin);

    messenger.Listen((ISelectionListener)originSelector);
    messenger.Listen((IRestrictedGraphListener)originSelector);
    messenger.Listen(this);

    var inventory = LoadInventory();
    var inventoryViewModel = new InventoryViewModel(inventory, messenger, originSelector);

    WorldView.DataContext = world;
    AreasEditorView.DataContext = areasEditor;
    InventoryView.DataContext = inventoryViewModel;
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

  void IInventoryListener.Changed(Inventory inventory, string origin)
  {
    Settings.Default.Inventory = JsonSerializer.Serialize(inventory);
    Settings.Default.Origin = origin;
    Settings.Default.Save();
  }

  private static Inventory LoadInventory()
  {
    Inventory? inventory = null;
    if (!string.IsNullOrEmpty(Settings.Default.Inventory))
    {
      inventory = JsonSerializer.Deserialize<Inventory>(Settings.Default.Inventory);
    }

    return inventory ?? Inventory.Default();
  }
}