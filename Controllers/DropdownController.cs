using FINTCS.Models;
using FINTCS.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

public class DropdownController : Controller
{
    private readonly IGenericService<TBLDEF> _repoDef;
    private readonly IGenericService<TBLDAT> _repoDat;

    public DropdownController(
        IGenericService<TBLDEF> repoDef,
        IGenericService<TBLDAT> repoDat)
    {
        _repoDef = repoDef;
        _repoDat = repoDat;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _repoDef.GetDropdownListAsync();
        return View(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetTBLDAT(int dtblSern)
    {
        var data = await _repoDat.GetListAsync(x => x.DTBLSERN == dtblSern);

        var result = data.Select(x => new
        {
            id = x.TBLSSERN,
            desc = x.TBLSDESC
        });

        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> SaveData(int DTBLSERN, string TBLSDESC, int? TBLSSERN)
    {
        if (string.IsNullOrWhiteSpace(TBLSDESC))
            return Json(new { success = false, message = "Description is required" });

        var tblDat = new TBLDAT
        {
            TBLSSERN = TBLSSERN ?? 0,
            DTBLSERN = DTBLSERN,
            TBLSDESC = TBLSDESC
        };

        int newId = await _repoDat.UpsertTBLDATAsync(tblDat);

        return Json(new { success = true, id = newId });
    }
}
