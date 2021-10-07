using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyService.Data;
using MyService.Models;
using Newtonsoft.Json;

namespace MyService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyServiceContext _context;
        private readonly ServiceBusClient _client;


        public UsersController(MyServiceContext context, ServiceBusClient client)
        {
            _context = context;
            _client = client;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            user.Id = Guid.NewGuid();
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            await CreatedUserEvent(user);

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        private async Task CreatedUserEvent(User user)
        {
            var sender = _client.CreateSender("SC2021");
            var createdUser = new Messages.User()
            {
                Name = user.Name,
                LastName = user.LastName,
                MailAddress = user.MailAddress,
                BirthDate = user.BirthDate
            };
            var msg = new ServiceBusMessage
            {
                Body = new BinaryData(JsonConvert.SerializeObject(createdUser))
            };

            await sender.SendMessageAsync(msg);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
