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
    
    public string ToJson(string spacing = "")
    {
        var list = Elements.Select(x => x.ToJson(spacing + "\t"));
        return spacing + "[\n" + 
               string.Join(",\n", list) + "\n" +
               spacing + "]";
    }

    public string ToSvg(string? id = null, string spacing = "", string color = "black")
    {;
        var result = spacing + "<g";
        if (id is not null)
        {
            result += $" id=\"{id}\"";
        }
        result += ">\n";
        foreach (var element in Elements)
        {
            result += spacing + "\t" + element.ToSvg(color) + "\n";
        }
        result += spacing + "</g>";
        return result;
    }
}