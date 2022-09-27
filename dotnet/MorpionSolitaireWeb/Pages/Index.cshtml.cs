using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MorpionSolitaire;

namespace MorpionSolitaireWeb.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public static Game Game { get; set; } = new Game();
    public static GridFootprint Footprint { get; set; } = new GridFootprint();
    public string ErrorMessage { get; set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        ErrorMessage = "";
    }

    public void OnGet() {
        Game = new Game();
        Footprint = Game.Image.GetFootprint();
        ErrorMessage = "";
    }

    public ActionResult OnPostDownload()
    {
        var jsonString = Game.ToJson();
        var bytes = Encoding.UTF8.GetBytes(jsonString);
        var file = "MorpionSolitaire-" +
            DateTime.Now.ToString("yyyy-MM-dd-HHmm") +
            ".json";
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
            if (file.Length > 200000)
            {
                throw new Exception("File size cannot exceed 200kb.");
            } 
            string jsonString;
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
            return new AddToGridAjaxResponse().ToJsonResult();
        }

        return new AjaxResponse().ToJsonResult();
    }

    public IActionResult OnGetRestart()
    {
        Game = new Game();
        Footprint = Game.Image.GetFootprint();
        return new ReplaceGridAjaxResponse().ToJsonResult();
    }

    public IActionResult OnGetReload()
    {
        Footprint = Game.Image.GetFootprint();
        return new ReplaceGridAjaxResponse().ToJsonResult();
    }

    public IActionResult OnGetUndo()
    {
        Game.Undo();
        return new ReplaceGridAjaxResponse().ToJsonResult();
    }

    public IActionResult OnGetUndoFive()
    {
        Game.Undo(5);
        return new ReplaceGridAjaxResponse().ToJsonResult();
    }
    
    private class AjaxResponse
    {
        public string Type { get; set; }
        public int Score { get; set; }

        public AjaxResponse()
        {
            Type = "None";
            Score = Game.GetScore();
        }

        public JsonResult ToJsonResult()
        {
            return new JsonResult(this);
        }
    }

    private class AlertAjaxResponse : AjaxResponse
    {
        public string Message { get; }

        public AlertAjaxResponse(string message)
        {
            Type = "Alert";
            Message = message;
        }
    }

    private class AddToGridAjaxResponse : AjaxResponse
    {
        public string NewElement { get; }

        public AddToGridAjaxResponse()
        {
            Type = "Add";
            NewElement = Game.Grid.Actions.Last().ToSvg();
        }
    }

    private class ReplaceGridAjaxResponse : AjaxResponse
    {
        public string GridContent { get; }
        public int MinX { get; }
        public int MinY { get; }

        public ReplaceGridAjaxResponse()
        {
            Type = "Replace";
            GridContent = Game.ToSvg();
            MinX = Footprint.Xmin;
            MinY = Footprint.Ymin;
        }
    }
}