using System.Net.Http.Json;
using TechMoveGLMS.Models;

namespace TechMoveGLMS.Services
{
    public class TechMoveApiClient : ITechMoveApiClient
    {
        private readonly HttpClient _httpClient;

        public TechMoveApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<Client>> GetClientsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Client>>("api/clients")
                ?? new List<Client>();
        }

        public async Task<Client?> CreateClientAsync(Client client)
        {
            var response = await _httpClient.PostAsJsonAsync("api/clients", client);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Client>();
        }

        public async Task<IReadOnlyList<Contract>> GetContractsAsync(DateTime? startDate, DateTime? endDate, string? status)
        {
            var query = new List<string>();

            if (startDate.HasValue)
            {
                query.Add($"startDate={Uri.EscapeDataString(startDate.Value.ToString("O"))}");
            }

            if (endDate.HasValue)
            {
                query.Add($"endDate={Uri.EscapeDataString(endDate.Value.ToString("O"))}");
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query.Add($"status={Uri.EscapeDataString(status)}");
            }

            var path = query.Count == 0
                ? "api/contracts"
                : $"api/contracts?{string.Join("&", query)}";

            return await _httpClient.GetFromJsonAsync<List<Contract>>(path)
                ?? new List<Contract>();
        }

        public async Task<Contract?> GetContractAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Contract>($"api/contracts/{id}");
        }

        public async Task<Contract?> CreateContractAsync(Contract contract)
        {
            var response = await _httpClient.PostAsJsonAsync("api/contracts", contract);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Contract>();
        }

        public async Task<bool> UpdateContractStatusAsync(int id, string status)
        {
            var response = await _httpClient.PatchAsJsonAsync($"api/contracts/{id}/status", new { status });
            return response.IsSuccessStatusCode;
        }

        public async Task<ServiceRequest?> CreateServiceRequestAsync(ServiceRequest serviceRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/service-requests", serviceRequest);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ServiceRequest>();
        }
    }
}
