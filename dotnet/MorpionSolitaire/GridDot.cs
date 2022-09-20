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

    // public override string ToJson(string spacing = "")
    // {
    //     return spacing + "{\n" +
    //            spacing + "\t\"type\": \"dot\",\n" +
    //            spacing + $"\t\"x\": {Pt.X},\n" +
    //            spacing + $"\t\"y\": {Pt.Y}\n" +
    //            spacing + "}";
    // }
    
    public override string ToSvg(string color)
    {
        return $"<circle cx=\"{Pt.X}\" cy=\"{Pt.Y}\" " +
               $"r=\"0.15\" fill=\"{color}\" />";
    }

    public override GridElementDto ToGridElementJson()
    {
        return new GridElementDto(this);
    }
}