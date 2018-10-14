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

            var flyMeContext = _context.Flight.Include(f => f.Airplane)
                .Include(a => a.DestAirport)
                .Include(s => s.SourceAirport);
				
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
                .Include(a => a.DestAirport)
                .Include(s => s.SourceAirport).FirstOrDefaultAsync(m => m.Id == id);
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
            ViewData["DestAirport"] = new SelectList(_context.Airport, "DestAirportId", "DestAirportId");
            ViewData["SourceAirport"] = new SelectList(_context.Airport, "SourceAirportId", "SourceAirportId");
                return View();
        }

        public IActionResult Search(string AirplaneModel, string DestAirportName, string SourceAirportName)
        {
            var flights = _context.Flight.Include(f => f.Airplane)
                .Include(a => a.DestAirport)
                .Include(s => s.SourceAirport).AsQueryable();
            if (AirplaneModel != null) flights = flights.Where(s => s.Airplane.Model.Equals(AirplaneModel));
            if (DestAirportName != null) flights = flights.Where(s => s.DestAirport.Acronyms.Equals(DestAirportName));
            if (SourceAirportName != null) flights = flights.Where(s => s.SourceAirport.Acronyms.Equals(SourceAirportName));
            var result = flights.ToList(); // execute query
            return View(result);
        }

        // POST: Flights/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, int SourceAirportId, int DestAirportId, int AirplaneId, DateTime time)
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }
			
            if (SourceAirportId != 0 && DestAirportId != 0 && AirplaneId != 0 && time != null)
            {
                Flight flight = new Flight();
                flight.SourceAirport = _context.Airport.SingleOrDefault(a => a.ID.Equals(SourceAirportId));
                flight.DestAirport = _context.Airport.SingleOrDefault(a => a.ID.Equals(DestAirportId));
                flight.Airplane = _context.Airplane.SingleOrDefault(a => a.Id.Equals(AirplaneId));
                flight.Id = id;
                flight.Date = time;
                _context.Add(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                return View(flight);
            }
            else { return View(); }
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

            var flight = await _context.Flight
   .Include(f => f.Airplane)
   .Include(a => a.DestAirport)
   .Include(s => s.SourceAirport).FirstOrDefaultAsync(m => m.Id == id);
            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        // POST: Flights/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int SourceAirportId, int DestAirportId, int AirplaneId, DateTime time)
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }
			
            //var flight = await _context.Flight.FindAsync(id);
            var flight = await _context.Flight
				.Include(f => f.Airplane)
				.Include(a => a.DestAirport)
				.Include(s => s.SourceAirport).FirstOrDefaultAsync(m => m.Id == id);


            if (id != flight.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (SourceAirportId != 0 && DestAirportId != 0 && AirplaneId != 0 && time != null)
                    {
                        flight.SourceAirport = _context.Airport.SingleOrDefault(a => a.ID.Equals(SourceAirportId));
                        flight.DestAirport = _context.Airport.SingleOrDefault(a => a.ID.Equals(DestAirportId));
                        flight.Airplane = _context.Airplane.SingleOrDefault(a => a.Id.Equals(AirplaneId));
                        flight.Id = id;
                        flight.Date = time;
                        _context.Update(flight);
                        await _context.SaveChangesAsync();
                    } else { return View(); }
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
                .Include(a => a.DestAirport)
                .Include(s => s.SourceAirport).FirstOrDefaultAsync(m => m.Id == id);
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
