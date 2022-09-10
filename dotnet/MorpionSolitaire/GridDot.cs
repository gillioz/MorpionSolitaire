namespace MorpionSolitaire;

public class GridDot : GridElement
{
    public GridCoordinates Pt { get; }

    public GridDot(GridCoordinates pt)
    {
        Pt = pt;
    }

    public override void ComputeFootprint(GridFootprint footprint)
    {
        footprint.Add(Pt);
    }

    public override string ToSvg(string color)
    {
        return $"<circle cx=\"{Pt.X}\" cy=\"{Pt.Y}\" " +
               $"r=\"0.15\" fill=\"{color}\" />";
    }

}