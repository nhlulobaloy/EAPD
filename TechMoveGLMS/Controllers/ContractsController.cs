using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services;

namespace TechMoveGLMS.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public ContractsController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, string status)
        {
            var query = _context.Contracts.Include(c => c.Client).AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(c => c.StartDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(c => c.EndDate <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(c => c.Status == status);
            }

            var contracts = await query.ToListAsync();

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.Status = status;

            return View(contracts);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Clients = await _context.Clients.ToListAsync();
            return View();
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile SignedAgreement)
        {
            Console.WriteLine("=== CREATE METHOD CALLED ===");
            Console.WriteLine($"ClientId: {contract.ClientId}");
            Console.WriteLine($"StartDate: {contract.StartDate}");
            Console.WriteLine($"EndDate: {contract.EndDate}");
            Console.WriteLine($"Status: {contract.Status}");
            Console.WriteLine($"ServiceLevel: {contract.ServiceLevel}");
            Console.WriteLine($"File: {(SignedAgreement != null ? SignedAgreement.FileName : "NO FILE")}");
            Console.WriteLine($"File Size: {(SignedAgreement != null ? SignedAgreement.Length.ToString() : "0")}");

            // REMOVE THIS LINE BELOW - DELETE IT
            // ViewBag.Clients = await _context.Clients.ToListAsync();

            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState is VALID");

                // Handle file upload
                if (SignedAgreement != null && SignedAgreement.Length > 0)
                {
                    Console.WriteLine("File received, attempting to save...");
                    try
                    {
                        contract.SignedAgreementPath = await _fileService.SavePdfFile(SignedAgreement);
                        Console.WriteLine($"File saved at: {contract.SignedAgreementPath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ERROR saving file: {ex.Message}");
                        ModelState.AddModelError("", ex.Message);
                        ViewBag.Clients = await _context.Clients.ToListAsync();
                        return View(contract);
                    }
                }
                else
                {
                    Console.WriteLine("No file uploaded - continuing without PDF");
                }

                if (string.IsNullOrEmpty(contract.Status))
                {
                    contract.Status = "Draft";
                }

                Console.WriteLine("Saving contract to database...");
                _context.Add(contract);
                await _context.SaveChangesAsync();
                Console.WriteLine("Contract saved successfully!");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                Console.WriteLine("ModelState is INVALID");
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        Console.WriteLine($"Error in {key}: {error.ErrorMessage}");
                    }
                }
            }

            ViewBag.Clients = await _context.Clients.ToListAsync();
            return View(contract);
        }

        public async Task<IActionResult> DownloadPdf(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementPath))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                contract.SignedAgreementPath.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, "application/pdf", Path.GetFileName(filePath));
        }

        public async Task<IActionResult> Details(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ServiceRequests)
                .FirstOrDefaultAsync(c => c.Id == id);
            return View(contract);
        }
    }
}