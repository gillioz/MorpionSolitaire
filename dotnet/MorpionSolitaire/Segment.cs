using System.Diagnostics;

namespace MorpionSolitaire;

public class Segment
{
    private List<ImageCoordinates> _lineImage { get; }
    private ImageCoordinates _dotImage { get; }
    private List<ImageCoordinates> _supportDotsImage { get; }
    private GridLine _line { get; }
    public GridDot _dot { get; }

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

        _lineImage = new List<ImageCoordinates>();
        _supportDotsImage = new List<ImageCoordinates>();

        var emptyDotSet = false;
        var pt0 = new ImageCoordinates(pt1);
        for (int i = iMin; i < iMax; i++)
        {
            var pt = new ImageCoordinates(pt0.X + i * dx, pt0.Y + i * dy);
            if (pt.IsDot())
            {
                if (image.Get(pt))
                {
                    _supportDotsImage.Add(pt);
                }
                else
                {
                    if (emptyDotSet)
                    {
                        throw new Exception("The segment does not have enough support dots.");
                    }

                    emptyDotSet = true;
                    _dotImage = pt;
                    _dot = new GridDot(pt.ToGridCoordinates());
                }
            }
            else
            {
                if (image.Get(pt))
                {
                    throw new Exception("The segment cannot overlap existing lines.");
                }

                _lineImage.Add(pt);
            }
        }

        if (!emptyDotSet || _dot is null)
        {
            throw new Exception("The segment does not go through any empty dot.");
        }

        if (newPt is not null && !_dot.Pt.Equals(newPt))
        {
            throw new Exception("Invalid segment.");
        }
        
        _line = new GridLine(pt1, pt2);
    }

    public GridAction ToGridAction()
    {
        var gridAction = new GridAction();
        gridAction.Add(_line);
        gridAction.Add(_dot);
        return gridAction;
    }

    public ImageAction ToImageAction()
    {
        var imageAction = new ImageAction();
        imageAction.Pixels.Add(_dotImage);
        foreach (var pixel in _lineImage)
        {
            imageAction.Pixels.Add(pixel);
        }

        return imageAction;
    }
}