namespace MorpionSolitaire;

public class ImageCoordinates
{
    public int X { get; set; }
    public int Y { get; set; }

    public ImageCoordinates(int x, int y)
    {
        X = x;
        Y = y;
    }

    public ImageCoordinates(GridCoordinates coord)
    {
        X = 3 * coord.X;
        Y = 3 * coord.Y;
    }

    public void Add(int x, int y)
    {
        X += x;
        Y += y;
    }

    public GridCoordinates ToGridCoordinates()
    {
        return new GridCoordinates(X / 3, Y / 3);
    }

    public bool IsDot()
    {
        return (X % 3 == 0) && (Y % 3 == 0);
    }
}