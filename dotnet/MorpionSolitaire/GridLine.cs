using System.Drawing;
using GrapeCity.Documents.Svg;

namespace MorpionSolitaire;

public class GridLine : GridElement
{
    public GridCoordinates[] Line { get; init; }
    
    private const float SvgLineWidth = (float)0.1;

    public GridLine(GridCoordinates pt1, GridCoordinates pt2)
    {
        Line = new []
        {
            pt1, pt2
        };
    }
    
    public override void ComputeFootprint(GridFootprint footprint)
    {
        foreach (var point in Line)
        {
            footprint.Add(point);
        }
    }

    public override void AddToSvgDoc(SvgDocument svgDoc)
    {
        var line = new SvgLineElement()
        {
            X1 = new SvgLength(Line[0].X),
            Y1 = new SvgLength(Line[0].Y),
            X2 = new SvgLength(Line[1].X),
            Y2 = new SvgLength(Line[1].Y),
            Stroke = new SvgPaint(Color.Black),
            StrokeWidth = new SvgLength(SvgLineWidth)
        };
        svgDoc.Document.RootSvg.Children.Add(line);
    }
}