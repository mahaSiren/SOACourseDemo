using CustomersServiceApi.Models;

namespace CustomersServiceApi.Interfaces
{
    public interface ICustomerService
    {
        Task AddCustomer(Customer customer);
    }
}
