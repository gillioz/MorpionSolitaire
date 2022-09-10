using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using MorpionSolitaire;

namespace MorpionSolitaireWeb.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public string GridSvg;
    public Game Game { get; set; }
    public string DebugText { get; set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        GridSvg = string.Empty;
        Game = new Game();
        DebugText = "application startup";
    }

    public void OnGet()
    {
        // Game.TrySegment(new GridCoordinates(0, 4), new GridCoordinates(4, 0));
        Game.TrySegment(new GridCoordinates(-1, 3), new GridCoordinates(3, 3));

        // Game.Grid.ToSvg("grid");
        Game.Grid.ToSvg("grid", Game.Image);

        DebugText = "OnGet";
    }

    public void OnGetTrySegment(int x1, int y1, int x2, int y2)
    {
        Game.TrySegment(new GridCoordinates(0, 4), new GridCoordinates(4, 0));

        DebugText = "OnGetTrySegment";
    }
    
    public void OnGetDebug(string msg)
    {
        Game.TrySegment(new GridCoordinates(0, 4), new GridCoordinates(4, 0));
        DebugText = "OnGetDebug"; 
    }}