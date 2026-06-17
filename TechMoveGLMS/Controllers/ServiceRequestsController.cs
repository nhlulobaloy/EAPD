using Microsoft.AspNetCore.Mvc;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services;

namespace TechMoveGLMS.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ITechMoveApiClient _apiClient;
        private readonly ICurrencyService _currencyService;
        private readonly IContractValidator _contractValidator;

        public ServiceRequestsController(
            ITechMoveApiClient apiClient,
            ICurrencyService currencyService,
            IContractValidator contractValidator)
        {
            _apiClient = apiClient;
            _currencyService = currencyService;
            _contractValidator = contractValidator;
        }

        public async Task<IActionResult> Create(int contractId)
        {
            var contract = await _apiClient.GetContractAsync(contractId);

            if (contract is null)
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
            var contract = await _apiClient.GetContractAsync(serviceRequest.ContractId);
            if (contract is null)
            {
                ModelState.AddModelError("", "Contract not found");
                return View(serviceRequest);
            }

            if (ModelState.IsValid)
            {
                serviceRequest.CostZAR = _currencyService.ConvertUsdToZar(serviceRequest.CostUSD, exchangeRate);
                serviceRequest.Status = "Pending";
                serviceRequest.CreatedAt = DateTime.UtcNow;

                var created = await _apiClient.CreateServiceRequestAsync(serviceRequest);
                if (created is not null)
                {
                    return RedirectToAction("Index", "Contracts");
                }

                ModelState.AddModelError("", "Service request could not be created.");
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
