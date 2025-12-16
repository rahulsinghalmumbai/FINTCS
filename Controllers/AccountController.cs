using FINTCS.Models;
using FINTCS.Repository;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public IActionResult Index()
    {
        var data = _accountService.GetAllGroups();
        return View(data);
    }

    [HttpPost]
    public IActionResult SaveGroup(int id, string name, int? underGroupId, string nature)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(nature))
            return Json("All fields required");

        // ✅ Duplicate check
        bool isDuplicate = _accountService.IsDuplicateName(name, id);
        if (isDuplicate)
            return Json("Duplicate");

        if (id > 0)
        {
            var existing = _accountService.GetById(id);
            int? oldUnderId = existing.UnderGroupId;

            existing.GroupName = name;
            existing.UnderGroupId = underGroupId;
            existing.NatureOfGroup = nature;

            _accountService.UpdateGroupWithNewCode(existing, oldUnderId);

            return Json("Updated Successfully");
        }
        else
        {
            GroupMaster obj = new GroupMaster()
            {
                GroupName = name,
                UnderGroupId = underGroupId,
                NatureOfGroup = nature
            };

            _accountService.SaveGroup(obj);
            return Json("Saved Successfully");
        }
    }
}
