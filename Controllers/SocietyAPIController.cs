using FINTCS.Models;
using FINTCS.Repositories;
using FINTCS.ViewModels;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class SocietyAPIController : ControllerBase
{
    private readonly IGenericService<Society> _societyRepo;
    private readonly IGenericService<LoanMaster> _loanRepo;

    public SocietyAPIController(
        IGenericService<Society> societyRepo,
        IGenericService<LoanMaster> loanRepo)
    {
        _societyRepo = societyRepo;
        _loanRepo = loanRepo;
    }

    [HttpGet("get")]
    public async Task<IActionResult> Get()
    {
        var society = await _societyRepo.GetFirstAsync();
        if (society == null) return NotFound();

        var loans = await _loanRepo.GetListAsync(x => x.SocietyId == society.Id);
        return Ok(new { society, loans });
    }

    [HttpPost("upsert")]
    public async Task<IActionResult> Upsert([FromBody] SocietyLoanVM vm)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        bool isNew = vm.Society.Id == 0;

        // SP Call for Society
        var newSocId = await _societyRepo.UpsertSocietyAsync(vm.Society);
        vm.Society.Id = newSocId;

        // SP Call for Loans
        if (vm.LoanList != null && vm.LoanList.Count > 0)
        {
            foreach (var loan in vm.LoanList)
            {
                loan.SocietyId = vm.Society.Id;
                var newLoanId = await _loanRepo.UpsertLoanMasterAsync(loan);
                loan.LoanMasterId = newLoanId;
            }
        }

        return Ok(new { message = isNew ? "Saved Successfully ✔" : "Updated Successfully ✔", societyId = vm.Society.Id });
    }
}
