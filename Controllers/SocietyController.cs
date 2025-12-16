using FINTCS.Models;
using FINTCS.Repositories;
using FINTCS.ViewModels;
using Microsoft.AspNetCore.Mvc;

public class SocietyController : Controller
{
    private readonly IGenericService<Society> _societyRepo;
    private readonly IGenericService<LoanMaster> _loanRepo;

    public SocietyController(IGenericService<Society> societyRepo,
                               IGenericService<LoanMaster> loanRepo)
    {
        _societyRepo = societyRepo;
        _loanRepo = loanRepo;
    }

    [HttpGet]
    public async Task<IActionResult> Upsert()
    {
        var society = await _societyRepo.GetFirstAsync();
        var vm = new SocietyLoanVM
        {
            Society = society ?? new Society(),
            LoanMasters = society != null ? await _loanRepo.GetListAsync(x => x.SocietyId == society.Id) : new List<LoanMaster>()
        };
        return View(vm); // ✅ View return possible
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(SocietyLoanVM vm)
    {
        if (!ModelState.IsValid) return View(vm);

        bool isNew = vm.Society.Id == 0;

        // SP call via repository
        var newSocId = await _societyRepo.UpsertSocietyAsync(vm.Society);
        vm.Society.Id = newSocId;

        if (vm.LoanList != null && vm.LoanList.Count > 0)
        {
            foreach (var loan in vm.LoanList)
            {
                loan.SocietyId = vm.Society.Id;
                await _loanRepo.UpsertLoanMasterAsync(loan);
            }
        }

        TempData["msg"] = isNew ? "Saved Successfully ✔" : "Updated Successfully ✔";
        return RedirectToAction("Upsert");
    }
}
