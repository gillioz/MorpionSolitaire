namespace MorpionSolitaire;

public abstract class GridElement
{
    public abstract void ComputeFootprint(GridFootprint footprint);

    public abstract void AddToSvgDoc(SvgDocument svgDoc);
}