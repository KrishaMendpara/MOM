using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class MeetingsModel
    {
        [Key]
        
        public int MeetingsID { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime MeetingDate{ get; set; }

        public int MeetingVenueID{ get;   set; }
        public int MeetingTypeID { get; set; }
        public int DepartmentID { get; set; }
        [StringLength(250)]
        public string? MeetingDescription { get; set; }
        [StringLength(250)]
        public string? DocumentPath { get;set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime Created { get; set; }= DateTime.Now;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime Modified { get; set; }

        public bool? IsCancelled { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime? CancellationDateTime { get; set; }

        [StringLength(250)]
        public string? CancellationReason { get; set; }
    }
}
