using Microsoft.VisualBasic.CompilerServices;

namespace MorpionSolitaire;

public class Segment
{
    public List<ImageCoordinates> LineImage { get; }
    public ImageCoordinates DotImage { get; }
    public List<ImageCoordinates> SupportDotsImage { get; }
    public GridLine Line { get; }
    public GridDot Dot { get; }

    public Segment(GridDot dot, GridLine line, ImageCoordinates dotImage,
        List<ImageCoordinates> supportDotsImage, List<ImageCoordinates> lineImage)
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
    
    public static bool operator ==(Segment s1, Segment s2)
    {
        return s1.Line == s2.Line;
    }

    public static bool operator !=(Segment s1, Segment s2)
    {
        return s1.Line != s2.Line;
    }
}