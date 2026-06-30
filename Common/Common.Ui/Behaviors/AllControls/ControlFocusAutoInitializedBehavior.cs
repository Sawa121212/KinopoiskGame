using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Common.Ui.Behaviors.AllControls;

public class ControlFocusAutoInitializedBehavior : Behavior<Control>
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();

        if (AssociatedObject != null)
        {
            AssociatedObject.Initialized -= OnInitialized;

            AssociatedObject.GetObservable(Visual.IsVisibleProperty).Subscribe(OnVisibleChanged);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.Initialized -= OnInitialized;
            //AssociatedObject.GetObservable(Visual.IsVisibleProperty).Unsubscribe
        }

        base.OnDetachedFromVisualTree();
    }

    private void OnInitialized(object? sender, EventArgs e)
    {
        AssociatedObject?.Focus();
    }

    public void OnVisibleChanged(bool isVisible)
    {
        if (isVisible)
        {
            AssociatedObject?.Focus();
        }
    }
}