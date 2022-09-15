namespace MorpionSolitaire;

public class Grid
{
    public List<GridAction> Actions { get; }

    public Grid()
    {
        Actions = new List<GridAction>();
    }

    public void Apply(GridAction action)
    {
        Actions.Add(action);
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

    public string ToJson(string spacing = "")
    {
        var result = spacing + "[\n";
        for (int i = 0; i < Actions.Count; i++)
        {
            result += spacing + "\t{\n" +
                      spacing + $"\t\t\"stage\": {i},\n" +
                      spacing + "\t\t\"actions\":\n" +
                      Actions[i].ToJson(spacing + "\t\t") + "\n" +
                      spacing + "\t},\n";
        }
        result = result.Remove(result.Length - 2) + "\n";
        result += spacing + "]";
        return result;
    }
    
    public string ToSvg(string spacing = "")
    {
        var result = string.Empty;
        for (int i = 0; i < Actions.Count; i++)
        {
            result += Actions[i].ToSvg($"grid-action-{i}", spacing) + "\n";
        }

        return result;
    }

    public int GetScore()
    {
        return Actions.Count - 1;
    }
}