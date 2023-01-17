using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MorpionSolitaire;
using System.Collections.ObjectModel;
using System.Text;
using MorpionSolitaireGraph;

namespace MorpionSolitaireWeb.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    
    public GameGraph GameGraph { get; set; }
    public string ErrorMessage { get; set; }

    
    public static Dictionary<string, GameGraph> GameGraphes = new Dictionary<string, GameGraph>();
    public static Collection<string> ActiveSessions = new Collection<string>();

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        GameGraph = new GameGraph(Grid.Cross());
        ErrorMessage = "";
    }

    private string RestoreSession()
    {
        var sessionId = HttpContext.Session.GetString("ID") ?? Guid.Empty.ToString();
        GameGraph = GameGraphes[sessionId];
        ActiveSessions.Add(sessionId);
        return sessionId;
    }

    // this must be called every day or so... how?
    private void SessionCleanUp()
    {
        foreach (KeyValuePair<string, GameGraph> keyValuePair in GameGraphes)
        {
            if (!ActiveSessions.Contains(keyValuePair.Key))
            {
                GameGraphes.Remove(keyValuePair.Key);
            }
        }
        ActiveSessions.Clear();
    }
    
    public GridFootprint Footprint()
    {
        return GameGraph.GetFootPrint();
    }
    
    public void OnGet()
    {
        var sessionId = HttpContext.Session.GetString("ID");
        if (sessionId is null)
        {
            sessionId = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("ID", sessionId);
            GameGraph = new GameGraph(Grid.Cross());
            GameGraphes[sessionId] = GameGraph;
        }
        else
        {
            GameGraph = GameGraphes[sessionId];
        }
        ActiveSessions.Add(sessionId);
        ErrorMessage = "";
    }

    public ActionResult OnPostDownload()
    {
        RestoreSession();
        var jsonString = GameGraph.ToJson();
        var bytes = Encoding.UTF8.GetBytes(jsonString);
        var file = "MorpionSolitaire-" +
            DateTime.Now.ToString("yyyy-MM-dd-HHmm") +
            ".json";
        return File(bytes, "application/json", file);
    }

    public void OnPostUpload(IFormFile file)
    {
        var sessionId = RestoreSession();
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

            GameGraph = new GameGraph(GameDto.FromJson(jsonString).ToGrid());
            GameGraphes[sessionId] = GameGraph;
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
        var success = GameGraph.TryPlay(new GridCoordinates(int.Parse(x1), int.Parse(y1)),
            new GridCoordinates(int.Parse(x2), int.Parse(y2)));
        if (success)
        {
            return new AddToGridAjaxResponse(GameGraph).ToJsonResult();
        }

        return new AjaxResponse(GameGraph).ToJsonResult();
    }

    public IActionResult OnGetRestart()
    {
        var sessionId = RestoreSession();
        GameGraph.Restart();
        GameGraphes[sessionId] = GameGraph;
        return new ReplaceGridAjaxResponse(GameGraph).ToJsonResult();
    }

    public IActionResult OnGetReload()
    {
        RestoreSession();
        return new ReplaceGridAjaxResponse(GameGraph).ToJsonResult();
    }

    public IActionResult OnGetUndo()
    {
        RestoreSession();
        GameGraph.Undo();
        return new ReplaceGridAjaxResponse(GameGraph).ToJsonResult();
    }

    public IActionResult OnGetUndoFive()
    {
        RestoreSession();
        GameGraph.Undo(5);
        return new ReplaceGridAjaxResponse(GameGraph).ToJsonResult();
    }
    
    private class AjaxResponse
    {
        public string Type { get; set; }
        public int Score { get; set; }

        public AjaxResponse(Game game)
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

        public AlertAjaxResponse(Game game, string message) : base(game)
        {
            Type = "Alert";
            Message = message;
        }
    }

    private class AddToGridAjaxResponse : AjaxResponse
    {
        public string NewElement { get; }

        public AddToGridAjaxResponse(Game game) : base(game)
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

        public ReplaceGridAjaxResponse(Game game) : base(game)
        {
            Type = "Replace";
            GridContent = game.ToSvg();
            var footprint = game.GetFootPrint();
            MinX = footprint.Xmin;
            MinY = footprint.Ymin;
        }
    }
}