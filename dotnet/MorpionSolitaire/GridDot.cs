namespace MorpionSolitaire;

public class GridDot : GridElement
{
    public GridPoint Pt { get; }

    public GridDot(GridPoint pt)
    {
        Pt = pt;
    }

    public override void ComputeFootprint(GridFootprint footprint)
    {
        footprint.Add(Pt);
    }
    
    public override string ToSvg(string color)
    {
        return $"<circle cx=\"{Pt.X}\" cy=\"{Pt.Y}\" r=\"0.15\" fill=\"{color}\" />";
    }

    public override List<sbyte> ToCoordinatesList()
    {
        return new List<sbyte> { Pt.X, Pt.Y };
    }
}