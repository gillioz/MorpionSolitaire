using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using MorpionSolitaire;

namespace MorpionSolitaireWeb.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public Game Game { get; set; }
    public int Xmin { get; set; }
    public int Xmax { get; set; }
    public int Ymin { get; set; }
    public int Ymax { get; set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        Game = new Game();
        // Game.TrySegment(new GridCoordinates(-1, 3), new GridCoordinates(3, 3));
        // Game.TrySegment(new GridCoordinates(-1, 3), new GridCoordinates(3, 7));
        var footprint = Game.Image.GetFootprint();  
        Xmin = footprint.Xmin;
        Xmax = footprint.Xmax;
        Ymin = footprint.Ymin;
        Ymax = footprint.Ymax;
    }

    public void OnGet()
    {}

    public IActionResult OnGetTrySegment(string x1, string y1, 
        string x2, string y2)
    {
        var success = Game.TrySegment(new GridCoordinates(int.Parse(x1), int.Parse(y1)), 
            new GridCoordinates(int.Parse(x2), int.Parse(y2)));
        if (success)
        {
            return new JsonResult(new
            {
                action = "add",
                content = Game.Grid.Actions.Last().ToSvg()
            });
        }
        return new JsonResult(new
        {
            action = "none"
        });
    }
    
    public IActionResult OnGetRestart()
    {
        Game = new Game();
        return new JsonResult(new
        {
            action = "replace",
            content = Game.Grid.ToSvg()
        });
    }
}