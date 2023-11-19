using System;
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
    catch (Exception e)
    {
      Logger.Error(e);
    }
    
    _button.IsEnabled = true;
  }

  private bool SeedValid(string seed)
  {
    return seed.Length > 0;
  }

  private bool SpoilerValid(string spoiler)
  {
    return spoiler.Length > 0;
  }

  private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
  private readonly Button _button;
  private readonly Settings _settings;
}