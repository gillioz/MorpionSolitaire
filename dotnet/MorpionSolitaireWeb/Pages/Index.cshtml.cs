using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using MorpionSolitaire;

namespace MorpionSolitaireWeb.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public string GridSvg;
    public SvgDocument SvgDoc { get; set; }
    public Game Game { get; set; }
    public string DebugText { get; set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        GridSvg = string.Empty;
        Game = new Game();
        SvgDoc = new SvgDocument("grid");
        DebugText = "application startup";
    }

    public void OnGet()
    {
        // Game.TrySegment(new GridCoordinates(0, 4), new GridCoordinates(4, 0));
        Game.TrySegment(new GridCoordinates(-1, 3), new GridCoordinates(3, 3));

        Game.Image.SetSvgDimensions(SvgDoc);
        SvgDoc.DrawGrid();
        Game.Grid.AddToSvgDoc(SvgDoc);

        DrawSvg();
        
        DebugText = "OnGet";
    }

    public void OnGetTrySegment(int x1, int y1, int x2, int y2)
    {
        Game.TrySegment(new GridCoordinates(0, 4), new GridCoordinates(4, 0));
        DrawSvg();

        DebugText = "OnGetTrySegment";
    }
    
    public void OnGetDebug(string msg)
    {
        Game.TrySegment(new GridCoordinates(0, 4), new GridCoordinates(4, 0));
        DebugText = "OnGetDebug"; 
    }

    private void DrawSvg()
    {
        var text = new StringBuilder();
        SvgDoc.Document.Save(text);
        GridSvg = text.ToString();
    }
}