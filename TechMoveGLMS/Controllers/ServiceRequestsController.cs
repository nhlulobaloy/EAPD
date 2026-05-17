using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services;

namespace TechMoveGLMS.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrencyService _currencyService;
        private readonly IContractValidator _contractValidator;

        public ServiceRequestsController(
            ApplicationDbContext context,
            ICurrencyService currencyService,
            IContractValidator contractValidator)
        {
            _context = context;
            _currencyService = currencyService;
            _contractValidator = contractValidator;
        }

        public async Task<IActionResult> Create(int contractId)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == contractId);

            if (contract == null)
            {
                return NotFound();
            }

            if (!_contractValidator.CanCreateServiceRequest(contract))
            {
                TempData["Error"] = $"Cannot create service request. Contract status is: {contract.Status}";
                return RedirectToAction("Index", "Contracts");
            }

            var rate = await _currencyService.GetUsdToZarRate();

            ViewBag.Contract = contract;
            ViewBag.ExchangeRate = rate;

            return View(new ServiceRequest { ContractId = contractId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest, decimal exchangeRate)
        {
            var contract = await _context.Contracts.FindAsync(serviceRequest.ContractId);
            if (contract == null)
            {
                ModelState.AddModelError("", "Contract not found");
                return View(serviceRequest);
            }

            if (ModelState.IsValid)
            {
                serviceRequest.CostZAR = _currencyService.ConvertUsdToZar(serviceRequest.CostUSD, exchangeRate);
                serviceRequest.Status = "Pending";
                serviceRequest.CreatedAt = DateTime.UtcNow;

                _context.Add(serviceRequest);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Contracts");
            }

            ViewBag.Contract = contract;
            return View(serviceRequest);
        }

        [HttpGet]
        public async Task<IActionResult> GetExchangeRate()
        {
            var rate = await _currencyService.GetUsdToZarRate();
            return Json(new { rate });
        }
    }
}