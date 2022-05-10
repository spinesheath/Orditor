using Orditor.ViewModels;

namespace Orditor;

internal partial class MainWindow
{
  public MainWindow()
  {
    InitializeComponent();

    var vm = new WorldViewModel();
    WorldView.DataContext = vm;
  }
}