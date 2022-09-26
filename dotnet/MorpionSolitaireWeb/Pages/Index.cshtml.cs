using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MorpionSolitaire;

namespace MorpionSolitaireWeb.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public static Game Game { get; set; }
    public static GridFootprint Footprint { get; set; }
    public string ErrorMessage { get; set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        Game = new Game();
        Footprint = Game.Image.GetFootprint();
        ErrorMessage = "";
    }

    public ActionResult OnPostDownload()
    {
        var jsonString = Game.ToJson();
        var bytes = Encoding.UTF8.GetBytes(jsonString);
        var dateTime = System.DateTime.Now;
        var file = 
            $"MorpionSolitaire-{dateTime.Year}-{dateTime.Month}-{dateTime.Day}" +
            $"-{dateTime.Hour}-{dateTime.Minute}.json";
        return File(bytes, "application/json", file);
    }

    public void OnPostUpload(IFormFile file)
    {
        try
        {
            if (file is null || file.Length == 0)
            {
                throw new Exception("No file selected.");
            }
            if (file.ContentType != "application/json")
            {
                throw new Exception("Invalid file type");
            }
            if (file.Length > (long)100000)
            {
                throw new Exception("File size cannot exceed 100kb.");
            }
            var jsonString = "";
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                jsonString = reader.ReadToEnd();
            }
            
            Game = Game.FromJson(jsonString);
            Footprint = Game.Image.GetFootprint();
            ErrorMessage = "";
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        ErrorMessage = $"Failed to load file '{file?.FileName}'";
    }

    public IActionResult OnGetTrySegment(string x1, string y1, string x2, string y2)
    {
        var success = Game.TrySegment(new GridCoordinates(int.Parse(x1), int.Parse(y1)),
            new GridCoordinates(int.Parse(x2), int.Parse(y2)));
        if (success)
        {
            return new AddToGridResponse().ToJsonResult();
        }

        return new Response().ToJsonResult();
    }

    public IActionResult OnGetRestart()
    {
        Game = new Game();
        Footprint = Game.Image.GetFootprint();
        return new ReplaceGridResponse().ToJsonResult();
    }

    public IActionResult OnGetUndo()
    {
        Game.Undo();
        return new ReplaceGridResponse().ToJsonResult();
    }

    public IActionResult OnGetUndoFive()
    {
        Game.Undo(5);
        return new ReplaceGridResponse().ToJsonResult();
    }

    public IActionResult OnGetResize()
    {
        Footprint = Game.Image.GetFootprint();
        return new ResizeGridResponse().ToJsonResult();
    }
    
    private class Response
    {
        public string Type { get; set; }
        public int Score { get; set; }

        public Response()
        {
            Type = "None";
            Score = Game.GetScore();
        }

        public JsonResult ToJsonResult()
        {
            return new JsonResult(this);
        }
    }

    private class AlertResponse : Response
    {
        public string Message { get; }

        public AlertResponse(string message)
        {
            Type = "Alert";
            Message = message;
        }
    }

    private class AddToGridResponse : Response
    {
        public string Content { get; }

        public AddToGridResponse()
        {
            Type = "Add";
            Content = Game.Grid.Actions.Last().ToSvg();
        }
    }

    private class ReplaceGridResponse : Response
    {
        public string Content { get; }

        public ReplaceGridResponse()
        {
            Type = "Replace";
            Content = Game.Grid.ToSvg();
        }
    }

    private class ResizeGridResponse : Response
    {
        public int Width { get; }
        public int Height { get; }
        public string ViewBox { get; }
        public string Background { get; }
        public int MinX { get; }
        public int MinY { get; }

        public ResizeGridResponse()
        {
            Type = "Resize";
            Width = Game.SvgWidth(Footprint);
            Height = Game.SvgHeight(Footprint);
            ViewBox = Game.SvgViewBox(Footprint);
            Background = Game.SvgBackground(Footprint);
            MinX = Footprint.Xmin;
            MinY = Footprint.Ymin;
        }
    }
}