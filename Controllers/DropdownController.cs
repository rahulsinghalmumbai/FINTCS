using FINTCS.Models;
using FINTCS.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

public class DropdownController : Controller
{
    private readonly IGenericRepository<TBLDEF> _repoDef;
    private readonly IGenericRepository<TBLDAT> _repoDat;

    public DropdownController(
        IGenericRepository<TBLDEF> repoDef,
        IGenericRepository<TBLDAT> repoDat)
    {
        _repoDef = repoDef;
        _repoDat = repoDat;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _repoDef.GetAll();
        return View(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetTBLDAT(int dtblSern)
    {
        var data = await _repoDat.GetListAsync(x => x.DTBLSERN == dtblSern);

        var result = data.Select(x => new
        {
            id = x.TBLSSERN,       // ← correct primary key
            desc = x.TBLSDESC
        });

        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> SaveData(int DTBLSERN, string TBLSDESC, int? TBLSSERN)
    {
        if (string.IsNullOrWhiteSpace(TBLSDESC))
        {
            return Json(new { success = false, message = "Description is required" });
        }
        if (TBLSSERN.HasValue && TBLSSERN.Value > 0)
        {
            // UPDATE existing
            var existing = await _repoDat.GetByIdAsync(TBLSSERN.Value);
            if (existing != null)
            {
                existing.TBLSDESC = TBLSDESC;
                _repoDat.Update(existing);
                await _repoDat.SaveAsync();
            }

        }
        else
        {
            // INSERT new
            var newRecord = new TBLDAT
            {
                DTBLSERN = DTBLSERN,
                TBLSDESC = TBLSDESC
            };
            await _repoDat.AddAsync(newRecord);
            await _repoDat.SaveAsync();
        }

        return Json(new { success = true });
    }

}
