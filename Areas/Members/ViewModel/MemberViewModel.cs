using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace FINTCS.Areas.Members.ViewModel
{
    public class MemberViewModel
    {
        public int Id { get; set; }

        // Step 1 - General
        [Required]
        public string Memno { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string FatherName { get; set; }
        [Required]
        public string OfficeAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Branch { get; set; }
        [Required]
        public string Designation { get; set; }
        public string Mobile1 { get; set; }
        public string Mobile2 { get; set; }
        public string ResidenceAddress { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? DOJSociety { get; set; }
        public string Email { get; set; }
        public DateTime? DOJ { get; set; }
        public DateTime? DOR { get; set; }
        public string Nominee { get; set; }
        public string NomineeRelation { get; set; }

        // Step 2 - Photo & Opening
        public decimal? Share { get; set; }
        public string ShareType { get; set; }
        public decimal? CD { get; set; }
        public string CDType { get; set; }
        public string BankName { get; set; }
        public string PayableAt { get; set; }
        public string AccountNo { get; set; }
        public string Status { get; set; }
        public DateTime? Date { get; set; }
        public string PhotoPath { get; set; }
        public string SignaturePath { get; set; }
        public IFormFile PhotoFile { get; set; }
        public IFormFile SignatureFile { get; set; }

        // Step 3 - Monthly Deduction
        public decimal? DeductionShare { get; set; }
        public decimal? WithDrawl { get; set; }
        public decimal? GLoanInstalment { get; set; }
        public decimal? ELoanInstalment { get; set; }
    }
}
