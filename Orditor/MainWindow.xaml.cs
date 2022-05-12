using Orditor.Model;
using Orditor.Orchestration;
using Orditor.ViewModels;

namespace Orditor;

internal partial class MainWindow
{
  public MainWindow()
  {
    InitializeComponent();

    var selection = new Selection();
    var world = new World();
    var connectionEditor = new ConnectionEditorViewModel(world);
    selection.Listen(connectionEditor);
    WorldView.DataContext = new WorldViewModel(world, selection);
    ConnectionEditorView.DataContext = connectionEditor;
  }
}