using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Orditor.Properties;

namespace Orditor;

internal record SeedgenResult(string Seed, string Spoiler);

internal class SeedgenRunner
{
  public SeedgenRunner(Settings settings)
  {
    _settings = settings;
  }

  public async Task<SeedgenResult> RunAsync()
  {
    PreparePythonPath();
    PrepareSeedgenPath();

    if (!File.Exists(_settings.PythonPath) || !File.Exists(_settings.SeedgenPath) || !File.Exists(_settings.AreasOriPath))
    {
      throw new InvalidOperationException("Could run validation");
    }

    var dir = CreateTemporaryDirectory();

    const string seedSettings = "--preset standard --keymode clues --balanced --competitive --force-trees";
    var args = $"{_settings.SeedgenPath} --areas-ori-path {_settings.AreasOriPath} --output-dir {dir} {seedSettings}";
    await RunProcessAsync(_settings.PythonPath, args);

    var seed = await File.ReadAllTextAsync(Path.Combine(dir, "randomizer0.dat"));
    var spoiler = await File.ReadAllTextAsync(Path.Combine(dir, "spoiler0.txt"));

    return new SeedgenResult(seed, spoiler);
  }

  private readonly Settings _settings;

  private static string CreateTemporaryDirectory()
  {
    var dir = Path.Combine(Path.GetTempPath(), "Orditor_EC0382369EA14E8B87E286BA86A5E290");
    if (!Directory.Exists(dir))
    {
      Directory.CreateDirectory(dir);
    }

    File.Delete(Path.Combine(dir, "randomizer0.dat"));
    File.Delete(Path.Combine(dir, "spoiler0.txt"));

    return dir;
  }

  private static Task<int> RunProcessAsync(string fileName, string args)
  {
    var taskCompletionSource = new TaskCompletionSource<int>();

    var process = new Process
    {
      StartInfo =
      {
        FileName = fileName,
        Arguments = args,
        UseShellExecute = false,
        CreateNoWindow = true,
        RedirectStandardError = true,
        RedirectStandardOutput = true
      },
      EnableRaisingEvents = true
    };

    process.Exited += (_, _) =>
    {
      taskCompletionSource.SetResult(process.ExitCode);
      process.Dispose();
    };

    process.Start();

    return taskCompletionSource.Task;
  }

  private void PreparePythonPath()
  {
    if (File.Exists(_settings.PythonPath))
    {
      return;
    }

    var dialog = new OpenFileDialog();
    dialog.CheckFileExists = true;
    dialog.Multiselect = false;
    dialog.Title = "Choose location of python 3";
    dialog.FileName = "python.exe";
    dialog.Filter = "python 3|python.exe";
    var result = dialog.ShowDialog();
    if (result == DialogResult.OK)
    {
      _settings.PythonPath = dialog.FileName;
      _settings.Save();
    }
  }

  private void PrepareSeedgenPath()
  {
    if (File.Exists(_settings.SeedgenPath))
    {
      return;
    }

    var dialog = new OpenFileDialog();
    dialog.CheckFileExists = true;
    dialog.Multiselect = false;
    dialog.Title = "Choose location of seedgen cli";
    dialog.FileName = "cli_gen.py";
    dialog.Filter = "seedgen cli|cli_gen.py";
    var result = dialog.ShowDialog();
    if (result == DialogResult.OK)
    {
      _settings.SeedgenPath = dialog.FileName;
      _settings.Save();
    }
  }
}