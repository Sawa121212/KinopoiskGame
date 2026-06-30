using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace Game.Ui.Views.Controls;

public class SunburstBackground : Control
{
    public static readonly StyledProperty<double> RotationAngleProperty =
        AvaloniaProperty.Register<SunburstBackground, double>(nameof(RotationAngle));

    private Geometry _raysGeometry;
    private Point _lastCenter;
    private double _lastRadius;
    private CancellationTokenSource _animationCts;

    public double RotationAngle
    {
        get => GetValue(RotationAngleProperty);
        set => SetValue(RotationAngleProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        //StartAnimation();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        StopAnimation();
    }

    private void StartAnimation()
    {
        _animationCts?.Cancel();
        _animationCts = new CancellationTokenSource();

        Animation animation = new()
        {
            Duration = TimeSpan.FromSeconds(50.0),
            IterationCount = IterationCount.Infinite,
            FillMode = FillMode.Forward,
            PlaybackDirection = PlaybackDirection.Normal,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0),
                    Setters =
                    {
                        new Setter(RotateTransform.AngleProperty, 0.0)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1),
                    Setters =
                    {
                        new Setter(RotateTransform.AngleProperty, 360.0)
                    }
                }
            }
        };

        this.Styles.Add(new Style()
        {
            Animations =
            {
                animation
            }
        });
    }

    private void StopAnimation()
    {
        _animationCts?.Cancel();
        _animationCts = null;
    }

    public override void Render(DrawingContext context) // Изменили OnDraw на Render
    {
        Point center = new(Bounds.Width / 2, Bounds.Height / 2);
        double radius = Math.Max(Bounds.Width, Bounds.Height) * 1.5;

        // Пересоздаем геометрию только при изменении размеров
        if (_raysGeometry == null || center != _lastCenter || radius != _lastRadius)
        {
            StreamGeometry geometry = new();

            using (StreamGeometryContext ctx = geometry.Open())
            {
                for (int i = 0; i < 24; i++)
                {
                    int angle = i * 15;

                    Point endPoint = new(
                        center.X + radius * Math.Sin(angle * Math.PI / 180),
                        center.Y - radius * Math.Cos(angle * Math.PI / 180));

                    ctx.BeginFigure(center, false);
                    ctx.LineTo(endPoint);
                    ctx.EndFigure(false);
                }
            }

            _raysGeometry = geometry;
            _lastCenter = center;
            _lastRadius = radius;
        }

        // В Avalonia используется PushPostTransform/PushPreTransform
        using (context.PushPostTransform(Matrix.CreateTranslation(-center.X, -center.Y)
                                         * Matrix.CreateRotation(RotationAngle * Math.PI / 180)
                                         * Matrix.CreateTranslation(center.X, center.Y)))
        {
            for (int i = 0; i < 24; i++)
            {
                Pen pen = new(i % 2 == 0 ? Brushes.OrangeRed : Brushes.Orange)
                {
                    Thickness = 10,
                    LineCap = PenLineCap.Round
                };
                context.DrawGeometry(null, pen, _raysGeometry);
            }
        }

        // Не нужно явно вызывать PopTransform - using автоматически удалит трансформацию
    }
}
