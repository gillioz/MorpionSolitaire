using System.Drawing;
using GrapeCity.Documents.Svg;

namespace MorpionSolitaire;

public class SvgDocument
{
    public GcSvgDocument Document { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int MinX { get; set; }
    public int MinY { get; set; }
        
    public const int PixelsPerUnit = 20;
    public const float Half = (float)0.5;
    private const float GridLineWidth = (float)0.05;

    public SvgDocument(string? id = null)
    {
        Document = new GcSvgDocument();
        if (id is not null)
        {
            Document.RootSvg.ID = id;
        }
        Width = 1;
        Height = 1;
        MinX = 0;
        MinY = 0;
    }
    
    public void SetDimensions(int width, int height, int minX, int minY)
    {
        Width = width;
        Height = height;
        MinX = minX;
        MinY = minY;
        Document.RootSvg.Width = new SvgLength(PixelsPerUnit * (Width + 1), SvgLengthUnits.Pixels);
        Document.RootSvg.Height = new SvgLength(PixelsPerUnit * (Height + 1), SvgLengthUnits.Pixels);
        Document.RootSvg.ViewBox = new SvgViewBox()
        {
            MinX = (float)MinX - Half,
            MinY = (float)MinY - Half,
            Width = Width + 1,
            Height = Height + 1
        };
    }

    public void DrawGrid()
    {
        var minX = new SvgLength(MinX - Half);
        var maxX = new SvgLength(MinX + Width + Half);
        var minY = new SvgLength(MinY - Half);
        var maxY = new SvgLength(MinY + Height + Half);
        var stroke = new SvgPaint(Color.LightGray);
        var strokeWidth = new SvgLength(GridLineWidth);
        for (int i = 0; i <= Width; i++)
        {
            var x = new SvgLength(MinX + i);
            Document.RootSvg.Children.Add(new SvgLineElement()
            {
                X1 = x,
                Y1 = minY,
                X2 = x,
                Y2 = maxY,
                Stroke = stroke,
                StrokeWidth = strokeWidth
            });
        }
        for (int i = 0; i <= Height; i++)
        {
            var y = new SvgLength(MinY + i);
            Document.RootSvg.Children.Add(new SvgLineElement()
            {
                X1 = minX,
                Y1 = y,
                X2 = maxY,
                Y2 = y,
                Stroke = stroke,
                StrokeWidth = strokeWidth
            });
        }
    }
}