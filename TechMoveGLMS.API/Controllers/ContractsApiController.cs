using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.API.Data;
using TechMoveGLMS.API.Models;

namespace TechMoveGLMS.API.Controllers
{
    [Route("api/contracts")]
    [ApiController]
    public class ContractsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContractsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contract>>> GetContracts(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? status)
        {
            var query = _context.Contracts
                .Include(contract => contract.Client)
                .Include(contract => contract.ServiceRequests)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(contract => contract.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(contract => contract.EndDate <= endDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(contract => contract.Status == status);
            }

            return await query.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Contract>> GetContract(int id)
        {
            var contract = await _context.Contracts
                .Include(contract => contract.Client)
                .Include(contract => contract.ServiceRequests)
                .FirstOrDefaultAsync(contract => contract.Id == id);

            if (contract is null)
            {
                return NotFound();
            }

            return contract;
        }

        [HttpPost]
        public async Task<ActionResult<Contract>> CreateContract(Contract contract)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContract), new { id = contract.Id }, contract);
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateContractStatus(int id, StatusUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Status))
            {
                return BadRequest("Status is required.");
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract is null)
            {
                return NotFound();
            }

            contract.Status = request.Status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class StatusUpdateRequest
        {
            public string Status { get; set; } = string.Empty;
        }
    }
}
