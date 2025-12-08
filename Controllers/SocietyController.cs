using FINTCS.Models;
using FINTCS.Repositories;
using FINTCS.ViewModels;
using Microsoft.AspNetCore.Mvc;

public class SocietyController : Controller
{
    private readonly IGenericRepository<Society> _societyRepo;
    private readonly IGenericRepository<LoanMaster> _loanRepo;

    public SocietyController(
        IGenericRepository<Society> societyRepo,
        IGenericRepository<LoanMaster> loanRepo)
    {
        _societyRepo = societyRepo;
        _loanRepo = loanRepo;
    }

    // GET: Load Form
    public async Task<IActionResult> Upsert()
    {
        var society = await _societyRepo.GetFirstAsync();

        var vm = new SocietyLoanVM
        {
            Society = society ?? new Society()
        };

        if (society != null)
        {
            vm.LoanMasters = await _loanRepo.GetListAsync(x => x.SocietyId == society.Id);
        }

        return View(vm);
    }

    // POST: Insert / Update
    [HttpPost]
    public async Task<IActionResult> Upsert(SocietyLoanVM vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        bool isNew = vm.Society.Id == 0;

        // Save / Update Society
        if (isNew)
            await _societyRepo.AddAsync(vm.Society);
        else
            _societyRepo.Update(vm.Society);

        await _societyRepo.SaveAsync();

        // Save / Update Loan List
        if (vm.LoanList != null && vm.LoanList.Count > 0)
        {
            foreach (var loan in vm.LoanList)
            {
                loan.SocietyId = vm.Society.Id;

                if (loan.LoanMasterId == 0)
                {
                    // New Loan → Add
                    await _loanRepo.AddAsync(loan);
                }
                else
                {
                    // Existing Loan → Update
                    _loanRepo.Update(loan);
                }
            }

            await _loanRepo.SaveAsync();
        }

        TempData["msg"] = isNew ? "Saved Successfully ✔" : "Updated Successfully ✔";

        return RedirectToAction("Upsert");
    }
}
