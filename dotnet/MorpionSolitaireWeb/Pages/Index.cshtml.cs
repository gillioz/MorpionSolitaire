using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MorpionSolitaire;
using System.Text;
using MorpionSolitaireGraph;

namespace MorpionSolitaireWeb.Pages;

public class IndexModel : PageModel
{
    public GameGraph GetSessionGame()
    {
        return SessionManager.Restore(HttpContext.Session);
    }

    public void OnGet()
    {
        SessionManager.Clean();
    }

    public IActionResult OnPostDownload()
    {
        var game = GetSessionGame();
        var json = game.Grid.ToJson();
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
            
            var game = new GameGraph(GridDto.FromJson(jsonString).ToGrid());
            SessionManager.Assign(HttpContext.Session, game);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public IActionResult OnGetTrySegment(string x1, string y1, string x2, string y2)
    {
        var game = GetSessionGame();
        var success = game.TryPlay(new GridCoordinates(int.Parse(x1), int.Parse(y1)),
            new GridCoordinates(int.Parse(x2), int.Parse(y2)));
        if (success)
        {
            var response = new AddToGridAjaxResponse(game);
            if (game.Nodes.Peek().Branches.Count == 0)
            {
                response.Message = "Game over.";
            }
            return response.ToJsonResult();
        }

        return new AjaxResponse(game).ToJsonResult();
    }

    public IActionResult OnGetRestart()
    {
        var game = GetSessionGame();
        game.Restart();
        return new ReplaceGridAjaxResponse(game).ToJsonResult();
    }

    public IActionResult OnGetReload()
    {
        var game = GetSessionGame();
        return new ReplaceGridAjaxResponse(game).ToJsonResult();
    }

    public IActionResult OnGetUndo()
    {
        var game = GetSessionGame();
        game.Undo();
        return new ReplaceGridAjaxResponse(game).ToJsonResult();
    }

    public IActionResult OnGetUndoFive()
    {
        var game = GetSessionGame();
        game.Undo(5);
        return new ReplaceGridAjaxResponse(game).ToJsonResult();
    }
    
    private class AjaxResponse
    {
        public string Type { get; set; }
        public int Score { get; set; }
        public string Message { get; set; }

        public AjaxResponse(GameGraph game)
        {
            Type = "None";
            Score = game.GetScore();
            Message = string.Empty;
        }

        public JsonResult ToJsonResult()
        {
            return new JsonResult(this);
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