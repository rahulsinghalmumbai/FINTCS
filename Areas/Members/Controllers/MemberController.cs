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

        public async Task<IActionResult> Index(int? id, int step = 1)
        {
            Member model = new Member();

            if (id.HasValue && id > 0)
            {
                model = await _memberService.GetByIdAsync(id.Value) ?? new Member();
            }

            ViewBag.Step = step; // ✅ To retain tab after post
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(Member model, int step, IFormFile? PhotoFile, IFormFile? SignatureFile)
        {
            if (step == 2)
            {
                // Photo upload
                if (PhotoFile != null)
                {
                    string photoFolder = @"D:\MemberPhotos";
                    Directory.CreateDirectory(photoFolder);

                    string photoFileName = Guid.NewGuid() + Path.GetExtension(PhotoFile.FileName);
                    string photoPath = Path.Combine(photoFolder, photoFileName);

                    using (var stream = new FileStream(photoPath, FileMode.Create))
                    {
                        await PhotoFile.CopyToAsync(stream);
                    }

                    // ✅ WEB URL store karo — FILE PATH nahi
                    model.PhotoPath = "/MemberPhotos/" + photoFileName;

                }

                // Signature upload
                if (SignatureFile != null)
                {
                    string signFolder = @"D:\MemberSignatures";
                    Directory.CreateDirectory(signFolder);

                    string signFileName = Guid.NewGuid() + Path.GetExtension(SignatureFile.FileName);
                    string signPath = Path.Combine(signFolder, signFileName);

                    using (var stream = new FileStream(signPath, FileMode.Create))
                    {
                        await SignatureFile.CopyToAsync(stream);
                    }

                    // ✅ WEB URL store karo — FILE PATH nahi
                    model.SignaturePath = "/MemberSignatures/" + signFileName;
                }
            }

            int memberId = await _memberService.AddOrUpdateStepAsync(model, step);

            // Redirect to next step
            return RedirectToAction("Index", new { id = memberId, step = step + 1 });
        }
    }
}
