namespace MorpionSolitaire;

public class Segment : GameAction
{
    public List<ImageCoordinates> SupportPixels { get; }
    
    public Segment(Image image,
        GridCoordinates pt1, GridCoordinates pt2,
        int length, bool noTouchingRule)
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
        int imin = 0;
        int imax = 3 * length + 1;
        if (noTouchingRule)
        {
            imin -= 1;
            imax += 1;
        }

        var pt0 = new ImageCoordinates(pt1);
        var pixels = new List<ImageCoordinates>();
        for (int i = imin; i < imax; i++)
        {
            pixels.Add(new ImageCoordinates(pt0.X + i * dx, pt0.Y + i * dy));
        }

        int emptyDotCount = 0;
        var emptyDot = new ImageCoordinates(0, 0);
        var supportDots = new List<ImageCoordinates>();
        foreach (ImageCoordinates pt in pixels)
        {
            if (pt.IsDot())
            {
                if (!image.Get(pt))
                {
                    emptyDotCount += 1;
                    emptyDot = pt;
                }
                else
                {
                    supportDots.Add(pt);
                }
            }
            else
            {
                if (image.Get(pt))
                {
                    throw new Exception("The segment cannot overlap existing lines");
                }
            }
        }

        if (emptyDotCount != 1)
        {
            throw new Exception($"The segment must go through {length} existing dots exactly");
        }
        
        ImageAction.Pixels.Add(emptyDot);
        foreach (ImageCoordinates pt in pixels)
        {
            if (!pt.IsDot())
            {
                ImageAction.Pixels.Add(pt);
            }
        }
        GridAction.Add(new GridDot(emptyDot.ToGridCoordinates()));
        GridAction.Add(new GridLine(pt1, pt2));
        SupportPixels = supportDots;
    }
}