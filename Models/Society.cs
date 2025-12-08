using System.ComponentModel.DataAnnotations;

namespace FINTCS.Models
{
    public class Society
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string Pin { get; set; } = "";
        public string? Phone { get; set; } = "";

        // Mobile Validation (10 digits only)
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits")]
        public string Mobile1 { get; set; } = "";

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits")]
        public string? Mobile2 { get; set; } = "";

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits")]
        public string? Mobile3 { get; set; } = "";

        // Email Validation
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email1 { get; set; } = "";

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email2 { get; set; } = "";

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email3 { get; set; } = "";

        public string? Website { get; set; } = "";
        public string RegistrationNo { get; set; } = "";

        // Numeric-only validations
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Only numeric values allowed")]
        public decimal? CheckBounceCharges { get; set; } = 0;
        public string? ChargesType { get; set; } = "";

        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Only numeric values allowed")]
       
        public decimal Shares { get; set; } = 0;

        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Only numeric values allowed")]
        public decimal OD { get; set; } = 0;

        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Only numeric values allowed")]
        public decimal CD { get; set; } = 0;

        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Only numeric values allowed")]
        public decimal Dividend { get; set; } = 0;
    }
}
