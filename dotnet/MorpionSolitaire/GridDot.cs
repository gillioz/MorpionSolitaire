using System.Drawing;
using GrapeCity.Documents.Svg;

namespace MorpionSolitaire;

public class GridDot : GridElement
{
    public GridCoordinates Dot { get; }

    private const float SvgCircleRadius = (float)0.15;

    public GridDot(GridCoordinates pt)
    {
        Dot = pt;
    }

    public override void ComputeFootprint(GridFootprint footprint)
    {
        footprint.Add(Dot);
    }

    public override void AddToSvgDoc(SvgDocument svgDoc)
    {
        var circle = new SvgCircleElement()
        {
            CenterX = new SvgLength(Dot.X),
            CenterY = new SvgLength(Dot.Y),
            Radius = new SvgLength(SvgCircleRadius),
            Fill = new SvgPaint(Color.Black)
        };
        svgDoc.Document.RootSvg.Children.Add(circle);
    }
}