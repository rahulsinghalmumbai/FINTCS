using FINTCS.Areas.Members.Models;
using FINTCS.Data;
using FINTCS.Models;
using Microsoft.Data.SqlClient;
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

        // ================= GET BY ID =================
        public async Task<Member?> GetByIdAsync(int id)
        {
            var member = _db.Members
    .FromSqlRaw("EXEC SP_GetMemberById @Id", new SqlParameter("@Id", id))
    .AsEnumerable()        // <-- result ko IEnumerable pe le jao
    .FirstOrDefault();

            return member;
        }

        // ================= STEPWISE SAVE =================
        public async Task<int> AddOrUpdateStepAsync(Member model, int step)
        {
            int memberId = model.Id;

            // ✅ FIRST INSERT IF NEW
            if (memberId == 0)
            {
                memberId = _db.Database
                    .SqlQuery<int>($"EXEC SP_InsertMember")
                    .AsEnumerable()
                    .First();
            }

            // ================= STEP PARAMETERS =================
            var prms = new List<SqlParameter>
            {
                new("@Id", memberId)
            };

            if (step == 1)
            {
                prms.AddRange(new[]
                {
                    new SqlParameter("@Memno", (object?)model.Memno ?? DBNull.Value),
                    new SqlParameter("@Name", (object?)model.Name ?? DBNull.Value),
                    new SqlParameter("@FatherName", (object?)model.FatherName ?? DBNull.Value),
                    new SqlParameter("@OfficeAddress", (object?)model.OfficeAddress ?? DBNull.Value),
                    new SqlParameter("@City", (object?)model.City ?? DBNull.Value),
                    new SqlParameter("@Phone", (object?)model.Phone ?? DBNull.Value),
                    new SqlParameter("@Branch", (object?)model.Branch ?? DBNull.Value),
                    new SqlParameter("@Designation", (object?)model.Designation ?? DBNull.Value),
                    new SqlParameter("@Mobile1", (object?)model.Mobile1 ?? DBNull.Value),
                    new SqlParameter("@Mobile2", (object?)model.Mobile2 ?? DBNull.Value),
                    new SqlParameter("@ResidenceAddress", (object?)model.ResidenceAddress ?? DBNull.Value),
                    new SqlParameter("@DOB", (object?)model.DOB ?? DBNull.Value),
                    new SqlParameter("@DOJSociety", (object?)model.DOJSociety ?? DBNull.Value),
                    new SqlParameter("@Email", (object?)model.Email ?? DBNull.Value),
                    new SqlParameter("@DOJ", (object?)model.DOJ ?? DBNull.Value),
                    new SqlParameter("@DOR", (object?)model.DOR ?? DBNull.Value),
                    new SqlParameter("@Nominee", (object?)model.Nominee ?? DBNull.Value),
                    new SqlParameter("@NomineeRelation", (object?)model.NomineeRelation ?? DBNull.Value)
                });

                // ✅ EXECUTE STEP 1 SP
                await _db.Database.ExecuteSqlRawAsync(
                    @"EXEC SP_UpdateMemberStep1 
                        @Id=@Id, @Memno=@Memno, @Name=@Name, @FatherName=@FatherName, 
                        @OfficeAddress=@OfficeAddress, @City=@City, @Phone=@Phone, @Branch=@Branch, 
                        @Designation=@Designation, @Mobile1=@Mobile1, @Mobile2=@Mobile2, @ResidenceAddress=@ResidenceAddress, 
                        @DOB=@DOB, @DOJSociety=@DOJSociety, @Email=@Email, @DOJ=@DOJ, @DOR=@DOR, 
                        @Nominee=@Nominee, @NomineeRelation=@NomineeRelation", prms.ToArray());
            }

            if (step == 2)
            {
                prms.AddRange(new[]
                {
                    new SqlParameter("@Share", (object?)model.Share ?? DBNull.Value),
                    new SqlParameter("@ShareType", (object?)model.ShareType ?? DBNull.Value),
                    new SqlParameter("@CD", (object?)model.CD ?? DBNull.Value),
                    new SqlParameter("@CDType", (object?)model.CDType ?? DBNull.Value),
                    new SqlParameter("@BankName", (object?)model.BankName ?? DBNull.Value),
                    new SqlParameter("@PayableAt", (object?)model.PayableAt ?? DBNull.Value),
                    new SqlParameter("@AccountNo", (object?)model.AccountNo ?? DBNull.Value),
                    new SqlParameter("@Status", (object?)model.Status ?? DBNull.Value),
                    new SqlParameter("@Date", (object?)model.Date ?? DBNull.Value),
                    new SqlParameter("@PhotoPath", (object?)model.PhotoPath ?? DBNull.Value),
                    new SqlParameter("@SignaturePath", (object?)model.SignaturePath ?? DBNull.Value)
                });

                await _db.Database.ExecuteSqlRawAsync(
                    @"EXEC SP_UpdateMemberStep2 
                        @Id=@Id, @Share=@Share, @ShareType=@ShareType, @CD=@CD, @CDType=@CDType,
                        @BankName=@BankName, @PayableAt=@PayableAt, @AccountNo=@AccountNo,
                        @Status=@Status, @Date=@Date, @PhotoPath=@PhotoPath, @SignaturePath=@SignaturePath",
                    prms.ToArray());
            }

            if (step == 3)
            {
                prms.AddRange(new[]
                {
                    new SqlParameter("@DeductionShare", (object?)model.DeductionShare ?? DBNull.Value),
                    new SqlParameter("@WithDrawl", (object?)model.WithDrawl ?? DBNull.Value),
                    new SqlParameter("@GLoanInstalment", (object?)model.GLoanInstalment ?? DBNull.Value),
                    new SqlParameter("@ELoanInstalment", (object?)model.ELoanInstalment ?? DBNull.Value)
                });

                await _db.Database.ExecuteSqlRawAsync(
                    @"EXEC SP_UpdateMemberStep3 
                        @Id=@Id, @DeductionShare=@DeductionShare, @WithDrawl=@WithDrawl, 
                        @GLoanInstalment=@GLoanInstalment, @ELoanInstalment=@ELoanInstalment",
                    prms.ToArray());
            }

            return memberId;
        }

        // ================= DROPDOWNS =================
        public async Task<List<TBLDAT>> GetBranchesAsync()
        {
            return await _db.TblDat
                .FromSqlRaw("EXEC SP_GetBranches")
                .ToListAsync();
        }

        public async Task<List<TBLDAT>> GetDesignationsAsync()
        {
            return await _db.TblDat
                .FromSqlRaw("EXEC SP_GetDesignations")
                .ToListAsync();
        }

        public async Task<List<TBLDAT>> GetNomineeRelationsAsync()
        {
            return await _db.TblDat
                .FromSqlRaw("EXEC SP_GetNomineeRelations")
                .ToListAsync();
        }

    }
}
