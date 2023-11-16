using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Navigation;
using NLog;
using NLog.Config;
using NLog.Targets;
using Orditor.Model;
using Orditor.Orchestration;
using Orditor.Properties;
using Orditor.Reachability;
using Orditor.ViewModels;

namespace Orditor;

// ReSharper disable once UnusedMember.Global
internal partial class MainWindow : IInventoryListener
{
  static MainWindow()
  {
    HttpClient = new HttpClient();
    HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    HttpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");
  }

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

    CheckForUpdateAsync();

    try
    {
      var file = new FileManager(Settings.Default);
      if (!file.Valid)
      {
        Logger.Info("No valid areas file");
        Application.Current.Shutdown();
        return;
      }

      LoadFromFile(file);
      ReloadButton.Click += OnReload;
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

  private const string LatestReleaseUrl = "https://api.github.com/repos/spinesheath/Orditor/releases/latest";
  private static readonly HttpClient HttpClient;
  private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
  private static readonly Regex VersionRegex = new(@"(\d+)(?:\.(\d+))?(?:\.(\d+))?(?:\.(\d+))?");

  private async void CheckForUpdateAsync()
  {
    try
    {
      var version = Assembly.GetEntryAssembly()?.GetName().Version;
      if (version == null)
      {
        return;
      }

      var response = await HttpClient.GetAsync(LatestReleaseUrl);
      response.EnsureSuccessStatusCode();
      var release = await response.Content.ReadFromJsonAsync<LatestReleaseResult>();
      if (release == null)
      {
        return;
      }

      if (CanUpdate(release, version))
      {
        LatestReleaseHyperlink.NavigateUri = new Uri(release.HtmlUrl);
        LatestReleaseTextBlock.Visibility = Visibility.Visible;
      }
    }
    catch
    {
      Logger.Info("Update check failed");
    }
  }

  private static bool CanUpdate(LatestReleaseResult release, Version version)
  {
    var match = VersionRegex.Match(release.TagName);
    return match.Success && Version.TryParse(match.Value, out var latestVersion) && latestVersion > version;
  }

  private void LoadFromFile(FileManager file)
  {
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

  private void OnReload(object sender, RoutedEventArgs e)
  {
    var file = new FileManager(Settings.Default, true);
    if (file.Valid)
    {
      LoadFromFile(file);
    }
  }

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

  private void OnNavigateToLatestRelease(object sender, RequestNavigateEventArgs e)
  {
    try
    {
      var processStartInfo = new ProcessStartInfo(e.Uri.AbsoluteUri)
      {
        UseShellExecute = true,
        Verb = "open"
      };
      Process.Start(processStartInfo);
      e.Handled = true;
    }
    catch
    {
      Logger.Warn("Failed to navigate to release");
    }
  }

  // ReSharper disable once ClassNeverInstantiated.Local
  private class LatestReleaseResult
  {
    [JsonPropertyName("html_url")]
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    public string HtmlUrl { get; set; } = "";

    [JsonPropertyName("tag_name")]
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    public string TagName { get; set; } = "";
  }
}