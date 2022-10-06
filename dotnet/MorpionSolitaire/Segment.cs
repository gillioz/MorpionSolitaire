using Microsoft.VisualBasic.CompilerServices;

namespace MorpionSolitaire;

public class Segment
{
    private List<ImageCoordinates> LineImage { get; }
    private ImageCoordinates DotImage { get; }
    private List<ImageCoordinates> SupportDotsImage { get; }
    private GridLine Line { get; }
    private GridDot Dot { get; }

    public Segment(GridCoordinates pt1, GridCoordinates pt2,
        Image image, int length, bool noTouchingRule, GridCoordinates? newPt = null)
    {
        if (length <= 0)
        {
            throw new Exception($"Invalid segment length: {length}");
        }

        var w = pt2.X - pt1.X;
        var h = pt2.Y - pt1.Y;
        if ((w != 0 && Math.Abs(w) != length)
            || (h != 0 && Math.Abs(h) != length)
            || (w == 0 && h == 0))
        {
            throw new Exception($"No segment can be defined between the points " +
                                $"({pt1.X}, {pt1.Y}) and ({pt2.X}, {pt2.Y})");
        }

        var dx = w / length;
        var dy = h / length;
        int iMin = 0;
        int iMax = 3 * length + 1;
        if (noTouchingRule)
        {
            iMin -= 1;
            iMax += 1;
        }

        LineImage = new List<ImageCoordinates>();
        SupportDotsImage = new List<ImageCoordinates>();

        var emptyDotSet = false;
        var pt0 = new ImageCoordinates(pt1);
        for (int i = iMin; i < iMax; i++)
        {
            var pt = new ImageCoordinates(pt0.X + i * dx, pt0.Y + i * dy);
            if (pt.IsDot())
            {
                if (image.Get(pt))
                {
                    SupportDotsImage.Add(pt);
                }
                else
                {
                    if (emptyDotSet)
                    {
                        throw new Exception("The segment does not have enough support dots.");
                    }

                    emptyDotSet = true;
                    DotImage = pt;
                    Dot = new GridDot(pt.ToGridCoordinates());
                }
            }
            else
            {
                if (image.Get(pt))
                {
                    throw new Exception("The segment cannot overlap existing lines.");
                }

                LineImage.Add(pt);
            }
        }

        if (!emptyDotSet || Dot is null || DotImage is null)
        {
            throw new Exception("The segment does not go through any empty dot.");
        }

        if (newPt is not null && !Dot.Pt.Equals(newPt))
        {
            throw new Exception("Invalid segment.");
        }
        
        Line = new GridLine(pt1, pt2);
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