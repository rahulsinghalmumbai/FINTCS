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
            Member model = new Member();

            if (id.HasValue && id > 0)
            {
                model = await _memberService.GetByIdAsync(id.Value) ?? new Member();
            }

            // ✅ DROPDOWNS
            ViewBag.BranchList = await _memberService.GetBranchesAsync();
            ViewBag.DesignationList = await _memberService.GetDesignationsAsync();
            ViewBag.NomineeRelationList = await _memberService.GetNomineeRelationsAsync();

            ViewBag.Step = step;
            return View(model);
        }

        // ✅ POST
        [HttpPost]
        public async Task<IActionResult> Index(Member model, int step, IFormFile? PhotoFile, IFormFile? SignatureFile)
        {
            // ✅ STEP 1 VALIDATION
            if (step == 1)
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.BranchList = await _memberService.GetBranchesAsync();
                    ViewBag.DesignationList = await _memberService.GetDesignationsAsync();
                    ViewBag.NomineeRelationList = await _memberService.GetNomineeRelationsAsync();

                    ViewBag.Step = 1;
                    return View(model);
                }
            }

            // ✅ STEP 2 FILE UPLOAD + NAME SAFE FIX
            if (step == 2)
            {
                // ✅ PHOTO
                if (PhotoFile != null)
                {
                    string folder = Path.Combine(_env.WebRootPath, "MemberPhotos");
                    Directory.CreateDirectory(folder);

                    string fileName = Guid.NewGuid() + Path.GetExtension(PhotoFile.FileName);
                    string path = Path.Combine(folder, fileName);

                    using var stream = new FileStream(path, FileMode.Create);
                    await PhotoFile.CopyToAsync(stream);

                    model.PhotoPath = "/MemberPhotos/" + fileName;
                   /* model.PhotoName = PhotoFile.FileName; */// ✅ NAME SAFE
                }

                // ✅ SIGNATURE
                if (SignatureFile != null)
                {
                    string folder = Path.Combine(_env.WebRootPath, "MemberSignatures");
                    Directory.CreateDirectory(folder);

                    string fileName = Guid.NewGuid() + Path.GetExtension(SignatureFile.FileName);
                    string path = Path.Combine(folder, fileName);

                    using var stream = new FileStream(path, FileMode.Create);
                    await SignatureFile.CopyToAsync(stream);

                    model.SignaturePath = "/MemberSignatures/" + fileName;
                   /* model.SignatureName = SignatureFile.FileName; */// ✅ NAME SAFE
                }

                // ✅ FILE REQUIRED VALIDATION
                if (string.IsNullOrEmpty(model.PhotoPath) || string.IsNullOrEmpty(model.SignaturePath))
                {
                    TempData["Error"] = "Photo & Signature Required";

                    ViewBag.BranchList = await _memberService.GetBranchesAsync();
                    ViewBag.DesignationList = await _memberService.GetDesignationsAsync();
                    ViewBag.NomineeRelationList = await _memberService.GetNomineeRelationsAsync();

                    ViewBag.Step = 2;
                    return View(model); // ✅ POPUP OPEN HOGA, TAB CHANGE NAHI
                }
            }

            int memberId = await _memberService.AddOrUpdateStepAsync(model, step);

            // ✅ STEP 3 PAR HI RUKNA HAI — STEP 4 PAR NAHI JAANA
            if (step == 3)
            {
                ViewBag.BranchList = await _memberService.GetBranchesAsync();
                ViewBag.DesignationList = await _memberService.GetDesignationsAsync();
                ViewBag.NomineeRelationList = await _memberService.GetNomineeRelationsAsync();

                ViewBag.Step = 3;
                TempData["Success"] = "Step 3 Saved Successfully";
                return View(model); // ✅ YAHI RUK GAYA
            }

            // ✅ NORMAL FLOW (Step 1 → 2 → 3)
            return RedirectToAction("Index", new { id = memberId, step = step + 1 });
        }
    }
}
