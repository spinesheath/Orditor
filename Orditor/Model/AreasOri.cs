using Orditor.Orchestration;

namespace Orditor.Model;

internal class AreasOri
{
  public AreasOri(FileManager file)
  {
    _file = file;
    _rawText = _file.Areas;
  }

  public string RawText
  {
    get => _rawText;
    set
    {
      if (_rawText != value)
      {
        _rawText = value;
        _file.Areas = _rawText;
      }
    }
  }

  public void SetLocation(Home home, int x, int y)
  {
    RawText = LineParser.SetLocation(_rawText, home.Name, x, y);
    home.SetLocation(x, y);
  }
  
  private readonly FileManager _file;
  private string _rawText;
}