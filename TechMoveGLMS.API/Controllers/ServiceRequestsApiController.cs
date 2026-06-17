using Microsoft.AspNetCore.Mvc;
using TechMoveGLMS.API.Data;
using TechMoveGLMS.API.Models;

namespace TechMoveGLMS.API.Controllers
{
    [Route("api/service-requests")]
    [ApiController]
    public class ServiceRequestsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceRequest>> CreateServiceRequest(ServiceRequest serviceRequest)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var contractExists = await _context.Contracts.FindAsync(serviceRequest.ContractId);
            if (contractExists is null)
            {
                return BadRequest("Contract not found.");
            }

            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateServiceRequest), new { id = serviceRequest.Id }, serviceRequest);
        }
    }
}
