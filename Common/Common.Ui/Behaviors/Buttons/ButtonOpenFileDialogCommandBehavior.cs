using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using Common.Ui.Extensions;

namespace Common.Ui.Behaviors.Buttons
{
    public class ButtonOpenFileDialogCommandBehavior : Behavior<Button>
    {
        public static readonly StyledProperty<ICommand> CommandProperty =
            AvaloniaProperty.Register<ButtonOpenFileDialogCommandBehavior, ICommand>(nameof(Command), default, true);

        public ICommand Command
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject != null)
            {
                AssociatedObject.Click += OnClick;
            }
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.Click -= OnClick;
            }

            base.OnDetaching();
        }

        private void OnClick(object? sender, RoutedEventArgs e)
        {
            TopLevel? ownerWindow = TopLevel.GetTopLevel(AssociatedObject);

            if (ownerWindow is null)
            {
                return;
            }

            Command?.Invoke(ownerWindow);
        }
    }
}
