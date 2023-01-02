using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MorpionSolitaire;
using MorpionSolitaireGraph;
using System.Collections.ObjectModel;

namespace MorpionSolitaireWeb.Pages;

public class ExploreModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    
    public GameGraph GameGraph { get; set; }
    public string ErrorMessage { get; set; }

    
    public static Dictionary<string, GameGraph> GameGraphes = new Dictionary<string, GameGraph>();
    public static Collection<string> ActiveSessions = new Collection<string>();

    public ExploreModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        GameGraph = new GameGraph();
    }

    private string RestoreSession()
    {
        var sessionId = HttpContext.Session.GetString("GraphID") ?? Guid.Empty.ToString();
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
        return GameGraph.Game.GetFootPrint();
    }
    
    public void OnGet()
    {
        var sessionId = HttpContext.Session.GetString("GraphID");
        if (sessionId is null)
        {
            sessionId = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("GraphID", sessionId);
            GameGraph = new GameGraph();
            GameGraphes[sessionId] = GameGraph;
        }
        else
        {
            GameGraph = GameGraphes[sessionId];
        }
        ActiveSessions.Add(sessionId);
    }

    public IActionResult OnGetLoad()
    {
        RestoreSession();
        return new AjaxResponse(GameGraph).ToJsonResult();
    }

    public IActionResult OnGetPlay(string id)
    {
        RestoreSession();
        var index = int.Parse(id);
        if (index >= 0 && index < GameGraph.Node.Branches.Count)
        {
            GameGraph.Play(index);
        }
        return new AjaxResponse(GameGraph).ToJsonResult();
    }

    public IActionResult OnGetPlayOneAtRandom()
    {
        RestoreSession();
        GameGraph.PlayAtRandom(1);
        return new AjaxResponse(GameGraph).ToJsonResult();
    }

    public IActionResult OnGetPlayAtRandom()
    {
        RestoreSession();
        GameGraph.PlayAtRandom();
        return new AjaxResponse(GameGraph).ToJsonResult();
    }

    public IActionResult OnGetUndo()
    {
        RestoreSession();
        GameGraph.Undo();
        return new AjaxResponse(GameGraph).ToJsonResult();
    }

    public IActionResult OnGetUndoFive()
    {
        RestoreSession();
        GameGraph.Undo(5);
        return new AjaxResponse(GameGraph).ToJsonResult();
    }

    public IActionResult OnGetRestart()
    {
        RestoreSession();
        GameGraph.Restart();
        return new AjaxResponse(GameGraph).ToJsonResult();
    }

    private class AjaxResponse
    {
        public int Score { get; }
        public string Grid { get; }
        public List<string> Buttons { get; }

        public AjaxResponse(GameGraph gameGraph)
        {
            Score = gameGraph.Game.GetScore();
            Grid = gameGraph.Game.ToSvg();
            Buttons = new List<string>();
            var branches = gameGraph.Node.Branches;
            for (int i = 0; i < branches.Count; i++)
            {
                Buttons.Add($"{i+1} " +
                            $"{branches[i].Line.Pt1.X} {branches[i].Line.Pt1.Y} " +
                            $"{branches[i].Line.Pt2.X} {branches[i].Line.Pt2.Y} " +
                            $"{branches[i].Dot.Pt.X} {branches[i].Dot.Pt.Y}");
            }
        }
        
        public JsonResult ToJsonResult()
        {
            return new JsonResult(this);
        }
    }
}