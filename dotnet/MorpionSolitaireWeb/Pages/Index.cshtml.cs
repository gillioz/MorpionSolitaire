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
    public int Xmin { get; set; }
    public int Xmax { get; set; }
    public int Ymin { get; set; }
    public int Ymax { get; set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        GridSvg = string.Empty;
        Game = new Game();
        var footprint = Game.Image.GetFootprint();
        Xmin = footprint.Xmin;
        Xmax = footprint.Xmax;
        Ymin = footprint.Ymin;
        Ymax = footprint.Ymax;
    }

    public void OnGet()
    {
        // Game.TrySegment(new GridCoordinates(0, 4), new GridCoordinates(4, 0));
        Game.TrySegment(new GridCoordinates(-1, 3), new GridCoordinates(3, 3));

        // Game.Grid.ToSvg("grid");
        Game.Grid.ToSvg("grid", Game.Image);
    }

    public void OnGetTrySegment(int x1, int y1, int x2, int y2)
    {
        Game.TrySegment(new GridCoordinates(0, 4), new GridCoordinates(4, 0));
    }
    
    public void OnGetDebug()
    {
        Game.TrySegment(new GridCoordinates(0, 4), new GridCoordinates(4, 0));
    }

    public string GridString()
    {
        return Game.Grid.ToSvg("grid", Game.Image, "\t\t");
    }
}