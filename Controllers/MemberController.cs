using Microsoft.AspNetCore.Mvc;

namespace FINTCS.Controllers
{
    public class MemberController : Controller
    {
        // Index Page
        public IActionResult Index()
        {
            return View();
        }

        // POST for Save Button
        [HttpPost]
        public IActionResult SaveData(string grid1Data, string grid2Data, string grid3Data)
        {
            // Yahan data aayega → Save in DB
            // TODO: Convert JSON to model and save

            TempData["msg"] = "Data saved successfully!";
            return RedirectToAction("Index");
        }
    }
}
