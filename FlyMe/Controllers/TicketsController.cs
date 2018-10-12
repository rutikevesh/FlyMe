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
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

            var flyMeContext = _context.Ticket.Include(t => t.Buyer).Include(t => t.Flight);
            return View(await flyMeContext.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .Include(t => t.Buyer)
                .Include(t => t.Flight)
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
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

            ViewData["UserId"] = new SelectList(_context.User, "ID", "Email");
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "DestAirport");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FlightId,Price,Seat,LuggageWeight,UserId")] Ticket ticket)
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                ticket.Flight = _context.Flight.SingleOrDefault(a => a.Id.Equals(ticket.FlightId));
                ticket.Buyer = _context.User.SingleOrDefault(a => a.ID.Equals(ticket.UserId));
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "ID", "Email", ticket.UserId);
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "DestAirport", ticket.FlightId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.User, "ID", "Email", ticket.UserId);
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "DestAirport", ticket.FlightId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FlightId,Price,Seat,LuggageWeight,UserId")] Ticket ticket)
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ticket.Flight = _context.Flight.SingleOrDefault(a => a.Id.Equals(ticket.FlightId));
                    ticket.Buyer = _context.User.SingleOrDefault(a => a.ID.Equals(ticket.UserId));
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
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
            ViewData["UserId"] = new SelectList(_context.User, "ID", "Email", ticket.UserId);
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "DestAirport", ticket.FlightId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .Include(t => t.Buyer)
                .Include(t => t.Flight)
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
