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
    public IActionResult SaveLedger(LEDGERAC model, string mode, bool MemberSpecific, bool IsBank)
    {
        model.Bank = IsBank;

        // Bank checked => MemberSpecific false
        if (IsBank)
        {
            MemberSpecific = false;
            model.MemSpecific = false;
        }

        if (mode == "new")
        {
            if (MemberSpecific)
            {
                _service.SaveMemberSpecificLedger(model);
                model.Bank = false;
                TempData["Success"] = "Member specific ledgers created successfully";
            }
            else
            {
                model.LEDGERCODE = _service.GenerateLedgerCode(model.GroupId);
                _service.SaveLedger(model);
                TempData["Success"] = "Ledger saved successfully";
            }
        }
        else
        {
            _service.UpdateLedgerWithCodeAdjust(model);
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
