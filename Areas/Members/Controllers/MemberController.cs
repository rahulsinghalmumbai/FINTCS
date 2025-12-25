using FINTCS.Areas.Members.Models;
using FINTCS.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FINTCS.Areas.Members.Controllers
{
    [Area("Members")]
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IWebHostEnvironment _env;

        public MemberController(IMemberService memberService, IWebHostEnvironment env)
        {
            _memberService = memberService;
            _env = env;
        }

        // ✅ GET
        public async Task<IActionResult> Index(int? id, int step = 1)
        {
            Member model;

            if (id.HasValue && id > 0)
            {
                model = await _memberService.GetByIdAsync(id.Value);
                model.LoanAmounts = await _memberService.GetLoanMasterAsync(id.Value);
            }
            else
            {
                model = new Member();
                model.LoanAmounts = await _memberService.GetLoanMasterAsync(0);
            }

            ViewBag.BranchList = await _memberService.GetBranchesAsync();
            ViewBag.DesignationList = await _memberService.GetDesignationsAsync();
            ViewBag.NomineeRelationList = await _memberService.GetNomineeRelationsAsync();
            ViewBag.Step = step;

            return View(model);
        }



        // ✅ POST

        [HttpPost]
        public async Task<IActionResult> Index(
    Member model,
    int step,
    IFormFile? PhotoFile,
    IFormFile? SignatureFile)
        {
            int memberId = model.Id;

            /* ================= STEP 1 ================= */
            if (step == 1)
            {
                memberId = await _memberService.AddOrUpdateStepAsync(model, step);

                if (model.Id == 0)
                {
                    await _memberService.GenerateMemberSpecificLedgersAsync(
                        memberId, model.Memno, model.Name);
                }
                else
                {
                    await _memberService.UpdateMemberLedgerNamesAsync(
                        memberId, model.Memno, model.Name);
                }
            }

            /* ================= STEP 2 ================= */
            if (step == 2)
            {
                model.Id = memberId;

                // ✅ PHOTO UPLOAD
                if (PhotoFile != null && PhotoFile.Length > 0)
                {
                    string photoFolder = Path.Combine(_env.WebRootPath, "MemberPhotos");
                    Directory.CreateDirectory(photoFolder);

                    string photoName = Guid.NewGuid() + Path.GetExtension(PhotoFile.FileName);
                    string photoPath = Path.Combine(photoFolder, photoName);

                    using (var stream = new FileStream(photoPath, FileMode.Create))
                    {
                        await PhotoFile.CopyToAsync(stream);
                    }

                    model.PhotoPath = "/MemberPhotos/" + photoName;
                }

                // ✅ SIGNATURE UPLOAD
                if (SignatureFile != null && SignatureFile.Length > 0)
                {
                    string signFolder = Path.Combine(_env.WebRootPath, "MemberSignatures");
                    Directory.CreateDirectory(signFolder);

                    string signName = Guid.NewGuid() + Path.GetExtension(SignatureFile.FileName);
                    string signPath = Path.Combine(signFolder, signName);

                    using (var stream = new FileStream(signPath, FileMode.Create))
                    {
                        await SignatureFile.CopyToAsync(stream);
                    }

                    model.SignaturePath = "/MemberSignatures/" + signName;
                }

                // ❌ REQUIRED FILE CHECK
                if (string.IsNullOrEmpty(model.PhotoPath) ||
                    string.IsNullOrEmpty(model.SignaturePath))
                {
                    TempData["Error"] = "Photo & Signature Required";

                    ViewBag.BranchList = await _memberService.GetBranchesAsync();
                    ViewBag.DesignationList = await _memberService.GetDesignationsAsync();
                    ViewBag.NomineeRelationList = await _memberService.GetNomineeRelationsAsync();

                    ViewBag.Step = 2;
                    return View(model);
                }

                // ✅ NOW SAVE STEP-2 DATA WITH FILE PATH
                await _memberService.AddOrUpdateStepAsync(model, step);
            }

            /* ================= STEP 3 ================= */
            if (step == 3)
            {
                model.Id = memberId;

                await _memberService.AddOrUpdateStepAsync(model, step);

                await _memberService.SaveLoanAmountsAsync(
                    model.Id,
                    model.LoanAmounts
                );

                // 🔥 Reload loans after save
                model.LoanAmounts = await _memberService.GetLoanMasterAsync(model.Id);

                ViewBag.BranchList = await _memberService.GetBranchesAsync();
                ViewBag.DesignationList = await _memberService.GetDesignationsAsync();
                ViewBag.NomineeRelationList = await _memberService.GetNomineeRelationsAsync();
                ViewBag.Step = 3;

                TempData["Success"] = "Member saved successfully";
                return View(model);
            }



            return RedirectToAction("Index", new { id = memberId, step = step + 1 });
        }

        [HttpGet]
        public JsonResult IsMemNoExists(string memno, int id)
        {
            var exists = _memberService.IsMemNoExists(memno, id);
            return Json(exists);
        }
        [HttpGet]
        public async Task<IActionResult> GetMembers(string search = "")
        {
            var data = await _memberService.GetMembersAsync(search);
            return Json(data);
        }

        
    }
}
