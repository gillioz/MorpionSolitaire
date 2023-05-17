namespace MorpionSolitaire;

public class Image
{
    private ImagePoint _dimensions;
    private ImagePoint _origin;
    private bool[,] _image;
    private readonly int _sizeIncrement;

    public Image(GridPoint dimensions, GridPoint origin, int sizeIncrement = 3)
    {
        _dimensions = dimensions.ToImagePoint();
        _origin = origin.ToImagePoint(1);
        _image = EmptyImage();
        _sizeIncrement = 3 * sizeIncrement;
    }
    
    private bool[,] EmptyImage()
    {
        return new bool[_dimensions.X, _dimensions.Y];
    }

    public void Load(Grid grid)
    {
        _image = EmptyImage();

        // adjust the grid size if needed
        var footprint = grid.GetFootprint();
        var minCorner = footprint.MinCorner().ToImagePoint();
        var maxCorner = footprint.MaxCorner().ToImagePoint();
        Set(minCorner, false);
        Set(maxCorner, false);

        var actions = grid.Actions.Reverse();
        foreach (var action in actions)
        {
            var gridLines = action.Elements.OfType<GridLine>().ToList();
            if (gridLines.Count == 1)
            {
                var segment = NewSegment(gridLines.First().Pt1, gridLines.First().Pt2,
                    grid.SegmentLength, grid.NoTouchingRule);
                if (segment is null)
                {
                    throw new Exception("Invalid segment");
                }
                Apply(segment.ToImageAction());
            }
            else if (gridLines.Count == 0)
            {
                var gridDots = action.Elements.OfType<GridDot>().ToList();
                foreach (var gridDot in gridDots)
                {
                    Set(gridDot.Pt.ToImagePoint(), true);
                }
            }
            else
            {
                throw new Exception("Impossible to load Image from Grid.");
            }
        }
    }

    public bool Get(ImagePoint pt)
    {
        var x = _origin.X + pt.X;
        var y = _origin.Y + pt.Y;
        
        if (x < 0 || x >= _dimensions.X)
        {
            return false;
        }

        if (y < 0 || y >= _dimensions.Y)
        {
            return false;
        }

        return _image[x, y];
    }

    public void Set(ImagePoint pt, bool value)
    {
        var x = _origin.X + pt.X;
        var y = _origin.Y + pt.Y;

        if (x < 3)
        {
            ExtendLeft();
            Set(pt, value);
            return;
        }

        if (x >= _dimensions.X - 3)
        {
            ExtendRight();
            Set(pt, value);
            return;
        }

        if (y < 3)
        {
            ExtendBottom();
            Set(pt, value);
            return;
        }

        if (y >= _dimensions.Y - 3)
        {
            ExtendTop();
            Set(pt, value);
            return;
        }

        _image[x, y] = value;
    }

    public Segment? NewSegment(GridPoint pt1, GridPoint pt2, int segmentLength, bool noTouchingRule)
    {
        if (segmentLength <= 0)
        {
            throw new Exception($"Invalid segment length: {segmentLength}");
        }
        
        var w = pt2.X - pt1.X;
        var h = pt2.Y - pt1.Y;
        if ((w != 0 && Math.Abs(w) != segmentLength)
            || (h != 0 && Math.Abs(h) != segmentLength)
            || (w == 0 && h == 0))
        {
            return null;
        }
        
        var dx = w / segmentLength;
        var dy = h / segmentLength;
        int iMin = 0;
        int iMax = 3 * segmentLength + 1;
        if (noTouchingRule)
        {
            iMin -= 1;
            iMax += 1;
        }

        ImagePoint? dot = null;
        var supportDots = new List<ImagePoint>();
        var line = new List<ImagePoint>();

        var emptyDotSet = false;
        var pt0 = pt1.ToImagePoint();
        for (int i = iMin; i < iMax; i++)
        {
            var pt = new ImagePoint(pt0.X + i * dx, pt0.Y + i * dy);
            if (pt.IsDot())
            {
                if (Get(pt))
                {
                    supportDots.Add(pt);
                }
                else
                {
                    if (emptyDotSet)
                    {
                        return null;
                    }

                    emptyDotSet = true;
                    dot = pt;
                }
            }
            else
            {
                if (Get(pt))
                {
                    return null;
                }

                line.Add(pt);
            }
        }

        if (!emptyDotSet || dot is null)
        {
            return null;
        }

        return new Segment(new GridDot(dot.Value.ToGridPoint()), new GridLine(pt1, pt2),
            dot.Value, supportDots, line);
    }
    
    private void ExtendLeft()
    {
        _dimensions = new ImagePoint(_dimensions.X + _sizeIncrement, _dimensions.Y);
        var imageCopy = _image;
        _image = EmptyImage();
        for (int x = _sizeIncrement; x < _dimensions.X; x++)
        {
            for (int y = 0; y < _dimensions.Y; y++)
            {
                _image[x, y] = imageCopy[x - _sizeIncrement, y];
            }
        }

        _origin = new ImagePoint(_origin.X + _sizeIncrement, _origin.Y);
    }

    private void ExtendRight()
    {
        _dimensions = new ImagePoint(_dimensions.X + _sizeIncrement, _dimensions.Y);
        var imageCopy = _image;
        _image = EmptyImage();
        for (int x = 0; x < _dimensions.X - _sizeIncrement; x++)
        {
            for (int y = 0; y < _dimensions.Y; y++)
            {
                _image[x, y] = imageCopy[x, y];
            }
        }
    }

    private void ExtendBottom()
    {
        _dimensions = new ImagePoint(_dimensions.X, _dimensions.Y + _sizeIncrement);
        var imageCopy = _image;
        _image = EmptyImage();
        for (int x = 0; x < _dimensions.X; x++)
        {
            for (int y = _sizeIncrement; y < _dimensions.Y; y++)
            {
                _image[x, y] = imageCopy[x, y - _sizeIncrement];
            }
        }

        _origin = new ImagePoint(_origin.X, _origin.Y + _sizeIncrement);
    }

    private void ExtendTop()
    {
        _dimensions = new ImagePoint(_dimensions.X, _dimensions.Y + _sizeIncrement);
        var imageCopy = _image;
        _image = EmptyImage();
        for (int x = 0; x < _dimensions.X; x++)
        {
            for (int y = 0; y < _dimensions.Y - _sizeIncrement; y++)
            {
                _image[x, y] = imageCopy[x, y];
            }
        }
    }

    public bool IsValid(ImageAction action)
    {
        foreach (ImagePoint pixel in action.Pixels)
        {
            if (Get(pixel))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsValid(Segment segment)
    {
        return IsValid(segment.ToImageAction());
    }

    public void Apply(ImageAction action, bool value = true)
    {
        foreach (ImagePoint pixel in action.Pixels)
        {
            Set(pixel, value);
        }
    }

    public GridFootprint GetFootprint()
    {
        var dimensions = _dimensions.ToGridPoint();
        var origin = _origin.ToGridPoint();
        var footprint = new GridFootprint();
        footprint.MinX = -1 * origin.X;
        footprint.MinY = -1 * origin.Y;
        footprint.MaxX = dimensions.X - origin.X - 1;
        footprint.MaxY = dimensions.Y - origin.Y - 1;
        return footprint;
    }

    // public Bitmap ToBitmap()
    // {
    //     var w = _dimensions.X;
    //     var h = _dimensions.Y;
    //     var bitmap = new Bitmap(w, height: h);
    //
    //     for (int x = 0; x < w; x++)
    //     {
    //         for (int y = 0; y < h; y++)
    //         {
    //             bitmap.SetPixel(x, y, _image[x, y] ? Color.Black : Color.White);
    //         }
    //     }
    //
    //     return bitmap;
    // }

    // public void SaveBitmap(string file)
    // {
    //     var bitmap = ToBitmap();
    //     bitmap.Save(file + ".bmp");
    // }

    public float[,] ToTensor(int width, int height)
    {
        var tensor = new float[width, height];
        var xRange = Math.Min(width, _dimensions.X - 2);
        var yRange = Math.Min(height, _dimensions.Y - 2);
        for (var x = 0; x < xRange; x++)
        {
            for (var y = 0; y < yRange; y++)
            {
                tensor[x, y] = _image[x + 1, y + 1] ? 1 : 0;
            }
        }

        return tensor;
    }

    public float[,] ToTensor(int size) => ToTensor(size, size);
}