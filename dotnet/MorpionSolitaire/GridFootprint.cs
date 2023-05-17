namespace MorpionSolitaire;

public class GridFootprint
{
    public int MinX { get; set; }
    public int MinY { get; set; }
    public int MaxX { get; set; }
    public int MaxY { get; set; }

    public GridFootprint()
    {
        MinX = 0;
        MinY = 0;
        MaxX = 0;
        MaxY = 0;
    }

    public void Add(GridPoint pt)
    {
        MinX = Math.Min(MinX, pt.X);
        MaxX = Math.Max(MaxX, pt.X);
        MinY = Math.Min(MinY, pt.Y);
        MaxY = Math.Max(MaxY, pt.Y);
    }

    public GridPoint MinCorner()
    {
        return new GridPoint(MinX, MinY);
    }

    public GridPoint MaxCorner()
    {
        return new GridPoint(MaxX, MaxY);
    }
}