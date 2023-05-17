namespace MorpionSolitaire;

public class GridLine : GridElement
{
    public GridPoint Pt1 { get; init; }
    public GridPoint Pt2 { get; init; }

    public GridLine(GridPoint pt1, GridPoint pt2)
    {
        if (pt1 < pt2)
        {
            Pt1 = pt1;
            Pt2 = pt2;
        }
        else
        {
            Pt1 = pt2;
            Pt2 = pt1;
        }
    }
    
    public override void ComputeFootprint(GridFootprint footprint)
    {
        footprint.Add(Pt1);
        footprint.Add(Pt2);
    }

    public override string ToSvg(string color)
    {
        return $"<line x1=\"{Pt1.X}\" y1=\"{Pt1.Y}\" x2=\"{Pt2.X}\" y2=\"{Pt2.Y}\" " +
               $"style=\"stroke:{color};stroke-width:0.1\" />";
    }

    private bool Equals(GridLine other)
    {
        return Pt1 == other.Pt1 && Pt2 == other.Pt2;
    }

    public override bool Equals(object? obj)
    {
        return obj is GridLine other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Pt1, Pt2);
    }

    public static bool operator ==(GridLine l1, GridLine l2)
    {
        return l1.Equals(l2);
    }

    public static bool operator !=(GridLine l1, GridLine l2)
    {
        return !l1.Equals(l2);
    }

    public override List<sbyte> ToCoordinatesList()
    {
        return new List<sbyte> { Pt1.X, Pt1.Y, Pt2.X, Pt2.Y };
    }
}