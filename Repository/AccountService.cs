using FINTCS.Data;
using FINTCS.DTOs;
using FINTCS.Models;
using FINTCS.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FINTCS.Repository
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }


        // ------------------------------------------------------------

        public List<GroupMaster> GetAllGroups()
        {
            return _context.GroupMaster
                .FromSqlRaw("EXEC SP_GetAllGroups")
                .ToList();
        }

        public GroupMaster? GetById(int id)
        {
            return _context.GroupMaster
                .FromSqlRaw(
                    "EXEC SP_GetGroupById @Id",
                    new SqlParameter("@Id", id)
                )
                .AsEnumerable()
                .FirstOrDefault();
        }

        public bool IsDuplicateName(string name, int id)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();

            command.CommandText = "SP_IsDuplicateGroup";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Name", name));
            command.Parameters.Add(new SqlParameter("@Id", id));

            _context.Database.OpenConnection();

            var result = command.ExecuteScalar();

            _context.Database.CloseConnection();

            return Convert.ToInt32(result) > 0;
        }
        public GroupMaster? GetGroupById(int id)
        {
            return _context.GroupMaster
                .FromSqlRaw(
                    "EXEC SP_GetGroupById @Id",
                    new SqlParameter("@Id", id)
                )
                .AsEnumerable()
                .FirstOrDefault();
        }


        // ------------------------------------------------------------
        // ADD GROUP (D-CODE / COUNTER logic in service)
        // ------------------------------------------------------------

        public void SaveGroup(GroupMaster model)
        {
            // Code generation in service (same logic)
            GenerateDCodeAndCounter(model, isEdit: false, oldUnderId: null);

            var parameters = new[]
            {
                new SqlParameter("@GroupName", model.GroupName),
                new SqlParameter("@UnderGroupId", (object?)model.UnderGroupId ?? DBNull.Value),
                new SqlParameter("@NatureOfGroup", model.NatureOfGroup),
                new SqlParameter("@DCode", model.DCode),
                new SqlParameter("@LedgerCounter", model.LedgerCounter),

            };

            _context.Database.ExecuteSqlRaw("EXEC SP_InsertGroup @GroupName,@UnderGroupId,@NatureOfGroup,@DCode,@LedgerCounter", parameters);
        }








        // ------------------------------------------------------------
        // LEDGER CRUD
        // ------------------------------------------------------------

        public List<GroupMaster> GetGroups()
        {
            return _context.GroupMaster
                .FromSqlRaw("EXEC SP_GetGroups_ExcludeMemSpecific")
                .AsNoTracking()
                .ToList();
        }
        public List<LedgerListDto> GetLedgersWithGroup()
        {
            return _context.Set<LedgerListDto>()
                .FromSqlRaw("EXEC SP_GetLedgersWithGroup")
                .AsNoTracking()
                .ToList();
        }
        public List<LEDGERAC> GetLedgers()
        {
            return _context.LEDGERAC
                .FromSqlRaw("EXEC SP_GetAllLedgers")
                .ToList();
        }

        public LEDGERAC? GetLedgerById(int id)
        {
            return _context.LEDGERAC
                .FromSqlRaw(
                    "EXEC SP_GetLedgerById @Id",
                    new SqlParameter("@Id", id)
                )
                .AsEnumerable()
                .FirstOrDefault();
        }



        public void SaveLedger(LEDGERAC ledger)
        {
            ledger.CreatedOn = DateTime.Now;

            if (ledger.drcr == 0)
                ledger.drcr = 1;

            var prms = new[]
            {
        new SqlParameter("@LEDGERCODE", ledger.LEDGERCODE ?? ""),
        new SqlParameter("@ledger", ledger.ledger ?? ""),
        new SqlParameter("@OpeningBalance", ledger.OpeningBalance),
        new SqlParameter("@drcr", ledger.drcr),
        new SqlParameter("@CreatedOn", ledger.CreatedOn),
        new SqlParameter("@GroupId", ledger.GroupId),


       new SqlParameter("@MemSpecific",
    ledger.MemSpecific ?? false   // null → false → 0
),

new SqlParameter("@Bank",
    ledger.Bank.HasValue
        ? (object)ledger.Bank.Value
        : DBNull.Value
),
    };

            _context.Database.ExecuteSqlRaw(
                "EXEC SP_InsertLedger @LEDGERCODE,@ledger,@OpeningBalance,@drcr,@CreatedOn,@GroupId,@MemSpecific,@Bank",
                prms
            );
        }


        public void UpdateLedgerWithCodeAdjust(LEDGERAC model)
        {
            var existing = GetLedgerById(model.Id);

            // 🔹 Group changed
            if (existing.GroupId != model.GroupId)
            {
                // 🔹 Generate code from NEW group only
                string newCode = GenerateLedgerCode(model.GroupId);
                existing.LEDGERCODE = newCode;

                // 🔹 Update parent group
                existing.GroupId = model.GroupId;

                // ❌ Old group LedgerCount untouched
                // ❌ Old group ledgers NOT re-numbered
            }

            // 🔹 Normal updates
            existing.ledger = model.ledger;
            existing.OpeningBalance = model.OpeningBalance;
            existing.drcr = model.drcr;
            existing.Bank = model.Bank;
            existing.MemSpecific = model.MemSpecific;

            UpdateLedger(existing);
        }
        public void UpdateLedger(LEDGERAC ledger)
        {
            ledger.CreatedOn = DateTime.Now;

            var prms = new[]
 {
    new SqlParameter("@Id", ledger.Id),
    new SqlParameter("@LEDGERCODE", ledger.LEDGERCODE ?? ""),
    new SqlParameter("@ledger", ledger.ledger ?? ""),
    new SqlParameter("@OpeningBalance", ledger.OpeningBalance),
    new SqlParameter("@drcr", ledger.drcr),
    new SqlParameter("@CreatedOn", ledger.CreatedOn),
    new SqlParameter("@GroupId", ledger.GroupId),

    // 🔥 NEW – null → 0
    new SqlParameter("@MemSpecific", ledger.MemSpecific ?? false),
    new SqlParameter("@Bank", ledger.Bank ?? false)
};

            _context.Database.ExecuteSqlRaw(
                @"EXEC SP_UpdateLedger 
        @Id,
        @LEDGERCODE,
        @ledger,
        @OpeningBalance,
        @drcr,
        @CreatedOn,
        @GroupId,
        @MemSpecific,
        @Bank",
                prms
            );

        }

        // ------------------------------------------------------------
        // LEDGER CODE GENERATOR (Service Logic)
        // ------------------------------------------------------------

        public string GenerateLedgerCode(int groupId)
        {
            var group = GetGroupById(groupId);

            // 🔹 Null check: agar LedgerCount null hai to 1 set karo
            if (!group.LedgerCount.HasValue || group.LedgerCount < 1)
            {
                group.LedgerCount = 1; // null ya 0 ho to 2 se start
            }

            int counterToUse = group.LedgerCount.Value;

            string newCode = group.DCode + "B" + counterToUse;

            // 🔹 Increment for next ledger
            group.LedgerCount = counterToUse + 1;

            _context.SaveChanges();

            return newCode;
        }



        // ------------------------------------------------------------
        // LEDGER UPDATE WITH CODE ADJUST
        // ------------------------------------------------------------


        public void UpdateGroupWithNewCode(GroupMaster model, int? oldUnderId)
        {
            var existing = GetById(model.Id)!;  // 👈 assume GetById never returns null

            bool isParentChanged = oldUnderId != model.UnderGroupId;

            if (isParentChanged)
            {
                GroupMaster? newParent = null;

                if (model.UnderGroupId != null)
                {
                    newParent = GetById(model.UnderGroupId.Value);
                }

                int counterToUse = (newParent != null)
                                   ? newParent.LedgerCounter
                                   : 1;

                string parentCode = newParent != null ? newParent.DCode! : "";
                string newDCode = parentCode + "A" + counterToUse;

                existing.DCode = newDCode;

                if (newParent != null)
                {
                    newParent.LedgerCounter += 1;

                    // Update parent
                    _context.GroupMaster.Update(newParent);
                    _context.SaveChanges();
                }
            }

            var parameters = new[]
            {
        new SqlParameter("@Id", model.Id),
        new SqlParameter("@GroupName", model.GroupName),
        new SqlParameter("@UnderGroupId", (object?)model.UnderGroupId ?? DBNull.Value),
        new SqlParameter("@NatureOfGroup", model.NatureOfGroup),
        new SqlParameter("@DCode", existing.DCode!),
        new SqlParameter("@LedgerCounter", existing.LedgerCounter),
    };

            _context.Database.ExecuteSqlRaw(
                "EXEC SP_UpdateGroup @Id,@GroupName,@UnderGroupId,@NatureOfGroup,@DCode,@LedgerCounter",
                parameters
            );
        }

        private void GenerateDCodeAndCounter(GroupMaster model, bool isEdit, int? oldUnderId)
        {
            if (isEdit && oldUnderId != null)
            {
                var oldParent = GetById(oldUnderId.Value);
                if (oldParent != null && oldParent.LedgerCounter > 0)
                {
                    oldParent.LedgerCounter -= 1;
                    _context.GroupMaster.Update(oldParent);
                    _context.SaveChanges();
                }
            }

            if (model.UnderGroupId == null)
            {
                int primaryCount = _context.GroupMaster
                    .Where(x => x.UnderGroupId == null && x.Id != model.Id)
                    .Count() + 1;

                model.DCode = "A" + primaryCount;
                model.LedgerCounter = 1;
            }
            else
            {
                var parent = GetById(model.UnderGroupId.Value);

                int childCount = _context.GroupMaster
                    .Where(x => x.UnderGroupId == model.UnderGroupId && x.Id != model.Id)
                    .Count() + 1;

                model.DCode = parent.DCode + "A" + childCount;
                model.LedgerCounter = 1;

                parent.LedgerCounter += 1;
                _context.GroupMaster.Update(parent);
                _context.SaveChanges();
            }
        }

        public void SaveMemberSpecificLedger(LEDGERAC model)
        {
            // 🔹 Get group (read-only)
            var group = _context.GroupMaster
                                .AsNoTracking()
                                .First(x => x.Id == model.GroupId);

            string groupCode = group.DCode;          // A1
            int ledgerCounter = group.LedgerCount ?? 1;

            var members = _context.Members.ToList();

            using var transaction = _context.Database.BeginTransaction(); // optional, ensures atomic insert

            foreach (var m in members)
            {
                string ledgerCode = $"{groupCode}B{ledgerCounter}";
                string ledgerName = $"{m.Memno}({m.Name})-{model.ledger}";

                var ledgerParams = new[]
                {
            new SqlParameter("@LEDGERCODE", ledgerCode ?? ""),
            new SqlParameter("@ledger", ledgerName ?? ""),
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
            new SqlParameter("@GroupId", model.GroupId),
            new SqlParameter("@MemSpecific", 1),  // always 1 for member-specific
            new SqlParameter("@Bank", false)          // always 0 for member-specific
        };

                // 🔹 Insert Ledger safely
                _context.Database.ExecuteSqlRaw(
                    "EXEC SP_InsertLedger @LEDGERCODE,@ledger,@OpeningBalance,@drcr,@CreatedOn,@GroupId,@MemSpecific,@Bank",
                    ledgerParams
                );

                ledgerCounter++; // 🔥 increment for next member
            }

            // 🔹 Update Group LedgerCounter via SP
            var groupParams = new[]
            {
        new SqlParameter("@GroupId", model.GroupId),
        new SqlParameter("@LedgerCounter", ledgerCounter)
    };

            _context.Database.ExecuteSqlRaw(
                "EXEC SP_UpdateGroupLedgerCounter @GroupId,@LedgerCounter",
                groupParams
            );

            transaction.Commit(); // commit transaction
        }



        //voucher
        // 🔢 SERIAL NUMBER LOGIC
        public int GetNextSerial(int voucherType)
        {
            int? last = _context.Mast_Voc
                .Where(x => x.VoucherType == voucherType)
                .Max(x => (int?)x.SerialNo); // make it nullable

            return (last ?? 0) + 1;
        }

        // 📅 PAST DATE VALIDATION
        public bool IsPastDateAllowed(DateTime voucherDate)
        {
            DateTime? maxDate = _context.Mast_Voc
                .Max(x => (DateTime?)x.VoucherDate);

            // agar table empty hai to allow
            if (!maxDate.HasValue)
                return true;

            return voucherDate >= maxDate.Value;
        }


        // 🔍 LEDGER FILTERING (MOST IMPORTANT PART)
        public List<LEDGERAC> GetLedgersForVoucher(int voucherType, int dbCr)
        {
            IQueryable<LEDGERAC> q = _context.LEDGERAC;

            switch ((VoucherType)voucherType)
            {
                case VoucherType.Receipt:
                    q = dbCr == 1
                        ? q.Where(x => x.Bank == true)
                        : q.Where(x => x.Bank != true);
                    break;

                case VoucherType.Payment:
                    q = dbCr == 2
                        ? q.Where(x => x.Bank == true)
                        : q.Where(x => x.Bank != true);
                    break;

                case VoucherType.Journal:
                    q = q.Where(x => x.Bank != true);
                    break;

                case VoucherType.Contra:
                    q = q.Where(x => x.Bank == true);
                    break;
            }

            return q.ToList();
        }

        // 💾 SAVE VOUCHER
        public bool SaveVoucher(VoucherVM model, out string message)
        {
            message = "";

            // 🔁 Duplicate check
            bool alreadyExists = _context.Mast_Voc.Any(x =>
                x.VoucherType == model.VoucherType &&
                x.SerialNo == model.SerialNo &&
                x.KeyNo != model.KeyNo
            );

            if (alreadyExists)
            {
                message = "Voucher already saved";
                return false;
            }

            // 🔢 Debit / Credit validation
            decimal drTotal = model.Items.Where(x => x.DbCr == 1).Sum(x => x.Amount);
            decimal crTotal = model.Items.Where(x => x.DbCr == 2).Sum(x => x.Amount);

            if (drTotal != crTotal)
            {
                message = "Debit & Credit amount must be equal";
                return false;
            }

            if (!IsPastDateAllowed(model.VoucherDate))
            {
                message = "Past voucher date not allowed";
                return false;
            }

            // ================= FINAL SAVE (SP) =================
            var keyNoParam = new SqlParameter("@KeyNo", model.KeyNo)
            {
                Direction = ParameterDirection.InputOutput
            };

            _context.Database.ExecuteSqlRaw(
                "EXEC dbo.SP_FinalSaveVoucher @KeyNo OUTPUT,@VoucherType,@SerialNo,@VoucherDate,@ChequeNo,@ChequeDate,@Narration,@Remarks,@PassedDate",
                keyNoParam,
                new SqlParameter("@VoucherType", model.VoucherType),
                new SqlParameter("@SerialNo", model.SerialNo),
                new SqlParameter("@VoucherDate", model.VoucherDate),
                new SqlParameter("@ChequeNo", (object?)model.ChequeNo ?? DBNull.Value),
                new SqlParameter("@ChequeDate", (object?)model.ChequeDate ?? DBNull.Value),
                new SqlParameter("@Narration", (object?)model.Narration ?? DBNull.Value),
                new SqlParameter("@Remarks", (object?)model.Remarks ?? DBNull.Value),
                new SqlParameter("@PassedDate", model.PassedDate)
            );

            int keyNo = Convert.ToInt32(keyNoParam.Value);


            // ================= ITEMS SAVE (EF SAME) =================
            if (model.KeyNo != 0)
            {
                var oldItems = _context.Voucher.Where(x => x.KeyNo == keyNo);
                _context.Voucher.RemoveRange(oldItems);
                _context.SaveChanges();
            }

            foreach (var item in model.Items)
            {
                _context.Voucher.Add(new Voucher
                {
                    KeyNo = keyNo,
                    DbCr = item.DbCr,
                    ParticularCode = item.ParticularCode,
                    Amount = item.Amount
                });
            }

            _context.SaveChanges();

            message = model.KeyNo == 0
                ? "Voucher saved successfully"
                : "Voucher updated successfully";

            return true;
        }


        public List<EditVoucherRowVM> SearchEditVouchers(
        int voucherType,
        string searchType,
        DateTime? fromDate,
        DateTime? toDate)
        {
            var q = from m in _context.Mast_Voc
                    join v in _context.Voucher on m.KeyNo equals v.KeyNo
                    where m.VoucherType == voucherType
                    select new { m, v };

            // DATE FILTER
            if (searchType == "month")
            {
                var now = DateTime.Now;
                q = q.Where(x =>
                    x.m.VoucherDate.Month == now.Month &&
                    x.m.VoucherDate.Year == now.Year);
            }
            else if (fromDate.HasValue && toDate.HasValue)
            {
                q = q.Where(x =>
                    x.m.VoucherDate >= fromDate.Value &&
                    x.m.VoucherDate <= toDate.Value);
            }

            // 🔥 TALLY DR / CR RULE
            int showDbCr = voucherType switch
            {
                5 => 2, // Receipt → CR
                6 => 1, // Payment → DR
                7 => 2, // Journal → CR
                8 => 1, // Contra → DR
                _ => 1
            };

            return q
                .Where(x => x.v.DbCr == showDbCr)
                .Select(x => new EditVoucherRowVM
                {
                    KeyNo = x.m.KeyNo,
                    SerialNo = x.m.SerialNo,
                    Date = x.m.VoucherDate,
                    Ledger = x.v.ParticularCode,
                    Amount = x.v.Amount
                })
                .OrderByDescending(x => x.Date)
                .ToList();
        }

        // ================= LOAD FOR EDIT =================
        public VoucherVM GetVoucherForEdit(int keyNo)
        {
            var mast = _context.Mast_Voc.First(x => x.KeyNo == keyNo);

            var items = (
     from v in _context.Voucher
     join l in _context.LEDGERAC
         on v.ParticularCode equals l.LEDGERCODE
     where v.KeyNo == keyNo
     orderby v.DbCr   // optional: Dr pehle, Cr baad me
     select new VoucherItemVM
     {
        
         DbCr = v.DbCr,                 // 1 = Dr, 2 = Cr
         ParticularCode = v.ParticularCode,
         ParticularName = l.ledger,     // Ledger Name
         Amount = v.Amount
     }
 ).ToList();

            return new VoucherVM
            {
                KeyNo = mast.KeyNo,
                VoucherType = mast.VoucherType,
                SerialNo = mast.SerialNo,
                VoucherDate = mast.VoucherDate,
                ChequeNo = mast.ChequeNo,
                ChequeDate = mast.ChequeDate,
                Narration = mast.Narration,
                Remarks = mast.Remarks,
                PassedDate = mast.PassedDate,
                Items = items
            };
        }
    }

}




