using FINTCS.Areas.Members.Models;
using FINTCS.Areas.Members.ViewModel;
using FINTCS.Data;
using FINTCS.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
        public async Task GenerateMemberSpecificLedgersAsync(
    int memberId,
    string memNo,
    string memberName)
        {
            // 🔹 1. LEDGERAC se MemSpecific ledgers (template)
            var memberSpecificLedgers = await _db.LEDGERAC
                .Where(l => l.MemSpecific == true)
                .Select(l => new
                {
                    l.GroupId,
                    LedgerSuffix = l.ledger.Substring(l.ledger.IndexOf(")") + 1) // -test1
                })
                .Distinct()
                .ToListAsync();

            if (!memberSpecificLedgers.Any())
                return;

            // 🔹 2. GroupMaster data
            var groupIds = memberSpecificLedgers.Select(x => x.GroupId).Distinct().ToList();

            var groups = await _db.GroupMaster
                .Where(g => groupIds.Contains(g.Id))
                .ToListAsync();

            foreach (var group in groups)
            {
                // 🔹 3. LedgerCounter (NO +1 BUG)
                int ledgerCounter = group.LedgerCount ?? 1;

                // 🔹 4. Correct suffix (test1 / test2)
                var suffix = memberSpecificLedgers
                    .First(x => x.GroupId == group.Id)
                    .LedgerSuffix;

                string ledgerCode = $"{group.DCode}B{ledgerCounter}";
                string ledgerName = $"{memNo}({memberName}){suffix}";

                var prms = new[]
                {
            new SqlParameter("@LEDGERCODE", ledgerCode),
            new SqlParameter("@ledger", ledgerName),

            new SqlParameter
            {
                ParameterName = "@OpeningBalance",
                SqlDbType = SqlDbType.Decimal,
                Precision = 18,
                Scale = 2,
                Value = 0m
            },

            new SqlParameter("@drcr", 1),
            new SqlParameter("@CreatedOn", DateTime.Now),
            new SqlParameter("@GroupId", group.Id),
            new SqlParameter("@MemSpecific", 1),
            new SqlParameter("@MemberId", memberId)
        };

                // 🔹 5. Insert Ledger
                await _db.Database.ExecuteSqlRawAsync(
                    @"EXEC SP_InsertMemberLedger 
              @LEDGERCODE,@ledger,@OpeningBalance,@drcr,@CreatedOn,@GroupId,@MemSpecific,@MemberId",
                    prms
                );

                // 🔹 6. Update counter AFTER insert
                await _db.Database.ExecuteSqlRawAsync(
                    @"UPDATE GroupMaster 
              SET LedgerCount = @NextCounter 
              WHERE Id = @GroupId",
                    new SqlParameter("@NextCounter", ledgerCounter + 1),
                    new SqlParameter("@GroupId", group.Id)
                );
            }
        }


        public async Task UpdateMemberLedgerNamesAsync(
    int memberId,
    string memNo,
    string memberName)
        {
            await _db.Database.ExecuteSqlRawAsync(
                "EXEC SP_UpdateLedgerNameByMember @MemberId,@MemNo,@MemberName",
                new SqlParameter("@MemberId", memberId),
                new SqlParameter("@MemNo", memNo),
                new SqlParameter("@MemberName", memberName)
            );
        }
       

            public bool IsMemNoExists(string memno, int id)
            {
                var memnoParam = new SqlParameter("@MemNo", memno);
                var idParam = new SqlParameter("@Id", id);

                var result = _db.Database
                    .SqlQueryRaw<int>("EXEC SP_IsMemNoExists @MemNo, @Id", memnoParam, idParam)
                    .AsEnumerable()
                    .FirstOrDefault();

                return result == 1;
            }
        public async Task<List<LoanAmountVM>> GetLoanMasterAsync(int memberId)
        {
            var loans = await _db.LoanMasters
                .Select(l => new LoanAmountVM
                {
                    LoanId = l.LoanMasterId,
                    LoanName = l.LoanName,
                    Amt = memberId > 0
                        ? _db.LoanMasterMember
                            .Where(m => m.MemberId == memberId && m.LoanId == l.LoanMasterId)
                            .Select(x => x.Amt)
                            .FirstOrDefault()
                        : null
                })
                .ToListAsync();

            return loans;
        }


        public async Task SaveLoanAmountsAsync(int memberId, List<LoanAmountVM> loans)
        {
            // DataTable banega (Table Valued Parameter ke liye)
            DataTable dt = new DataTable();
            dt.Columns.Add("LoanId", typeof(int));
            dt.Columns.Add("Amt", typeof(decimal));

            foreach (var item in loans)
            {
                if (item.Amt.HasValue && item.Amt > 0)
                {
                    dt.Rows.Add(item.LoanId, item.Amt.Value);
                }
            }

            var memberParam = new SqlParameter("@MemberId", memberId);

            var tvpParam = new SqlParameter("@LoanAmounts", dt)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.LoanAmountType"
            };

            await _db.Database.ExecuteSqlRawAsync(
                "EXEC SP_SaveLoanAmounts @MemberId, @LoanAmounts",
                memberParam,
                tvpParam
            );
        }

        public async Task<List<MemberSearchVM>> GetMembersAsync(string search)
        {
            return await _db.Members
                .Where(x => search == "" ||
                            x.Name.Contains(search) ||
                            x.Memno.Contains(search))
                .Select(x => new MemberSearchVM
                {
                    Id = x.Id,
                    Memno = x.Memno,
                    Name = x.Name
                })
                .ToListAsync();
        }

        
      

    }


}

