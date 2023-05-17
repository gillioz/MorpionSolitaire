namespace MorpionSolitaire;

public struct Point
{
    // coordinates are signed 8-bit integers (-128 to 127) 
    public sbyte X { get; }
    public sbyte Y { get; }

    public Point(sbyte x, sbyte y)
    {
        X = x;
        Y = y;
    }

    public Point(int x, int y)
    {
        X = (sbyte)x;
        Y = (sbyte)y;
    }

    private bool Equals(Point other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Point other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Point pt1, Point pt2)
    {
        return pt1.Equals(pt2);
    }

    public static bool operator !=(Point pt1, Point pt2)
    {
        return !(pt1 == pt2);
    }

    public static bool operator <(Point pt1, Point pt2)
    {
        return pt1.X < pt2.X || (pt1.X == pt2.X && pt1.Y < pt2.Y);
    }

    public static bool operator >(Point pt1, Point pt2)
    {
        return pt1.X > pt2.X || (pt1.X == pt2.X && pt1.Y > pt2.Y);
    }

    public static Point operator *(int n, Point pt)
    {
        return new Point((sbyte)(n * pt.X), (sbyte)(n * pt.Y));
    }

    public static Point operator /(Point pt, int n)
    {
        return new Point((sbyte)(pt.X / n), (sbyte)(pt.Y / n));
    }

    public static Point operator +(Point pt, int n)
    {
        return new Point((sbyte)(pt.X + 1), (sbyte)(pt.Y + 1));
    }

    public static Point operator +(Point pt1, Point pt2)
    {
        return new Point((sbyte)(pt1.X + pt2.X), (sbyte)(pt1.Y + pt2.Y));
    }
}