namespace MorpionSolitaire;

public class GridFootprint
{
    public int Xmin { get; set; }
    public int Ymin { get; set; }
    public int Xmax { get; set; }
    public int Ymax { get; set; }

    public GridFootprint()
    {
        Xmin = 0;
        Ymin = 0;
        Xmax = 0;
        Ymax = 0;
    }

    public void Add(GridCoordinates pt)
    {
        Xmin = Math.Min(Xmin, pt.X);
        Xmax = Math.Max(Xmax, pt.X);
        Ymin = Math.Min(Ymin, pt.Y);
        Ymax = Math.Max(Ymax, pt.Y);
    }
}