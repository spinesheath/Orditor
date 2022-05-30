using System.Windows;
using System.Windows.Controls;

namespace Orditor.Controls;

internal class LogicSetButton : ItemsControl
{
  static LogicSetButton()
  {
    DefaultStyleKeyProperty.OverrideMetadata(typeof(LogicSetButton), new FrameworkPropertyMetadata(typeof(LogicSetButton)));
  }

  public bool IsChecked
  {
    get => (bool)GetValue(IsCheckedProperty);
    set => SetValue(IsCheckedProperty, value);
  }

  public object Title
  {
    get => GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
  }

  public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
    nameof(IsChecked), typeof(bool), typeof(LogicSetButton), 
    new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

  public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
    nameof(Title), typeof(object), typeof(LogicSetButton), new PropertyMetadata(default(object)));
}