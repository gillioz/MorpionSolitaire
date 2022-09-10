using GrapeCity.Documents.Svg;

namespace MorpionSolitaire;

public class GridAction
{
    public List<GridElement> Elements { get; }

    public GridAction()
    {
        Elements = new List<GridElement>();
    }

    public void Add(GridElement element)
    {
        Elements.Add(element);
    }

    public void ComputeFootprint(GridFootprint footprint)
    {
        foreach (var element in Elements)
        {
            element.ComputeFootprint(footprint);
        }
    }

    public void AddToSvgDoc(SvgDocument svgDoc)
    {
        foreach (var element in Elements)
        {
            element.AddToSvgDoc(svgDoc);
        }
    }
}