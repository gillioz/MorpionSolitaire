using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MorpionSolitaire;
using MorpionSolitaireGraph;
using System.Collections.ObjectModel;
using System.Text;

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
}