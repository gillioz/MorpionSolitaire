﻿using System.Text.Json;

namespace MorpionSolitaire;

public class Game
{
    public Grid Grid { get; }
    public Image Image { get; }
    public int SegmentLength { get; }
    public bool NoTouchingRule { get; }
    
    public const int PixelsPerUnit = 20;

    public Game(int segmentLength = 4, bool noTouchingRule = false,
        Grid? grid = null)
    {
        if (grid is null)
        {
            grid = segmentLength switch
            {
                4 => Grid.Cross(),
                _ => throw new Exception($"No default grid implemented with segment length {segmentLength}.")
            };
        }
        SegmentLength = segmentLength;
        NoTouchingRule = noTouchingRule;
        Grid = grid;
        Image = new Image(dimensions: new GridCoordinates(20, 20),
            origin: new GridCoordinates(5, 5));

        if (grid.Actions.Count == 0)
        {
            throw new Exception("Attempt to create a game with an invalid grid");
        }

        var setupAction = grid.Actions.First();
        var dots = setupAction.Elements.OfType<GridDot>().ToList();
        foreach (var dot in dots)
        {
            Image.Set(new ImageCoordinates(dot.Pt), true);
        }
        var lines = setupAction.Elements.OfType<GridLine>().ToList();
        if (lines.Count > 0)
        {
            throw new NotImplementedException();
        }

        for (int i = 1; i < grid.Actions.Count; i++)
        {
            var action = grid.Actions[i];
            dots = action.Elements.OfType<GridDot>().ToList();
            if (dots.Count != 1)
            {
                throw new Exception($"Invalid number of dots at stage {i}.");
            }
            lines = action.Elements.OfType<GridLine>().ToList();
            if (lines.Count != 1)
            {
                throw new Exception($"Invalid number of lines at stage {i}.");
            }

            var line = lines.First();
            var pt = dots.First().Pt;

            var segment = new Segment(line.Pt1, line.Pt2, Image, SegmentLength, NoTouchingRule, pt);
            Image.Apply(segment.ToImageAction());
        }
    }

    public bool TrySegment(GridCoordinates pt1, GridCoordinates pt2)
    {
        try
        {
            var segment = new Segment(pt1, pt2, Image, SegmentLength, NoTouchingRule);
            Grid.Apply(segment.ToGridAction());
            Image.Apply(segment.ToImageAction());
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return false;
    }

    public int GetScore()
    {
        return Grid.GetScore();
    }

    public GridFootprint GetFootPrint(bool crop = false)
    {
        return (crop) ? Grid.GetFootprint() : Image.GetFootprint();
    }
    
    private void TryAddSegment(GridCoordinates pt1, GridCoordinates pt2, ICollection<Segment> list) 
    {
        try
        {
            var segment = new Segment(pt1, pt2, Image, SegmentLength, NoTouchingRule);
            list.Add(segment);
        }
        catch (Exception e)
        {
            // ignored
            Console.WriteLine(e);
        }
    }

    public List<Segment> FindAllSegments()
    {
        var footprint = Grid.GetFootprint();
        var segments = new List<Segment>();

        for (var x = footprint.Xmin; x <= footprint.Xmax; x++)
        {
            for (var y = footprint.Ymin - 1; y <= footprint.Ymax - SegmentLength + 1; y++)
            {
                TryAddSegment(new GridCoordinates(x, y), new GridCoordinates(x, y + SegmentLength), segments);
            }
        }

        for (var y = footprint.Ymin; y <= footprint.Ymax; y++)
        {
            for (var x = footprint.Xmin - 1; x <= footprint.Xmax - SegmentLength + 1; x++)
            {
                TryAddSegment(new GridCoordinates(x, y), new GridCoordinates(x + SegmentLength, y), segments);
            }
        }

        for (var x = footprint.Xmin - 1; x <= footprint.Xmax - SegmentLength + 1; x++)
        {
            for (var y = footprint.Ymin - 1; y <= footprint.Ymax - SegmentLength + 1; y++)
            {
                TryAddSegment(new GridCoordinates(x, y), new GridCoordinates(x + SegmentLength, y + SegmentLength), segments);
                TryAddSegment(new GridCoordinates(x, y + SegmentLength), new GridCoordinates(x + SegmentLength, y), segments);
            }
        }

        return segments;
    }
    
    public List<Segment> FindNewSegments(GridCoordinates lastDot)
    {
        throw new NotImplementedException();
    }

    public void Undo(int steps = 1)
    {
        Grid.Undo(steps);
        Image.Load(Grid, SegmentLength, NoTouchingRule);
    }

    public int SvgWidth(GridFootprint footprint)
    {
        return PixelsPerUnit * (footprint.Xmax - footprint.Xmin + 1);
    }

    public int SvgHeight(GridFootprint footprint)
    {
        return PixelsPerUnit * (footprint.Ymax - footprint.Ymin + 1);
    }
    
    public string SvgViewBox(GridFootprint footprint)
    {
        return $"{footprint.Xmin - 0.5:F1} {footprint.Ymin - 0.5:F1} " +
               $"{footprint.Xmax - footprint.Xmin + 1} {footprint.Ymax - footprint.Ymin + 1}";
    }

    public string SvgBackground(GridFootprint footprint, bool grouped = false)
    {
        var width = footprint.Xmax - footprint.Xmin + 1;
        var height = footprint.Ymax - footprint.Ymin + 1;
        var minX = footprint.Xmin - 0.5;
        var maxX = footprint.Xmax + 0.5;
        var minY = footprint.Ymin - 0.5;
        var maxY = footprint.Ymax + 0.5;
        
        var result = $"<rect width=\"{width}\" height=\"{height}\" "
                          + $"x=\"{minX}\" y=\"{minY}\" style=\"fill:white\" />";
        
        const string gridStyle = "stroke:lightgray;stroke-width:0.1";
        for (var i = 0; i < width; i++)
        {
            var x = footprint.Xmin + i;
            result += $"<line x1=\"{x}\" y1=\"{minY}\" x2=\"{x}\" y2=\"{maxY}\" style=\"{gridStyle}\" />";
        }
        for (var i = 0; i < height; i++)
        {
            var y = footprint.Ymin + i;
            result += $"<line x1=\"{minX}\" y1=\"{y}\" x2=\"{maxX}\" y2=\"{y}\" style=\"{gridStyle}\" />";
        }

        return (grouped) ? "<g>" + result + "</g>" : result;
    }
    
    public string ToSvg(bool crop = false)
    {
        var footprint = (crop) ? Grid.GetFootprint() : Image.GetFootprint();

        return $"<svg width=\"{SvgWidth(footprint)}\" height=\"{SvgHeight(footprint)}\" " +
               $"viewbox=\"{SvgViewBox(footprint)}\">" +
               SvgBackground(footprint) +
               Grid.ToSvg() +
               "</svg>";
    }
    
    public string ToJson()
    {
        var json = new GameDto(this);
        var options = new JsonSerializerOptions { WriteIndented = true };
        return JsonSerializer.Serialize(json, options);
    }

    public void Save(string file, bool overwrite = false)
    {
        if (!overwrite && File.Exists(file))
        {
            throw new Exception($"File '{file}' exists already.");
        }
        var jsonString = ToJson();
        using (var outputFile = new StreamWriter(file))
        {
            outputFile.Write(jsonString);
        }
    }
    
    public static Game FromJson(string json)
    {
        var gameJson = JsonSerializer.Deserialize<GameDto>(json);
        if (gameJson is null)
        {
            throw new Exception("Could not parse JSON file.");
        }
        var game = gameJson.ToGame();
        return game;
    }

    public static Game Load(string file)
    {
        if (!File.Exists(file))
        {
            throw new Exception($"File '{file}' cannot be found.");
        }
        var jsonString = "";
        using (var reader = new StreamReader(file))
        {
            jsonString = reader.ReadToEnd();
        }
        
        return FromJson(jsonString);
    }
    
    
}