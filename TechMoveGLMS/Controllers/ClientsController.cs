using Microsoft.AspNetCore.Mvc;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services;

namespace TechMoveGLMS.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ITechMoveApiClient _apiClient;

        public ClientsController(ITechMoveApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var clients = await _apiClient.GetClientsAsync();
            return View(clients);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (!ModelState.IsValid)
            {
                var clients = await _apiClient.GetClientsAsync();
                return View("Index", clients);
            }

            var created = await _apiClient.CreateClientAsync(client);
            if (created is null)
            {
                TempData["Error"] = "Client could not be created.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
