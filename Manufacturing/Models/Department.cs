using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        public string DepartmentCode { get; set; }

        [Required]
        public string DepartmentName { get; set; }

        public string DepartmentHead {  get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public DateTime CreatedDate { get; set; }


    }
}
