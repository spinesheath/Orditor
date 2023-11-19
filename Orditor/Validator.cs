using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using NLog;
using Orditor.Properties;

namespace Orditor;

internal class Validator
{
  public Validator(Button button, Settings settings)
  {
    _button = button;
    _settings = settings;
  }

  public async Task RunAsync()
  {
    _button.IsEnabled = false;

    try
    {
      var seedgen = new SeedgenRunner(_settings);
      var result = await seedgen.RunAsync();

      if (SeedValid(result.Seed) && SpoilerValid(result.Spoiler))
      {
        MessageBox.Show("A valid seed was generated.", "Valid", MessageBoxButton.OK, MessageBoxImage.Information);
      }
      else
      {
        MessageBox.Show("Could not generate a valid seed.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
    catch
    {
      MessageBox.Show("Could not generate a valid seed.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
    }
    
    _button.IsEnabled = true;
  }

  private static bool SeedValid(string seed)
  {
    var lines = seed.SplitToLines().ToList();
    return lines.Count == 257 && 
           lines[0].StartsWith("Standard,Clues,ForceTrees,Competitive,balanced") && 
           lines.All(line => !string.IsNullOrEmpty(line));
  }

  private static bool SpoilerValid(string spoiler)
  {
    return spoiler.StartsWith("Standard,Clues,ForceTrees,Competitive,balanced");
  }

  private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
  private readonly Button _button;
  private readonly Settings _settings;
}