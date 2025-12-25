using FINTCS.Repository;
using FINTCS.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FINTCS.Controllers
{
    public class VoucherController : Controller
    {
        private readonly IAccountService _service;

        public VoucherController(IAccountService service)
        {
            _service = service;
        }

        public IActionResult Add()
        {
            return View(new VoucherVM
            {
                VoucherDate = DateTime.Today,
                PassedDate = DateTime.Today
            });
        }

        [HttpPost]
        [Consumes("application/json")]
        [IgnoreAntiforgeryToken]
        public IActionResult Add([FromBody] VoucherVM model)
        {
            if (model == null)
                return Json(new { success = false, message = "Model is null" });

            if (model.Items == null || !model.Items.Any())
                return Json(new { success = false, message = "No voucher items" });

            string msg;
            bool success = _service.SaveVoucher(model, out msg);

            return Json(new
            {
                success = success,
                message = msg
            });
        }


        public IActionResult SearchLedger(int voucherType, int dbCr)
        {
            var data = _service.GetLedgersForVoucher(voucherType, dbCr);
            return PartialView("_LedgerModal", data);
        }
        [HttpGet]
        public IActionResult GetNextVoucherNo(int voucherType)
        {
            int serialNo = _service.GetNextSerial(voucherType);
            return Json(serialNo);
        }
        public IActionResult SearchEditVouchers(
        int voucherType,
        string searchType,
        DateTime? fromDate,
        DateTime? toDate)
        {
            var data = _service.SearchEditVouchers(
                voucherType, searchType, fromDate, toDate);

            return PartialView("_EditVoucherList", data);
        }

        // ✏️ OPEN EDIT
        public IActionResult Edit(int keyNo)
        {
            var model = _service.GetVoucherForEdit(keyNo);
            return View("Add", model);
        }

    }
}

