using FINTCS.Data;
using FINTCS.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FINTCS.Repositories
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _db;

        public GenericService(ApplicationDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        
        // ✅ SP for Society Upsert
        public async Task<int> UpsertSocietyAsync(Society society)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", society.Id),
                new SqlParameter("@Name", society.Name ?? ""),
                new SqlParameter("@Address", society.Address ?? ""),
                new SqlParameter("@City", society.City ?? ""),
                new SqlParameter("@Pin", society.Pin ?? ""),
                new SqlParameter("@Phone", society.Phone ?? ""),
                new SqlParameter("@Mobile1", society.Mobile1 ?? ""),
                new SqlParameter("@Mobile2", society.Mobile2 ?? ""),
                new SqlParameter("@Mobile3", society.Mobile3 ?? ""),
                new SqlParameter("@Email1", society.Email1 ?? ""),
                new SqlParameter("@Email2", society.Email2 ?? ""),
                new SqlParameter("@Email3", society.Email3 ?? ""),
                new SqlParameter("@Website", society.Website ?? ""),
                new SqlParameter("@RegistrationNo", society.RegistrationNo ?? ""),
                new SqlParameter("@CheckBounceCharges", society.CheckBounceCharges),
                new SqlParameter("@ChargesType", society.ChargesType ?? ""),
                new SqlParameter("@Shares", society.Shares),
                new SqlParameter("@OD", society.OD),
                new SqlParameter("@CD", society.CD),
                new SqlParameter("@Dividend", society.Dividend)
            };

            if (society.Id == 0)
            {
                var newId = await _context.Societies.FromSqlRaw(
                    "EXEC UpsertSociety @Id, @Name, @Address, @City, @Pin, @Phone, @Mobile1, @Mobile2, @Mobile3, @Email1, @Email2, @Email3, @Website, @RegistrationNo, @CheckBounceCharges, @ChargesType, @Shares, @OD, @CD, @Dividend",
                    parameters).Select(s => s.Id).FirstOrDefaultAsync();

                return newId;
            }
            else
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC UpsertSociety @Id, @Name, @Address, @City, @Pin, @Phone, @Mobile1, @Mobile2, @Mobile3, @Email1, @Email2, @Email3, @Website, @RegistrationNo, @CheckBounceCharges, @ChargesType, @Shares, @OD, @CD, @Dividend",
                    parameters);

                return society.Id;
            }
        }

        // ✅ SP for LoanMaster Upsert
        public async Task<int> UpsertLoanMasterAsync(LoanMaster loan)
        {
            var paramId = new SqlParameter("@LoanMasterId", loan.LoanMasterId)
            { Direction = System.Data.ParameterDirection.InputOutput };

            var paramName = new SqlParameter("@LoanName", loan.LoanName ?? "");
            var paramMultiple = new SqlParameter("@MultipleTimes", loan.MultipleTimes);
            var paramMaxLimit = new SqlParameter("@MaxLimit", loan.MaxLimit);
            var paramInt = new SqlParameter("@LoanInt", loan.LoanInt);
            var paramSociety = new SqlParameter("@SocietyId", loan.SocietyId);

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC UpsertLoanMaster @LoanMasterId OUTPUT, @LoanName, @MultipleTimes, @MaxLimit, @LoanInt, @SocietyId",
                paramId, paramName, paramMultiple, paramMaxLimit, paramInt, paramSociety
            );

            return (int)paramId.Value;
        }
        public async Task<IEnumerable<TBLDEF>> GetDropdownListAsync()
        {
            return await _context.TBLDEF
                .FromSqlRaw("EXEC GetAllTBLDEF")
                .ToListAsync();
        }

        // For TBLDAT Upsert
        public async Task<int> UpsertTBLDATAsync(TBLDAT data)
        {
            var paramId = new SqlParameter("@TBLSSERN", data.TBLSSERN)
            { Direction = System.Data.ParameterDirection.InputOutput };

            var paramDTBLSERN = new SqlParameter("@DTBLSERN", data.DTBLSERN);
            var paramDesc = new SqlParameter("@TBLSDESC", data.TBLSDESC ?? "");

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC UpsertTBLDAT @TBLSSERN OUTPUT, @DTBLSERN, @TBLSDESC",
                paramId, paramDTBLSERN, paramDesc
            );

            return (int)paramId.Value;
        }
        public async Task<T?> GetFirstAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null
                ? await _db.FirstOrDefaultAsync()
                : await _db.FirstOrDefaultAsync(predicate);
        }



        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate) => await _db.Where(predicate).ToListAsync();



        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}
