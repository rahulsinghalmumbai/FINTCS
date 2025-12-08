using FINTCS.Areas.Members.Models;
using FINTCS.Data;
using Microsoft.EntityFrameworkCore;

namespace FINTCS.Repositories
{
    public class MemberService : IMemberService
    {
        private readonly ApplicationDbContext _db;

        public MemberService(ApplicationDbContext db)
        {
            _db = db;
        }

        // ================= SAFE GET BY ID =================
        public async Task<Member?> GetByIdAsync(int id)
        {
            var member = await _db.Members
                .Where(x => x.Id == id)
                .Select(x => new Member
                {
                    Id = x.Id,
                    Memno = x.Memno ?? string.Empty,
                    Name = x.Name ?? string.Empty,
                    FatherName = x.FatherName ?? string.Empty,
                    OfficeAddress = x.OfficeAddress ?? string.Empty,
                    City = x.City ?? string.Empty,
                    Phone = x.Phone ?? string.Empty,
                    Branch = x.Branch ?? string.Empty,
                    Designation = x.Designation ?? string.Empty,
                    Mobile1 = x.Mobile1 ?? string.Empty,
                    Mobile2 = x.Mobile2 ?? string.Empty,
                    ResidenceAddress = x.ResidenceAddress ?? string.Empty,
                    DOB = x.DOB,
                    DOJSociety = x.DOJSociety,
                    Email = x.Email ?? string.Empty,
                    DOJ = x.DOJ,
                    DOR = x.DOR,
                    Nominee = x.Nominee ?? string.Empty,
                    NomineeRelation = x.NomineeRelation ?? string.Empty,
                    Share = x.Share ?? string.Empty,
                    CD = x.CD ?? string.Empty,
                    BankName = x.BankName ?? string.Empty,
                    PayableAt = x.PayableAt ?? string.Empty,
                    AccountNo = x.AccountNo ?? string.Empty,
                    Status = x.Status ?? string.Empty,
                    PhotoPath = x.PhotoPath ?? string.Empty,
                    SignaturePath = x.SignaturePath ?? string.Empty,
                    DeductionShare = x.DeductionShare,
                    WithDrawl = x.WithDrawl,
                    GLoanInstalment = x.GLoanInstalment,
                    ELoanInstalment = x.ELoanInstalment
                })
                .FirstOrDefaultAsync();

            return member;
        }

        // ================= STEPWISE ADD/UPDATE =================
        public async Task<int> AddOrUpdateStepAsync(Member model, int step)
        {
            Member entity;

            if (model.Id == 0)
            {
                entity = new Member();
                _db.Members.Add(entity);
            }
            else
            {
                entity = await _db.Members.FirstOrDefaultAsync(x => x.Id == model.Id)
                         ?? throw new Exception("Member not found in database");
            }

            // ================= STEP 1 : GENERAL =================
            if (step == 1)
            {
                entity.Memno = model.Memno;
                entity.Name = model.Name;
                entity.FatherName = model.FatherName;
                entity.OfficeAddress = model.OfficeAddress;
                entity.City = model.City;
                entity.Phone = model.Phone;
                entity.Branch = model.Branch;
                entity.Designation = model.Designation;
                entity.Mobile1 = model.Mobile1;
                entity.Mobile2 = model.Mobile2;
                entity.ResidenceAddress = model.ResidenceAddress;
                entity.DOB = model.DOB;
                entity.DOJSociety = model.DOJSociety;
                entity.Email = model.Email;
                entity.DOJ = model.DOJ;
                entity.DOR = model.DOR;
                entity.Nominee = model.Nominee;
                entity.NomineeRelation = model.NomineeRelation;
            }

            // ================= STEP 2 : PHOTO & OPENING BALANCE =================
            if (step == 2)
            {
                entity.Share = model.Share;
                entity.CD = model.CD;
                entity.BankName = model.BankName;
                entity.PayableAt = model.PayableAt;
                entity.AccountNo = model.AccountNo;

                entity.PhotoPath = model.PhotoPath;
                entity.SignaturePath = model.SignaturePath;
            }

            // ================= STEP 3 : MONTHLY DEDUCTION =================
            if (step == 3)
            {
                entity.DeductionShare = model.DeductionShare;
                entity.WithDrawl = model.WithDrawl;
                entity.GLoanInstalment = model.GLoanInstalment;
                entity.ELoanInstalment = model.ELoanInstalment;
            }

            await _db.SaveChangesAsync();
            return entity.Id;
        }
    }

}

