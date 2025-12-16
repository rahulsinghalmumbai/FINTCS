using FINTCS.Data;
using FINTCS.DTOs;
using FINTCS.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
            var paramName = new SqlParameter("@Name", name);
            var paramId = new SqlParameter("@Id", id);

            // Execute SP and get scalar result
            var count = _context.GroupMaster
                .FromSqlRaw("EXEC SP_IsDuplicateGroup @Name, @Id", paramName, paramId)
                .AsEnumerable()  // Execute query in memory
                .Count();        // SP returns count, so we can take count

            return count > 0;
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
            return _context.GroupMaster.ToList();
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

            var prms = new[]
            {
        new SqlParameter("@LEDGERCODE", ledger.LEDGERCODE),
        new SqlParameter("@ledger", ledger.ledger),
        new SqlParameter("@OpeningBalance", ledger.OpeningBalance),
        new SqlParameter("@drcr", ledger.drcr),
        new SqlParameter("@CreatedOn", ledger.CreatedOn),
        new SqlParameter("@GroupId", ledger.GroupId)
    };

            _context.Database.ExecuteSqlRaw(
                "EXEC SP_InsertLedger @LEDGERCODE,@ledger,@OpeningBalance,@drcr,@CreatedOn,@GroupId",
                prms
            );
        }


        public void UpdateLedger(LEDGERAC ledger)
        {
            ledger.CreatedOn = DateTime.Now;

            var prms = new[]
            {
                new SqlParameter("@Id", ledger.Id),
                new SqlParameter("@LEDGERCODE", ledger.LEDGERCODE),
                new SqlParameter("@ledger", ledger.ledger),
                new SqlParameter("@OpeningBalance", ledger.OpeningBalance),
                new SqlParameter("@drcr", ledger.drcr),
                new SqlParameter("@CreatedOn", ledger.CreatedOn),
                new SqlParameter("@GroupId", ledger.GroupId),
                
            };

            _context.Database.ExecuteSqlRaw(
                "EXEC SP_UpdateLedger @Id,@LEDGERCODE,@ledger,@OpeningBalance,@drcr,@CreatedOn,@GroupId",
                prms
            );
        }

        // ------------------------------------------------------------
        // LEDGER CODE GENERATOR (Service Logic)
        // ------------------------------------------------------------

        public string GenerateLedgerCode(int groupId)
        {
            var group = GetGroupById(groupId);

            // 🔹 Null check: agar LedgerCount null hai to 2 set karo
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

            UpdateLedger(existing);
        }
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

    }
}
