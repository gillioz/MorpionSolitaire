namespace MorpionSolitaire;

public class Segment
{
    public List<ImagePoint> LineImage { get; }
    public ImagePoint DotImage { get; }
    public List<ImagePoint> SupportDotsImage { get; }
    public GridLine Line { get; }
    public GridDot Dot { get; }

    public Segment(GridDot dot, GridLine line, ImagePoint dotImage,
        List<ImagePoint> supportDotsImage, List<ImagePoint> lineImage)
    {
        Dot = dot;
        Line = line;
        DotImage = dotImage;
        SupportDotsImage = supportDotsImage;
        LineImage = lineImage;
    }

    public GridAction ToGridAction()
    {
        var gridAction = new GridAction();
        gridAction.Add(Line);
        gridAction.Add(Dot);
        return gridAction;
    }

    public ImageAction ToImageAction()
    {
        var imageAction = new ImageAction();
        imageAction.Pixels.Add(DotImage);
        foreach (var pixel in LineImage)
        {
            imageAction.Pixels.Add(pixel);
        }

        return imageAction;
    }

    private bool Equals(Segment other)
    {
        return Line == other.Line;
    }

    public override bool Equals(object? obj)
    {
        return obj is Segment other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Line.GetHashCode();
    }

    public static bool operator ==(Segment s1, Segment s2)
    {
        return s1.Equals(s2);
    }

    public static bool operator !=(Segment s1, Segment s2)
    {
        return !s1.Equals(s2);
    }
}