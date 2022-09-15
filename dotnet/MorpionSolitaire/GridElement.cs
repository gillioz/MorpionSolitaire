namespace MorpionSolitaire;

public abstract class GridElement
{
    public abstract void ComputeFootprint(GridFootprint footprint);

    public abstract string ToJson(string spacing = "");
    public abstract string ToSvg(string color);
}