namespace MorpionSolitaire;

public class GridLine : GridElement
{
    public GridCoordinates Pt1 { get; init; }
    public GridCoordinates Pt2 { get; init; }

    public GridLine(GridCoordinates pt1, GridCoordinates pt2)
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

    public override GridElementDto ToGridElementJson()
    {
        return new GridElementDto(this);
    }
    
    public static bool operator ==(GridLine l1, GridLine l2)
    {
        return l1.Pt1 == l2.Pt1 && l1.Pt2 == l2.Pt2;
    }

    public static bool operator !=(GridLine l1, GridLine l2)
    {
        return l1.Pt1 != l2.Pt1 || l1.Pt2 != l2.Pt2;
    }
}