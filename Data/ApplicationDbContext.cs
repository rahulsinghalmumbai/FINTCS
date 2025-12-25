using FINTCS.Areas.Members.Models;
using FINTCS.DTOs;
using FINTCS.Models;
using FINTCS.ViewModels;
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

        public DbSet<GroupMaster> GroupMaster { get; set; }
        public DbSet<LEDGERAC> LEDGERAC { get; set; }

        public DbSet<LedgerListDto> LedgerListDto { get; set; }

        public DbSet<LoanMasterMember> LoanMasterMember { get; set; }
        public DbSet<MastVoc> Mast_Voc { get; set; }
        public DbSet<Voucher> Voucher { get; set; }
    }
}