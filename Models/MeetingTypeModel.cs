using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class MeetingTypeModel
    {
        [Key]
        public int MeetingTypeID { get; set; }

        [Required(ErrorMessage ="Meeting type name must be required")]
        [StringLength(100)]
        public string MeetingTypeName { get; set; }

        [Required(ErrorMessage = "Remark must be required")]
        [StringLength(100)]
        public string Remarks { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime Created { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime Modified { get; set; }
    }
}
