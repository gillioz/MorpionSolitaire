namespace MorpionSolitaire;

public struct GridPoint
{
    public Point Value { get; }

    public sbyte X => Value.X;
    public sbyte Y => Value.Y;

    public GridPoint(Point pt)
    {
        Value = pt;
    }

    public GridPoint(int x, int y)
    {
        Value = new Point(x, y);
    }

    public ImagePoint ToImagePoint()
    {
        return new ImagePoint(3 * Value);
    }

    public ImagePoint ToImagePoint(int offset)
    {
        return new ImagePoint(3 * Value + 1);
    }

    private bool Equals(GridPoint other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is GridPoint other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(GridPoint pt1, GridPoint pt2)
    {
        return pt1.Equals(pt2);
    }

    public static bool operator !=(GridPoint pt1, GridPoint pt2)
    {
        return !(pt1 == pt2);
    }

    public static bool operator <(GridPoint pt1, GridPoint pt2)
    {
        return pt1.Value < pt2.Value;
    }

    public static bool operator >(GridPoint pt1, GridPoint pt2)
    {
        return pt1.Value > pt2.Value;
    }

    public static GridPoint operator +(GridPoint pt1, GridPoint pt2)
    {
        return new GridPoint(pt1.Value + pt2.Value);
    }

    public static GridPoint operator *(int n, GridPoint pt)
    {
        return new GridPoint(n * pt.Value);
    }
}