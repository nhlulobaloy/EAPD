using System.ComponentModel.DataAnnotations;

namespace TechMoveGLMS.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required]
        public string Status { get; set; } = "Draft";

        public string ServiceLevel { get; set; } = string.Empty;
        public string? SignedAgreementPath { get; set; }

        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}