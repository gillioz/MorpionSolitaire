using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MorpionSolitaire;

namespace MorpionSolitaireWeb.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public static Game Game { get; set; } = new Game();
    public int Xmin { get; set; } = 0;
    public int Xmax { get; set; } = 0;
    public int Ymin { get; set; } = 0;
    public int Ymax { get; set; } = 0;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        // Game = new Game();
        Game = Game.Load("Game-0000.json");
        // Game.TrySegment(new GridCoordinates(-1, 3), new GridCoordinates(3, 3));
        // Game.TrySegment(new GridCoordinates(-1, 3), new GridCoordinates(3, 7));
        var footprint = Game.Image.GetFootprint();  
        Xmin = footprint.Xmin;
        Xmax = footprint.Xmax;
        Ymin = footprint.Ymin;
        Ymax = footprint.Ymax;
    }

    public IActionResult OnGetTrySegment(string x1, string y1, string x2, string y2)
    {
        var success = Game.TrySegment(new GridCoordinates(int.Parse(x1), int.Parse(y1)), 
            new GridCoordinates(int.Parse(x2), int.Parse(y2)));
        if (success)
        {
            return new Response(Response.ActionType.Add).Value();
        }
        return new Response(Response.ActionType.None).Value();
    }
    
    public IActionResult OnGetRestart()
    {
        Game = new Game();
        // Game = Game.Load("Game-0000.json");
        return new Response(Response.ActionType.Replace).Value();
    }
    
    public IActionResult OnGetSave()
    {
        Game.Save("Game-0000.json", true);
        return new Response(Response.ActionType.Alert, "Success").Value();
    }

    private class Response
    {
        public string Action { get; }
        public string Content { get; }
        public int? Score { get; }

        public Response(ActionType action, string message = "")
        {
            Action = action.ToString();
            Content = action switch
            {
                ActionType.Add => Game.Grid.Actions.Last().ToSvg(),
                ActionType.Replace => Game.Grid.ToSvg(),
                ActionType.Alert => message,
                _ => ""
            };
            Score = (action == ActionType.None) ? null : Game.GetScore();
        }

        public JsonResult Value()
        {
            return new JsonResult(this);
        }

        public enum ActionType
        {
            None,
            Alert,
            Add,
            Replace
        }
    }
}