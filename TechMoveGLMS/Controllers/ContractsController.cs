using Microsoft.AspNetCore.Mvc;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services;

namespace TechMoveGLMS.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ITechMoveApiClient _apiClient;
        private readonly IFileService _fileService;

        public ContractsController(ITechMoveApiClient apiClient, IFileService fileService)
        {
            _apiClient = apiClient;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, string? status)
        {
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.Status = status;

            var contracts = await _apiClient.GetContractsAsync(startDate, endDate, status);
            return View(contracts);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var contract = await _apiClient.GetContractAsync(id);
            if (contract is null)
            {
                return NotFound();
            }

            return View(contract);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Clients = (await _apiClient.GetClientsAsync()).ToList();
            return View(new Contract());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile? signedAgreement)
        {
            if (signedAgreement is not null)
            {
                if (!_fileService.IsValidPdfFile(signedAgreement))
                {
                    ModelState.AddModelError(nameof(signedAgreement), "Only PDF files are allowed.");
                }
                else
                {
                    contract.SignedAgreementPath = await _fileService.SavePdfFile(signedAgreement);
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Clients = (await _apiClient.GetClientsAsync()).ToList();
                return View(contract);
            }

            var created = await _apiClient.CreateContractAsync(contract);
            if (created is null)
            {
                ModelState.AddModelError("", "Contract could not be created.");
                ViewBag.Clients = (await _apiClient.GetClientsAsync()).ToList();
                return View(contract);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var updated = await _apiClient.UpdateContractStatusAsync(id, status);
            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var contract = await _apiClient.GetContractAsync(id);
            if (contract is null || string.IsNullOrWhiteSpace(contract.SignedAgreementPath))
            {
                return NotFound();
            }

            return Redirect(contract.SignedAgreementPath);
        }
    }
}
