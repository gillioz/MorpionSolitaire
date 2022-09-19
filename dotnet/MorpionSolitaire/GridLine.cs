namespace MorpionSolitaire;

public class GridLine : GridElement
{
    public GridCoordinates Pt1 { get; init; }
    public GridCoordinates Pt2 { get; init; }

    public GridLine(GridCoordinates pt1, GridCoordinates pt2)
    {
        Pt1 = pt1;
        Pt2 = pt2;
    }
    
    public override void ComputeFootprint(GridFootprint footprint)
    {
        footprint.Add(Pt1);
        footprint.Add(Pt2);
    }
    
    // public override string ToJson(string spacing = "")
    // {
    //     return spacing + "{\n" +
    //            spacing + "\t\"type\": \"line\",\n" +
    //            spacing + $"\t\"x1\": {Pt1.X},\n" +
    //            spacing + $"\t\"y1\": {Pt1.Y},\n" +
    //            spacing + $"\t\"x2\": {Pt2.X},\n" +
    //            spacing + $"\t\"y2\": {Pt2.Y}\n" +
    //            spacing + "}";
    // }

    public override string ToSvg(string color)
    {
        return $"<line x1=\"{Pt1.X}\" y1=\"{Pt1.Y}\" " +
               $"x2=\"{Pt2.X}\" y2=\"{Pt2.Y}\" " +
               $"style=\"stroke:{color};stroke-width:0.1\" />";
    }

    public override GridElementJson ToGridElementJson()
    {
        return new GridElementJson(this);
    }
}