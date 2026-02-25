using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class MeetingMemberModel
    {
        [Key]
   
        public int MeetingMemberID { get; set; }

        public int MeetingID { get; set; }
        public int StaffID { get; set; }
        [Required]
        public bool IsPresent {  get; set; }
        [StringLength(250)]
        public string? Remarks { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime Created { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime Modified { get; set; }
    }
}
