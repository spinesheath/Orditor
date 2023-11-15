using System;
using System.ComponentModel;
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
    if (Settings.Default.UpgradeSettings)
    {
      Settings.Default.Upgrade();
      Settings.Default.UpgradeSettings = false;
      Settings.Default.Save();
    }

    InitializeComponent();

    SetupLogging();

    try
    {
      var file = new FileManager(Settings.Default);
      if (!file.Valid)
      {
        Logger.Info("No valid areas file");
        Application.Current.Shutdown();
        return;
      }

      var parser = new PickupGraphParser();

      var messenger = new Messenger();
      var areas = new AreasOri(file, messenger);
      var areasEditor = new AreasEditorViewModel(areas, messenger);

      var inventory = LoadInventory();
      var origin = LoadOrigin();

      var graph = new RestrictedGraph(areas, parser, messenger, inventory, origin);
      var world = new WorldViewModel(graph, messenger, areas);
      var originSelector = new OriginSelectorViewModel(graph, origin);
      var inventoryViewModel = new InventoryViewModel(inventory, messenger, originSelector);

      messenger.Listen(areasEditor);
      messenger.Listen((IAreasListener)graph);
      messenger.Listen((IInventoryListener)graph);
      messenger.Listen((ISelectionListener)originSelector);
      messenger.Listen((IRestrictedGraphListener)originSelector);
      messenger.Listen(this);

      WorldView.DataContext = world;
      AreasEditorView.DataContext = areasEditor;
      InventoryView.DataContext = inventoryViewModel;
      FilePathDisplay.Text = areas.FilePath;
    }
    catch (Exception e)
    {
      Logger.Fatal(e);
      throw;
    }
  }

  void IInventoryListener.Changed(Inventory inventory, string origin)
  {
    Settings.Default.Inventory = JsonSerializer.Serialize(inventory);
    Settings.Default.Origin = origin;
    Settings.Default.Save();
  }

  private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

  protected override void OnClosing(CancelEventArgs e)
  {
    Logger.Info("Shutting down");
    base.OnClosing(e);
  }

  private static void SetupLogging()
  {
    var config = new LoggingConfiguration();
    var logfile = new FileTarget("logfile")
    {
      FileName = "${specialfolder:folder=LocalApplicationData}/Orditor/log.txt",
      ArchiveAboveSize = 5_000_000,
      MaxArchiveFiles = 2
    };
    config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
    LogManager.Configuration = config;

    Logger.Info("Starting");
  }

  private static string LoadOrigin()
  {
    return string.IsNullOrEmpty(Settings.Default.Origin) ? "SunkenGladesRunaway" : Settings.Default.Origin;
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