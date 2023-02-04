using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MorpionSolitaire;
using MorpionSolitaireGraph;

namespace MorpionSolitaireWeb.Pages;

public class ExploreModel : PageModel
{
    public string ErrorMessage;

    public ExploreModel()
    {
        ErrorMessage = "";
    }

    public GameGraph GetSessionGame()
    {
        return SessionManager.Restore(HttpContext.Session);
    }

    public IActionResult OnGetLoad()
    {
        var game = GetSessionGame();
        return new AjaxResponse(game).ToJsonResult();
    }

    public IActionResult OnGetPlay(string id)
    {
        var game = GetSessionGame();
        var index = int.Parse(id);
        if (index >= 0 && index < game.Nodes.Peek().Branches.Count)
        {
            game.Play(index);
        }
        return new AjaxResponse(game).ToJsonResult();
    }

    public IActionResult OnGetPlayOneAtRandom()
    {
        var game = GetSessionGame();
        game.PlayAtRandom(1);
        return new AjaxResponse(game).ToJsonResult();
    }

    public IActionResult OnGetPlayAtRandom()
    {
        var game = GetSessionGame();
        game.PlayAtRandom();
        return new AjaxResponse(game).ToJsonResult();
    }

    public IActionResult OnGetUndo()
    {
        var game = GetSessionGame();
        game.Undo();
        return new AjaxResponse(game).ToJsonResult();
    }

    public IActionResult OnGetUndoFive()
    {
        var game = GetSessionGame();
        game.Undo(5);
        return new AjaxResponse(game).ToJsonResult();
    }

    public IActionResult OnGetRestart()
    {
        var game = GetSessionGame();
        game.Restart();
        return new AjaxResponse(game).ToJsonResult();
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