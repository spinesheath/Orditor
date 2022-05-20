using System.IO;
using System.Windows.Forms;
using Orditor.Properties;

namespace Orditor.Orchestration;

internal class FileManager
{
  public FileManager(Settings settings)
  {
    _settings = settings;
    if (File.Exists(settings.AreasOriPath))
    {
      Valid = true;
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
    }
  }

  public bool Valid { get; }

  public string AreasText()
  {
    return File.ReadAllText(_settings.AreasOriPath);
  }

  private readonly Settings _settings;
}