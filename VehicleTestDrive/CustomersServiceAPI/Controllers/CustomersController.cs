using CustomersServiceApi.Interfaces;
using CustomersServiceApi.Models;
using MessageBroker;
using MessageBroker.Publisher;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomersServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController(ICustomerService _customerService, IPublisher<Customer> _publisher) : ControllerBase
    {
        // POST api/<CustomersController>
        [HttpPost]
        public async Task Post([FromBody] Customer customer)
        {
            if (customer != null)
            {
                await _customerService.AddCustomer(customer);
                await _publisher.PublishMessageAsync(customer, QueuesNames.RESERVATION);
            }
        }

    }
}
