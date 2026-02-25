using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class MeetingVenueModel
    {
        [Key]
        public int MeetingVenueID { get; set; }

        [Required(ErrorMessage = "Meeting venue name must be required")]
        [StringLength(100)]
        public string MeetingVenueName { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime Created { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime Modified { get; set; }
    }
}
