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

            var flyMeContext = _context.Ticket
                .Include(t => t.Buyer)
                .Include(t => t.Flight)
                .Include(t => t.Flight.SourceAirport);

            return View(await flyMeContext.ToListAsync());
        }

        public IActionResult Search(int Price, int LuggageWeight, int Id)
        {
            var tickets = _context.Ticket.AsQueryable();
            if (Price != 0) tickets = tickets.Where(s => s.Price.Equals(Price));
            if (LuggageWeight != 0) tickets = tickets.Where(s => s.LuggageWeight.Equals(LuggageWeight));
            if (Id != 0 && Id != null) tickets = tickets.Where(s => s.Id.Equals(Id));
            var result = tickets.ToList(); // execute query
            return View(result);
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
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

            ViewBag.FlightID = new SelectList(_context.Flight, "Id", "Id");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Id,FlightID,Price,LuggageWeight")] Ticket ticket)
        {

         UsersController.CheckIfLoginAndManager(this, _context);

        if (ViewBag.IsManager == null || !ViewBag.IsManager)
        {
            return Unauthorized();
        }

        if (ticket.FlightID == 0)
        {
                ViewBag.FlightID = new SelectList(_context.Flight, "Id", "Id");
                ViewBag.ErrorMessage = "You must choose a flight!";
                return View("Create");
        }

        if (ModelState.IsValid)
        {
            _context.Ticket.Add(ticket);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

            ViewBag.FlightID = new SelectList(_context.Flight, "Id", "Id");
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
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }
			
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
