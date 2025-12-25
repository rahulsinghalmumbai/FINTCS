using FINTCS.Areas.Members.ViewModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace FINTCS.Areas.Members.Models
{
    public class Member
    {
        public int Id { get; set; }

       
        public string? Memno { get; set; }


        public string? Name { get; set; }

        public string? FatherName { get; set; }
        public string? OfficeAddress { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Branch { get; set; }
        public string? Designation { get; set; }
        public string? Mobile1 { get; set; }
        public string? Mobile2 { get; set; }
        public string? ResidenceAddress { get; set; }

      
        public DateTime? DOB { get; set; }

    
        public DateTime? DOJSociety { get; set; }

       
        public string? Email { get; set; }

     
        public DateTime? DOJ { get; set; }

 
        public DateTime? DOR { get; set; }

        public string? Nominee { get; set; }
        public string? NomineeRelation { get; set; }
        public decimal? Share { get; set; }
        public string? ShareType { get; set; }
        public decimal? CD { get; set; }
        public string? CDType { get; set; }
        public string? BankName { get; set; }
        public string? PayableAt { get; set; }
        public string? AccountNo { get; set; }
        public string? Status { get; set; }
        public DateTime? Date { get; set; }
        public string? PhotoPath { get; set; }
        public string? SignaturePath { get; set; }
        public decimal? DeductionShare { get; set; }
        public decimal? WithDrawl { get; set; }
        public decimal? GLoanInstalment { get; set; }
        public decimal? ELoanInstalment { get; set; }
        public List<LoanAmountVM> ?LoanAmounts { get; set; } = new();
    }
}
