using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class StaffModel
    {
        [Key]
        public int StaffID { get; set; }

        public int DepartmentID { get; set; }

        [Required(ErrorMessage = " Staff name must be required")]
        [StringLength(50)]
        public string StaffName { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[0-9]\d{9}$", ErrorMessage = "Enter valid 10 digit mobile number")]
        public string MobileNo { get; set; }

     

        [Required(ErrorMessage = "Email address is required")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
    ErrorMessage = "Invalid email address format")]
        public string EmailAddress { get; set; }


        [StringLength(250)]
        public string? Remarks {  get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime Created { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime Modified { get; set; }
    }
}
