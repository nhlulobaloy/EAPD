using TechMoveGLMS.Models;

namespace TechMoveGLMS.Services
{
    public interface IContractValidator
    {
        bool CanCreateServiceRequest(Contract contract);
    }

    public class ContractValidator : IContractValidator
    {
        public bool CanCreateServiceRequest(Contract contract)
        {
            if (contract == null) return false;

            if (contract.Status == "Expired" || contract.Status == "On Hold")
            {
                return false;
            }

            return true;
        }
    }
}