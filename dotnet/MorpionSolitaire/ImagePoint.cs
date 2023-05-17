namespace MorpionSolitaire;

public struct ImagePoint
{
    public Point Value;

    public sbyte X => Value.X;
    public sbyte Y => Value.Y;

    public ImagePoint(Point pt)
    {
        Value = pt;
    }

    public ImagePoint(int x, int y)
    {
        Value = new Point(x, y);
    }

    public GridPoint ToGridPoint()
    {
        return new GridPoint(Value / 3);
    }
    public bool IsDot()
    {
        return (Value.X % 3 == 0) && (Value.Y % 3 == 0);
    }
}