using TechMoveGLMS.Models;

namespace TechMoveGLMS.Services
{
    public interface ITechMoveApiClient
    {
        Task<IReadOnlyList<Client>> GetClientsAsync();
        Task<Client?> CreateClientAsync(Client client);
        Task<IReadOnlyList<Contract>> GetContractsAsync(DateTime? startDate, DateTime? endDate, string? status);
        Task<Contract?> GetContractAsync(int id);
        Task<Contract?> CreateContractAsync(Contract contract);
        Task<bool> UpdateContractStatusAsync(int id, string status);
        Task<ServiceRequest?> CreateServiceRequestAsync(ServiceRequest serviceRequest);
    }
}
