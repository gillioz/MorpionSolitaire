﻿namespace MorpionSolitaire;

public class Image
{
    private ImageCoordinates _dimensions;
    private ImageCoordinates _origin;
    private bool[,] _image;
    private int _sizeIncrement;

    public Image(GridCoordinates dimensions, GridCoordinates origin, int sizeIncrement = 1)
    {
        _dimensions = new ImageCoordinates(dimensions);
        _origin = new ImageCoordinates(origin);
        _origin.Add(1, 1);
        InitializeImage();
        _sizeIncrement = 3 * sizeIncrement;
    }

    public Image(Grid grid)
    {
        throw new NotImplementedException();
    }

    private void InitializeImage()
    {
        _image = new bool[_dimensions.X, _dimensions.Y];
    }

    public bool Get(ImageCoordinates pt)
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

    public void Set(ImageCoordinates pt, bool value)
    {
        var x = _origin.X + pt.X;
        var y = _origin.Y + pt.Y;

        if (x < 0)
        {
            ExtendLeft();
            Set(pt, value);
            return;
        }

        if (x >= _dimensions.X)
        {
            ExtendRight();
            Set(pt, value);
            return;
        }

        if (y < 0)
        {
            ExtendBottom();
            Set(pt, value);
            return;
        }

        if (y >= _dimensions.Y)
        {
            ExtendTop();
            Set(pt, value);
            return;
        }

        _image[x, y] = value;
    }

    private void ExtendLeft()
    {
        _dimensions.X += _sizeIncrement;
        var imageCopy = _image;
        InitializeImage();
        for (int x = _sizeIncrement; x < _dimensions.X; x++)
        {
            for (int y = 0; y < _dimensions.Y; y++)
            {
                _image[x, y] = imageCopy[x, y];
            }
        }

        _origin.X += _sizeIncrement;
    }

    private void ExtendRight()
    {
        _dimensions.X += _sizeIncrement;
        var imageCopy = _image;
        InitializeImage();
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
        _dimensions.Y += _sizeIncrement;
        var imageCopy = _image;
        InitializeImage();
        for (int x = 0; x < _dimensions.X; x++)
        {
            for (int y = _sizeIncrement; y < _dimensions.Y; y++)
            {
                _image[x, y] = imageCopy[x, y];
            }
        }

        _origin.Y += _sizeIncrement;
    }

    private void ExtendTop()
    {
        _dimensions.Y += _sizeIncrement;
        var imageCopy = _image;
        InitializeImage();
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
        foreach (ImageCoordinates pixel in action.Pixels)
        {
            if (Get(pixel))
            {
                return false;
            }
        }

        return true;
    }

    public void Apply(ImageAction action, bool value = true)
    {
        foreach (ImageCoordinates pixel in action.Pixels)
        {
            Set(pixel, value);
        }
    }

    public void Undo(ImageAction action)
    {
        Apply(action, false);
    }
    
    public void SetSvgDimensions(SvgDocument svgDocument)
    {
        var dimensions = _dimensions.ToGridCoordinates();
        var origin = _origin.ToGridCoordinates();
        svgDocument.SetDimensions(dimensions.X, dimensions.Y, 
            -1 * origin.X, -1 * origin.Y);
    }
}