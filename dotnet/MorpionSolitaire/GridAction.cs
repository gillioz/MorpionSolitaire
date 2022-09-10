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

    public string ToSvg(string? id = null, string prefix = "", string color = "black")
    {;
        var result = prefix + "<g";
        if (id is not null)
        {
            result += $" id=\"{id}\"";
        }
        result += ">\n";
        foreach (var element in Elements)
        {
            result += prefix + "\t" + element.ToSvg(color) + "\n";
        }
        result += prefix + "</g>";
        return result;
    }
}