using FINTCS.Repository;
using Microsoft.AspNetCore.Mvc;

public class LedgerController : Controller
{
    private readonly IAccountService _service;

    public LedgerController(IAccountService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        ViewBag.Groups = _service.GetGroups();
        ViewBag.Ledgers = _service.GetLedgers();
        ViewBag.GroupLedgers = _service.GetLedgersWithGroup();
        return View();
    }

    [HttpPost]
    public IActionResult SaveLedger(LEDGERAC model, string mode)
    {
        if (mode == "new")
        {
            // 🔹 New Ledger
            model.LEDGERCODE = _service.GenerateLedgerCode(model.GroupId);
            _service.SaveLedger(model);

            // ✅ Success message
            TempData["Success"] = "Ledger saved successfully";
        }
        else
        {
            // 🔹 Update Ledger
            _service.UpdateLedgerWithCodeAdjust(model);

            // ✅ Update message
            TempData["Success"] = "Ledger updated successfully";
        }

        return RedirectToAction("Index");
    }


    public JsonResult GetLedger(int id)
    {
        var data = _service.GetLedgerById(id);
        return Json(data);
    }
}
