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
        Image = new Image(dimensions: new GridCoordinates(24, 24),
            origin: new GridCoordinates(8, 8));

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

            Image.SaveBitmap("before");
            var segment = new Segment(line.Pt1, line.Pt2, Image, SegmentLength, NoTouchingRule, pt);
            Apply(segment);
            Image.SaveBitmap("after");
        }
    }

    public void Apply(GameAction action)
    {
        Grid.Apply(action.GridAction);
        Image.Apply(action.ImageAction);
    }

    public bool TrySegment(GridCoordinates pt1, GridCoordinates pt2)
    {
        try
        {
            var segment = new Segment(pt1, pt2, Image, SegmentLength, NoTouchingRule);
            Apply(segment);
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
    
    public string ToSvg(string? id = null, string spacing = "", bool crop = false)
    {
        var footprint = (crop) ? Grid.GetFootprint() : Image.GetFootprint();
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
        result += spacing + $"\t\t<rect width=\"{width}\" height=\"{height}\" "
                          + $"x=\"{minX}\" y=\"{minY}\" "
                          + "style=\"fill:white\" />\n";
        var gridstyle = "stroke:lightgray;stroke-width:0.1";
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

        result += Grid.ToSvg(spacing + '\t');
        
        result += spacing + "</svg>";
        return result;
    }
    
    public string ToJson()
    {
        var json = new GameDto(this);
        var options = new JsonSerializerOptions { WriteIndented = true };
        return JsonSerializer.Serialize(json, options);
        // return "{\n" +
        //        "\t\"title\": \"Morpion Solitaire\",\n" +
        //        "\t\"version\": \"v1\",\n" +
        //        $"\t\"segment_length\": {SegmentLength},\n" +
        //        $"\t\"no_touching\": \"{NoTouchingRule.ToString()}\",\n" +
        //        "\t\"grid\":\n"
        //        + Grid.ToJson("\t") + "\n" +
        //        "}";
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
        using (var inputFile = new StreamReader(file))
        {
            jsonString = inputFile.ReadToEnd();
        }
        
        return FromJson(jsonString);
    }
    
    
}