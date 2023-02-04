using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MorpionSolitaire;
using MorpionSolitaireGraph;

namespace MorpionSolitaireWeb.Pages;

public class ExploreModel : PageModel
{
    public GameGraph Game;
    public string ErrorMessage;

    public ExploreModel()
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
        RestoreSession();
    }

    public IActionResult OnGetLoad()
    {
        RestoreSession();
        return new AjaxResponse(Game).ToJsonResult();
    }

    public IActionResult OnGetPlay(string id)
    {
        RestoreSession();
        var index = int.Parse(id);
        if (index >= 0 && index < Game.Nodes.Peek().Branches.Count)
        {
            Game.Play(index);
        }
        return new AjaxResponse(Game).ToJsonResult();
    }

    public IActionResult OnGetPlayOneAtRandom()
    {
        RestoreSession();
        Game.PlayAtRandom(1);
        return new AjaxResponse(Game).ToJsonResult();
    }

    public IActionResult OnGetPlayAtRandom()
    {
        RestoreSession();
        Game.PlayAtRandom();
        return new AjaxResponse(Game).ToJsonResult();
    }

    public IActionResult OnGetUndo()
    {
        RestoreSession();
        Game.Undo();
        return new AjaxResponse(Game).ToJsonResult();
    }

    public IActionResult OnGetUndoFive()
    {
        RestoreSession();
        Game.Undo(5);
        return new AjaxResponse(Game).ToJsonResult();
    }

    public IActionResult OnGetRestart()
    {
        RestoreSession();
        Game.Restart();
        return new AjaxResponse(Game).ToJsonResult();
    }

    private class AjaxResponse
    {
        public int Score { get; }
        public string Grid { get; }
        public List<string> Buttons { get; }

        public AjaxResponse(GameGraph gameGraph)
        {
            Score = gameGraph.GetScore();
            Grid = gameGraph.ToSvg();
            Buttons = new List<string>();
            var branches = gameGraph.Nodes.Peek().Branches;
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