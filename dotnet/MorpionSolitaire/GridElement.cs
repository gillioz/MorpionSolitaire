namespace MorpionSolitaire;

public abstract class GridElement
{
    public abstract void ComputeFootprint(GridFootprint footprint);

    public abstract string ToSvg(string color);
    public abstract List<sbyte> ToCoordinatesList();
}