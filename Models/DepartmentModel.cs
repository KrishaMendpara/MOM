using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class DepartmentModel
    {
        [Key]
       
        public int DepartmentID { get; set; }

        [Required(ErrorMessage = " Department Name must be enter")]
        [StringLength(100)]
        [Display(Name ="Department Name")]
        public string DepartmentName { get; set; }
        
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime Created { get; set; }

       
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime Modified { get; set; }
    }
}
