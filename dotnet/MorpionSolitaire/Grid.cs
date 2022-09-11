namespace MorpionSolitaire;

public class Grid
{
    public List<GridAction> Actions { get; }
    
    public const int PixelsPerUnit = 20;

    public Grid()
    {
        Actions = new List<GridAction>();
    }

    public void Apply(GridAction action)
    {
        Actions.Add(action);
    }

    public bool Validate(Segment segment)
    {
        throw new NotImplementedException();
    }
    
    public GridFootprint GetFootprint()
    {
        var footprint = new GridFootprint();
        foreach (var action in Actions)
        {
            action.ComputeFootprint(footprint);
        }
        return footprint;
    }

    public string ToSvg(string? id = null, Image? image = null, string spacing = "")
    {
        var footprint = (image is null) ? GetFootprint() : image.GetFootprint();
        var width = footprint.Xmax - footprint.Xmin + 1;
        var height = footprint.Ymax - footprint.Ymin + 1;
        var minX = footprint.Xmin - 0.5;
        var maxX = footprint.Xmax + 0.5;
        var minY = footprint.Ymin - 0.5;
        var maxY = footprint.Ymax + 0.5;
        var viewBox = $"{minX:F1} {minY:F1} {width} {height}";

        var result = spacing + "<svg ";
        if (id is not null)
        {
            result += $"id=\"{id}\" ";
        }
        result += $"width=\"{PixelsPerUnit * width}\" " +
                  $"height=\"{PixelsPerUnit * height}\" " +
                  $"viewbox=\"{viewBox}\">\n";

        result += spacing + "\t<g id=\"grid-background\">\n";
        var gridstyle = "stroke:lightgray;stroke-width:0.05";
        for (int i = 0; i < width; i++)
        {
            var x = footprint.Xmin + i;
            result += spacing + 
                      $"\t\t<line x1=\"{x}\" y1=\"{minY}\" " +
                      $"x2=\"{x}\" y2=\"{maxY}\" " +
                      $"style=\"{gridstyle}\" />\n";
        }
        for (int i = 0; i < height; i++)
        {
            var y = footprint.Ymin + i;
            result += spacing + 
                      $"\t\t<line x1=\"{minX}\" y1=\"{y}\" " +
                      $"x2=\"{maxX}\" y2=\"{y}\" " +
                      $"style=\"{gridstyle}\" />\n";
        }
        result += spacing + "\t</g>\n";
        
        for (int i = 0; i < Actions.Count; i++)
        {
            result += Actions[i].ToSvg($"grid-action-{i}", spacing + "\t") + "\n";
        }
        
        result += spacing + "</svg>";
        return result;
    }
}