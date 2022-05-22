using Orditor.Orchestration;

namespace Orditor.Model;

internal class AreasOri
{
  public AreasOri(FileManager file, Messenger messenger)
  {
    _file = file;
    _messenger = messenger;
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
        _messenger.ChangeAreas();
      }
    }
  }

  public void SetLocation(Home home, int x, int y)
  {
    home.SetLocation(x, y);
    RawText = LineParser.SetLocation(_rawText, home.Name, x, y);
  }
  
  private readonly FileManager _file;
  private readonly Messenger _messenger;
  private string _rawText;
}