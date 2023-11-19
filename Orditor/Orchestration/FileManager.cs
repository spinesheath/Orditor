using System.IO;
using System.Windows.Forms;
using NLog;
using Orditor.Properties;

namespace Orditor.Orchestration;

internal class FileManager
{
  public FileManager(Settings settings, bool forceDialog = false)
  {
    _settings = settings;
    if (!forceDialog && File.Exists(settings.AreasOriPath))
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
    get => File.ReadAllText(FilePath);
    set => File.WriteAllText(FilePath, value);
  }

  public string FilePath => _settings.AreasOriPath;

  public bool Valid { get; }
  private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
  private readonly Settings _settings;
}