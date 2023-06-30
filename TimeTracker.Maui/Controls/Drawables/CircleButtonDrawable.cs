namespace TimeTracker.Maui.Controls.Drawables;

public class CircleButtonDrawable : IDrawable
{
    public Color StrokeColor { get; set; } = Colors.DarkGrey;

    public bool AreShadowsEnabled { get; set; } = true;

    /// <summary>
    /// A string containing the Image name.
    /// </summary>
    public string Image { get; set; }

    public int Width { get; set; } = 0;
    public int Height { get; set; } = 0;

    public Color ButtonColor { get; set; } = Colors.White;

    public bool SetInvisible { get; set; } = false;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (SetInvisible)
            return;

        canvas.StrokeColor = StrokeColor;

        //if (AreShadowsEnabled)
        //    canvas.EnableDefaultShadow();

        //canvas.SetShadow(new SizeF(5,5), 4, Colors.Gray); 

        var width = Width != 0 ? Width : dirtyRect.Width;
        var height = Height != 0 ? Height : dirtyRect.Height;

        var limitingDim = width > height ? height : width;
        PointF centerOfCircle = new PointF(width / 2, height / 2);
        canvas.FillColor = this.ButtonColor;
        canvas.FillCircle(centerOfCircle, limitingDim / 2);

#if WINDOWS
        canvas.FillColor = this.ButtonColor;
        canvas.FillCircle(centerOfCircle, limitingDim / 2);
#endif
    }
}
