using Azure.Messaging.ServiceBus;
using MassTransit.Testing;
using MessageBroker;
using MessageBroker.Consumer;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReservationsApi.Data;
using ReservationsApi.Interfaces;
using ReservationsApi.Models;
using System.Net;
using System.Net.Mail;

namespace ReservationsApi.Services
{
    public class ReservationService : IReservation
    {
        private ApiDbContext _dbContext;
        private IMessageConsumer _messageConsumer;
        public ReservationService(IMessageConsumer messageConsumer)
        {
            _dbContext = new ApiDbContext();
            _messageConsumer = messageConsumer;
        }
        public async Task<List<Reservation>> GetReservations()
        {
            return new List<Reservation>();
            //added some comment
            //_messageConsumer.Receive(QueuesNames.RESERVATION, onReservationReceived);
            //return await _dbContext.Reservations.ToListAsync();
        }

        private async Task onReservationReceived(string message)
        {
            // Example: Deserialize the message
            var data = JsonConvert.DeserializeObject<Reservation>(message);

            // Perform custom behavior, e.g., saving to the database
            Console.WriteLine($"Processing message with ID: {data.Id}");
            string body = message;
            var messageCreated = JsonConvert.DeserializeObject<Reservation>(body);
            await _dbContext.Reservations.AddAsync(messageCreated);
            await _dbContext.SaveChangesAsync();
        }

    }
}
