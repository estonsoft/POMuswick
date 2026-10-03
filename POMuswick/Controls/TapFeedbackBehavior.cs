namespace POMuswick.Controls;

public sealed class TapFeedbackBehavior : Behavior<View>
{
    private readonly List<TapGestureRecognizer> _tapRecognizers = new();
    private View? _target;
    private bool _isAnimating;

    protected override void OnAttachedTo(View bindable)
    {
        base.OnAttachedTo(bindable);
        _target = bindable;
        bindable.HandlerChanged += OnHandlerChanged;
        Subscribe();
    }

    private void OnHandlerChanged(object? sender, EventArgs e) => Subscribe();

    // Recognizers may be added after the behavior attaches (XAML order), so re-scan.
    private void Subscribe()
    {
        if (_target is null)
            return;

        foreach (var recognizer in _target.GestureRecognizers.OfType<TapGestureRecognizer>())
        {
            if (_tapRecognizers.Contains(recognizer))
                continue;

            recognizer.Tapped += OnTapped;
            _tapRecognizers.Add(recognizer);
        }
    }

    protected override void OnDetachingFrom(View bindable)
    {
        bindable.HandlerChanged -= OnHandlerChanged;
        foreach (var recognizer in _tapRecognizers)
            recognizer.Tapped -= OnTapped;

        _tapRecognizers.Clear();
        _target = null;
        base.OnDetachingFrom(bindable);
    }

    private async void OnTapped(object? sender, TappedEventArgs args)
    {
        if (_target is not { } target || _isAnimating)
            return;

        _isAnimating = true;
        var originalScale = target.Scale;
        var originalOpacity = target.Opacity;
        try
        {
            // Applied synchronously so it shows even when the command navigates immediately.
            target.Scale = originalScale * 0.96;
            target.Opacity = originalOpacity * 0.7;
            await Task.Delay(90);
            await Task.WhenAll(
                target.ScaleToAsync(originalScale, 180, Easing.CubicOut),
                target.FadeToAsync(originalOpacity, 180, Easing.CubicOut));
        }
        catch (OperationCanceledException)
        {
            target.Scale = originalScale;
            target.Opacity = originalOpacity;
        }
        finally
        {
            _isAnimating = false;
        }
    }
}