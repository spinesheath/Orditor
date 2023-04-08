using System.IO;
using System.Reflection;
using System.Windows.Forms;
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
        return;
      }
    }

    if (File.Exists(settings.AreasOriPath))
    {
      Valid = true;
      return;
    }

    if (!AcceptModification())
    {
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

  private static bool AcceptModification()
  {
    const string text = "Choose an areas.ori file. It may immediately be modified.";
    const string caption = "Choose areas.ori";
    var result = MessageBox.Show(text, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
    return result == DialogResult.OK;
  }

  public bool Valid { get; }

  public string Areas
  {
    get => File.ReadAllText(_areasOriPath ?? _settings.AreasOriPath);
    set => File.WriteAllText(_areasOriPath ?? _settings.AreasOriPath, value);
  }

  private readonly Settings _settings;
  private readonly string? _areasOriPath;
}