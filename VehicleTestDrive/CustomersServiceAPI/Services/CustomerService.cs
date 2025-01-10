using CustomersServiceApi.Data;
using CustomersServiceApi.Interfaces;
using CustomersServiceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomersServiceApi.Services
{
    public class CustomerService : ICustomerService
    {
        private ApiDbContext dbContext;
        public CustomerService()
        {
            dbContext = new ApiDbContext();
        }
        public async Task AddCustomer(Customer customer)
        {
            var vehicleInDb = await dbContext.Vehicles.FirstOrDefaultAsync(v => v.Id == customer.VehicleId);
            if (vehicleInDb == null)
            {
                await dbContext.Vehicles.AddAsync(customer.Vehicle);
                await dbContext.SaveChangesAsync();
            }
            customer.Vehicle = null;
            await dbContext.Customers.AddAsync(customer);
            await dbContext.SaveChangesAsync();
            
            


        }
    }
}
