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

    public string ToSvg(string spacing = "")
    {
        var result = string.Empty;
        for (int i = 0; i < Actions.Count; i++)
        {
            result += Actions[i].ToSvg($"grid-action-{i}", spacing) + "\n";
        }

        return result;
    }
}