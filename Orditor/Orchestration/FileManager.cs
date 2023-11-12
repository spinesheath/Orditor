using System.IO;
using System.Reflection;
using System.Windows.Forms;
using NLog;
using Orditor.Properties;

namespace Orditor.Orchestration;

internal class FileManager
{
  public FileManager(Settings settings)
  {
    _settings = settings;
    var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    if (directoryName != null)
    {
      var localAreasPath = Path.Combine(directoryName, "areas.ori");
      if (File.Exists(localAreasPath))
      {
        _areasOriPath = localAreasPath;
        Valid = true;
        Logger.Info("Attempting to load local file {0}", _areasOriPath);
        return;
      }
    }

    if (File.Exists(settings.AreasOriPath))
    {
      Valid = true;
      Logger.Info("Attempting to load from setting {0}", settings.AreasOriPath);
      return;
    }

    var dialog = new OpenFileDialog();
    dialog.CheckFileExists = true;
    dialog.Multiselect = false;
    dialog.Title = "Choose location of areas.ori";
    dialog.FileName = "areas.ori";
    dialog.Filter = "areas|areas.ori";
    var result = dialog.ShowDialog();
    if (result == DialogResult.OK)
    {
      settings.AreasOriPath = dialog.FileName;
      settings.Save();
      Valid = true;
      Logger.Info("Attempting to load new {0}", settings.AreasOriPath);
    }
  }

  public string Areas
  {
    get => File.ReadAllText(_areasOriPath ?? _settings.AreasOriPath);
    set => File.WriteAllText(_areasOriPath ?? _settings.AreasOriPath, value);
  }

  public bool Valid { get; }
  private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
  private readonly string? _areasOriPath;
  private readonly Settings _settings;
}