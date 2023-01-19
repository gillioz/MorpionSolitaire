namespace MorpionSolitaire;

public class Game
{
    public Grid Grid { get; init; }
    public Image Image { get; init; }
    
    public const int PixelsPerUnit = 20;
    private IReadOnlyList<GridCoordinates> _directions = new List<GridCoordinates>
        { new (1, 0), new (0, 1), new (1, 1), new (1, -1)};

    protected Game(int segmentLength, bool noTouchingRule)
    {
        Grid = new Grid(segmentLength, noTouchingRule);
        Image = new Image(dimensions: new GridCoordinates(20, 20),
            origin: new GridCoordinates(5, 5));
    }

    public Game(Grid grid)
    {
        if (grid.Actions.Count == 0)
        {
            throw new Exception("Attempt to create a game with an invalid grid");
        }

        Grid = grid;
        Image = new Image(dimensions: new GridCoordinates(20, 20),
            origin: new GridCoordinates(5, 5));

        var counter = 0;
        var actions = Grid.Actions.Reverse();
        foreach (var action in actions)
        {
            var dots = action.Elements.OfType<GridDot>().ToList();
            var lines = action.Elements.OfType<GridLine>().ToList();

            if (counter == 0)
            {
                if (lines.Count > 0) 
                    throw new Exception("Line elements are not supported at the initial stage");

                foreach (var dot in dots)
                {
                    Image.Set(new ImageCoordinates(dot.Pt), true);
                }
            }
            else
            {
                if (lines.Count != 1 || dots.Count != 1)
                    throw new Exception($"Invalid grid element at stage {counter}.");

                var line = lines.Single();
                var newPoint = dots.Single().Pt;
                var segment = NewSegment(line.Pt1, line.Pt2, newPoint);
                if (segment is null)
                {
                    throw new Exception($"Invalid segment at stage {counter}.");
                }
                Image.Apply(segment.ToImageAction());
            }

            counter += 1;
        }
    }

    public Segment? NewSegment(GridCoordinates pt1, GridCoordinates pt2, GridCoordinates? newPt = null)
    {
        var segment = Image.NewSegment(pt1, pt2, Grid.SegmentLength, Grid.NoTouchingRule);
        if (newPt.HasValue && segment is not null && !segment.Dot.Pt.Equals(newPt))
        {
            return null;
        }

        return segment;
    }

    public void ApplySegment(Segment segment)
    {
        Grid.Apply(segment.ToGridAction());
        Image.Apply(segment.ToImageAction());
    }

    public bool TryApplySegment(GridCoordinates pt1, GridCoordinates pt2)
    {
        var segment = NewSegment(pt1, pt2);
        if (segment is not null)
        {
            ApplySegment(segment);
            return true;
        }

        return false;
    }

    public int GetScore()
    {
        return Grid.GetScore();
    }

    public GridFootprint GetFootPrint(bool crop = false)
    {
        return (crop) ? Grid.GetFootprint() : Image.GetFootprint();
    }
    
    private bool TryAddSegment(GridCoordinates pt1, GridCoordinates pt2, ICollection<Segment> list)
    {
        var segment = NewSegment(pt1, pt2);
        if (segment is not null)
        {
            list.Add(segment);
            return true;
        }

        return false;
    }

    public List<Segment> FindAllSegments()
    {
        var footprint = Grid.GetFootprint();
        var segments = new List<Segment>();

        for (var x = footprint.Xmin; x <= footprint.Xmax; x++)
        {
            for (var y = footprint.Ymin - 1; y <= footprint.Ymax - Grid.SegmentLength + 1; y++)
            {
                TryAddSegment(new GridCoordinates(x, y), new GridCoordinates(x, y + Grid.SegmentLength), segments);
            }
        }

        for (var y = footprint.Ymin; y <= footprint.Ymax; y++)
        {
            for (var x = footprint.Xmin - 1; x <= footprint.Xmax - Grid.SegmentLength + 1; x++)
            {
                TryAddSegment(new GridCoordinates(x, y), new GridCoordinates(x + Grid.SegmentLength, y), segments);
            }
        }

        for (var x = footprint.Xmin - 1; x <= footprint.Xmax - Grid.SegmentLength + 1; x++)
        {
            for (var y = footprint.Ymin - 1; y <= footprint.Ymax - Grid.SegmentLength + 1; y++)
            {
                TryAddSegment(new GridCoordinates(x, y), new GridCoordinates(x + Grid.SegmentLength, y + Grid.SegmentLength), segments);
                TryAddSegment(new GridCoordinates(x, y + Grid.SegmentLength), new GridCoordinates(x + Grid.SegmentLength, y), segments);
            }
        }

        return segments;
    }
    
    public List<Segment> FindNewSegments(GridCoordinates lastDot)
    {
        var segments = new List<Segment>();
        
        foreach (var direction in _directions)
        {
            for (int position = 0; position <= Grid.SegmentLength; position++)
            {
                // TODO: implement multiplication and addition of GridCoordinates
                // var pt1 = lastDot + position * direction;
                // var pt2 = lastDot + (SegmentLength - position) * direction;
                var pt1 = new GridCoordinates(lastDot.X + position * direction.X,
                    lastDot.Y + position * direction.Y);
                var pt2 = new GridCoordinates(lastDot.X + (position - Grid.SegmentLength) * direction.X,
                    lastDot.Y + (position - Grid.SegmentLength) * direction.Y);
                
                var segment = NewSegment(pt1, pt2);
                if (segment is not null)
                {
                    segments.Add(segment);
                }
            }
        }

        return segments;
    }

    public virtual void Undo(int steps = 1)
    {
        Grid.Undo(steps);
        Image.Load(Grid);
    }

    public virtual void Restart()
    {
        Undo(GetScore());
    }
    public int SvgWidth(GridFootprint footprint)
    {
        return PixelsPerUnit * (footprint.Xmax - footprint.Xmin + 1);
    }

    public int SvgHeight(GridFootprint footprint)
    {
        return PixelsPerUnit * (footprint.Ymax - footprint.Ymin + 1);
    }
    
    public string SvgViewBox(GridFootprint footprint)
    {
        return $"{footprint.Xmin - 0.5:F1} {footprint.Ymin - 0.5:F1} " +
               $"{footprint.Xmax - footprint.Xmin + 1} {footprint.Ymax - footprint.Ymin + 1}";
    }

    public string SvgBackground(GridFootprint footprint, bool grouped = false)
    {
        var width = footprint.Xmax - footprint.Xmin + 1;
        var height = footprint.Ymax - footprint.Ymin + 1;
        var minX = footprint.Xmin - 0.5;
        var maxX = footprint.Xmax + 0.5;
        var minY = footprint.Ymin - 0.5;
        var maxY = footprint.Ymax + 0.5;
        
        var result = $"<rect width=\"{width}\" height=\"{height}\" "
                          + $"x=\"{minX}\" y=\"{minY}\" style=\"fill:white\" />";
        
        const string gridStyle = "stroke:lightgray;stroke-width:0.1";
        for (var i = 0; i < width; i++)
        {
            var x = footprint.Xmin + i;
            result += $"<line x1=\"{x}\" y1=\"{minY}\" x2=\"{x}\" y2=\"{maxY}\" style=\"{gridStyle}\" />";
        }
        for (var i = 0; i < height; i++)
        {
            var y = footprint.Ymin + i;
            result += $"<line x1=\"{minX}\" y1=\"{y}\" x2=\"{maxX}\" y2=\"{y}\" style=\"{gridStyle}\" />";
        }

        return (grouped) ? "<g>" + result + "</g>" : result;
    }
    
    public string ToSvg(bool crop = false)
    {
        var footprint = (crop) ? Grid.GetFootprint() : Image.GetFootprint();

        return $"<svg width=\"{SvgWidth(footprint)}\" height=\"{SvgHeight(footprint)}\" " +
               $"viewbox=\"{SvgViewBox(footprint)}\">" +
               SvgBackground(footprint) +
               Grid.ToSvg() +
               "</svg>";
    }
}