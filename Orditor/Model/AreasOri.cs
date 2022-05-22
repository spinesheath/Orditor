using Orditor.Orchestration;

namespace Orditor.Model;

internal class AreasOri
{
  public AreasOri(FileManager file, Messenger messenger)
  {
    _file = file;
    _messenger = messenger;
    _text = _file.Areas;
  }

  public string Text
  {
    get => _text;
    set
    {
      if (_text != value)
      {
        _text = value;
        _file.Areas = _text;
        _messenger.ChangeAreas();
      }
    }
  }

  public void SetLocation(Home home, int x, int y)
  {
    home.SetLocation(x, y);
    Text = LineParser.SetLocation(_text, home.Name, x, y);
  }
  
  private readonly FileManager _file;
  private readonly Messenger _messenger;
  private string _text;
}