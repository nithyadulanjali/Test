using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSLSystem.Models
{
    public class Registration
    {
        [Required(ErrorMessage = "First Name is required.")]
        public string FIRST_NAME { get; set; }

        [StringLength(100)]
        public string LAST_NAME { get; set; }

        [StringLength(100)]
        public string DESIGNATION { get; set; }

        [Required(ErrorMessage = "Please select a company.")]
        public string SelectedCompanyCode { get; set; } 
        public string SelectedCompany { get; set; } 

        public string DOB { get; set; }

        [Required(ErrorMessage = "Contact number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact number must be exactly 10 digits and cannot contain letters or symbols.")]
        public string CONTACT_NO { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string E_MAIL { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Password must be 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string PASSWORD { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("PASSWORD", ErrorMessage = "The password and confirmation password do not match.")]
        public string CON_PASSWORD { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string GENDER { get; set; }

        [Required(ErrorMessage = "House number is required.")]
        public string HouseNo { get; set; }

        [Required(ErrorMessage = "Street is required.")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        public string Address
        {
            get
            {
                return $"{HouseNo}/{Street}/{City}";
            }
        }

        public class Company
        {
            public string COMPANY_CODE { get; set; }
            public string COMPANY_DES { get; set; }
        }
    }

}