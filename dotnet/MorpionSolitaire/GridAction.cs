namespace MorpionSolitaire;

public class GridAction
{
    public List<GridElement> Elements { get; init; }

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

    public string ToSvg(string color = "black", bool grouped = false)
    {
        var result = "";
        foreach (var element in Elements)
        {
            result += element.ToSvg(color);
        }
        return (grouped) ? "<g>" + result + "</g>" : result;
    }
}