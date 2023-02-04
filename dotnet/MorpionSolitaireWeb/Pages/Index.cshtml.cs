using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MorpionSolitaire;
using System.Text;
using MorpionSolitaireGraph;

namespace MorpionSolitaireWeb.Pages;

public class IndexModel : PageModel
{
    public GameGraph Game { get; set; }
    public string ErrorMessage { get; set; }

    public IndexModel()
    {
        Game = new GameGraph(Grid.Cross());
        ErrorMessage = "";
    }

    private void RestoreSession()
    {
        Game = SessionManager.Restore(HttpContext.Session);
    }
    
    public GridFootprint Footprint()
    {
        return Game.GetFootPrint();
    }
    
    public void OnGet()
    {
        SessionManager.Clean();
        RestoreSession();
        ErrorMessage = "";
    }

    public ActionResult OnPostDownload()
    {
        RestoreSession();
        var json = Game.Grid.ToJson();
        var bytes = Encoding.UTF8.GetBytes(json);
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

            RestoreSession();
            Game = new GameGraph(GridDto.FromJson(jsonString).ToGrid());
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
        RestoreSession();
        var success = Game.TryPlay(new GridCoordinates(int.Parse(x1), int.Parse(y1)),
            new GridCoordinates(int.Parse(x2), int.Parse(y2)));
        if (success)
        {
            return new AddToGridAjaxResponse(Game).ToJsonResult();
        }

        return new AjaxResponse(Game).ToJsonResult();
    }

    public IActionResult OnGetRestart()
    {
        RestoreSession();
        Game.Restart();
        return new ReplaceGridAjaxResponse(Game).ToJsonResult();
    }

    public IActionResult OnGetReload()
    {
        RestoreSession();
        return new ReplaceGridAjaxResponse(Game).ToJsonResult();
    }

    public IActionResult OnGetUndo()
    {
        RestoreSession();
        Game.Undo();
        return new ReplaceGridAjaxResponse(Game).ToJsonResult();
    }

    public IActionResult OnGetUndoFive()
    {
        RestoreSession();
        Game.Undo(5);
        return new ReplaceGridAjaxResponse(Game).ToJsonResult();
    }
    
    private class AjaxResponse
    {
        public string Type { get; set; }
        public int Score { get; set; }

        public AjaxResponse(GameGraph game)
        {
            Type = "None";
            Score = game.GetScore();
        }

        public JsonResult ToJsonResult()
        {
            return new JsonResult(this);
        }
    }

    private class AlertAjaxResponse : AjaxResponse
    {
        public string Message { get; }

        public AlertAjaxResponse(GameGraph game, string message) : base(game)
        {
            Type = "Alert";
            Message = message;
        }
    }

    private class AddToGridAjaxResponse : AjaxResponse
    {
        public string NewElement { get; }

        public AddToGridAjaxResponse(GameGraph game) : base(game)
        {
            Type = "Add";
            NewElement = game.Grid.Actions.Peek().ToSvg();
        }
    }

    private class ReplaceGridAjaxResponse : AjaxResponse
    {
        public string GridContent { get; }
        public int MinX { get; }
        public int MinY { get; }

        public ReplaceGridAjaxResponse(GameGraph game) : base(game)
        {
            Type = "Replace";
            GridContent = game.ToSvg();
            var footprint = game.GetFootPrint();
            MinX = footprint.Xmin;
            MinY = footprint.Ymin;
        }
    }
}