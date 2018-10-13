using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlyMe.Data;
using FlyMe.Models;

namespace FlyMe.Controllers
{
    public class TicketsController : Controller
    {
        private readonly FlyMeContext _context;

        public TicketsController(FlyMeContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var flyMeContext = _context.Ticket
                .Include(t => t.Buyer)
                .Include(t => t.Flight)
                .Include(t => t.Flight.SourceAirport);


            return View(await flyMeContext.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .Include(t => t.Buyer)
                .Include(t => t.Flight)
                .Include(t => t.Flight.SourceAirport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.User, "ID", "Email");
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "DestAirport");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int FlightId, int UserId, int id, int LuggageWeight, int price)
        {
            if (FlightId != 0 & UserId != 0 && LuggageWeight != 0 && price != 0)
            {
                Ticket ticket = new Ticket();
                ticket.Id = id;
                ticket.Price = price;
                ticket.LuggageWeight = LuggageWeight;
                ticket.Flight = _context.Flight.SingleOrDefault(a => a.Id.Equals(FlightId));
                ticket.Buyer = _context.User.SingleOrDefault(a => a.ID.Equals(UserId));
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                ViewData["UserId"] = new SelectList(_context.User, "ID", "Email", ticket.Buyer.ID);
                ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "DestAirport", ticket.Flight.Id);
                return View(ticket);
            } else { return View(); }
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

           var ticket = await _context.Ticket
                .Include(t => t.Buyer)
                .Include(t => t.Flight)
                .Include(t => t.Flight.SourceAirport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.User, "ID", "Email", ticket.Buyer.ID);
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "DestAirport", ticket.Flight.Id);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int FlightId,int Price,int LuggageWeight, int UserId)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (FlightId != 0 & UserId != 0 && LuggageWeight != 0 && Price != 0)
                    {
                        ticket.Id = id;
                        ticket.Price = Price;
                        ticket.LuggageWeight = LuggageWeight;
                        ticket.Flight = _context.Flight.SingleOrDefault(a => a.Id.Equals(FlightId));
                        ticket.Buyer = _context.User.SingleOrDefault(a => a.ID.Equals(UserId));
                        _context.Add(ticket);
                        await _context.SaveChangesAsync();
                        ViewData["UserId"] = new SelectList(_context.User, "ID", "Email", ticket.Buyer.ID);
                        ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "DestAirport", ticket.Flight.Id);
                    }
                    else { return View(); }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .Include(t => t.Buyer)
                .Include(t => t.Flight)
                .Include(t => t.Flight.SourceAirport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            _context.Ticket.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Ticket.Any(e => e.Id == id);
        }
    }
}
