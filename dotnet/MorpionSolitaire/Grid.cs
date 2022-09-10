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

    public void SetSvgDimensions(SvgDocument svgDocument)
    {
        var footprint = GetFootprint();
        svgDocument.SetDimensions(footprint.Xmax - footprint.Xmin, 
            footprint.Ymax - footprint.Ymin, 
            footprint.Xmin, footprint.Ymin);
        
    }
    
    public void AddToSvgDoc(SvgDocument svgDocument)
    {
        foreach (var action in Actions)
        {
            action.AddToSvgDoc(svgDocument);
        }
    }
}