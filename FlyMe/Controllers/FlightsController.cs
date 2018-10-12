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
    public class FlightsController : Controller
    {
        private readonly FlyMeContext _context;

        public FlightsController(FlyMeContext context)
        {
            _context = context;
        }

        // GET: Flights
        public async Task<IActionResult> Index()
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

            var flyMeContext = _context.Flight.Include(f => f.Airplane);
            return View(await flyMeContext.ToListAsync());
        }

        // GET: Flights/Details/5
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

            var flight = await _context.Flight
                .Include(f => f.Airplane)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // GET: Flights/Create
        public IActionResult Create()
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

            ViewData["AirplaneId"] = new SelectList(_context.Airplane, "Id", "Id");
            ViewData["DestAirport"] = new SelectList(_context.Airport);
            return View();
        }

        // POST: Flights/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SourceAirportName,DestAirportName,DestAirport,SourceAirport,AirplaneId,Date")] Flight flight)
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

            flight.SourceAirport = _context.Airport.SingleOrDefault(a => a.Acronyms.Equals(flight.SourceAirportName));
                flight.DestAirport = _context.Airport.SingleOrDefault(a => a.Acronyms.Equals(flight.DestAirportName));
                flight.Airplane = _context.Airplane.SingleOrDefault(a => a.Id.Equals(flight.AirplaneId));
                _context.Add(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            ViewData["AirplaneId"] = new SelectList(_context.Airplane, "Id", "Id", flight.AirplaneId);
            ViewData["DestAirport"] = new SelectList(_context.Airport, flight.DestAirport);
            return View(flight);
        }

        // GET: Flights/Edit/5
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

            var flight = await _context.Flight.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }
            ViewData["AirplaneId"] = new SelectList(_context.Airplane, "Id", "Id", flight.AirplaneId);
            return View(flight);
        }

        // POST: Flights/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DestAirport,SourceAirport,AirplaneId,Date")] Flight flight)
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

            if (id != flight.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flight);
                    await _context.SaveChangesAsync();
                    flight.SourceAirport = _context.Airport.SingleOrDefault(a => a.Acronyms.Equals(flight.SourceAirportName)); flight.SourceAirport = _context.Airport.SingleOrDefault(a => a.Acronyms.Equals(flight.DestAirportName));
                    flight.DestAirport = _context.Airport.SingleOrDefault(a => a.Acronyms.Equals(flight.DestAirportName));
                    flight.Airplane = _context.Airplane.SingleOrDefault(a => a.Id.Equals(flight.AirplaneId));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExists(flight.Id))
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
            ViewData["AirplaneId"] = new SelectList(_context.Airplane, "Id", "Id", flight.AirplaneId);
            return View(flight);
        }

        // GET: Flights/Delete/5
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

            var flight = await _context.Flight
                .Include(f => f.Airplane)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _context.Flight.FindAsync(id);
            _context.Flight.Remove(flight);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightExists(int id)
        {
            return _context.Flight.Any(e => e.Id == id);
        }
    }
}
