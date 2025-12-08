using FINTCS.Areas.Members.Models;

using FINTCS.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FINTCS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Society> Societies { get; set; }
        public DbSet<LoanMaster> LoanMasters { get; set; }
        public DbSet<TBLDEF> TBLDEF { get; set; }

        public DbSet<TBLDAT> TblDat { get; set; }
        public DbSet<Member> Members { get; set; }

    }
}