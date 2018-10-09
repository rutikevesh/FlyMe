using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlyMe.Data;
using FlyMe.Models;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace FlyMe.Controllers
{
    public class UsersController : Controller
    {
        private readonly FlyMeContext _context;

        public UsersController(FlyMeContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserName,Password,FirstName,LastName,Email,IsManager")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Create
        public IActionResult SignUp()

        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("ID,UserName,Password,FirstName,LastName,Email")] User user)
        {
            if (ModelState.IsValid)
            {
                user.IsManager = false;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Create
        public IActionResult SignUpAsManager()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUpAsManager([Bind("ID,UserName,Password,FirstName,LastName,Email")] User user)
        {
            if (ModelState.IsValid)
            {
                user.IsManager = true;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,UserName,Password,FirstName,LastName,Email,IsManager")] User user)
        {
            if (id != user.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult FailedLogin()
        {
            return View("FailedLogin");
        }

        public ActionResult FailedLogout()
        {
            return View("FailedLogout");
        }

        [HttpPost]
        public ActionResult Logout()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (currentUserId != null)
            {
                var currentUser = _context.User.FirstOrDefault(u => u.ID == currentUserId);

                if (currentUser != null)
                {
                    HttpContext.Session.Remove("UserId");
                }
                else
                {
                    return RedirectToAction("FailedLogout", "Users");
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind("UserName,Password")] User loginCredentials)
        {
            var user = _context.User.SingleOrDefault(u => u.UserName.Equals(loginCredentials.UserName) && u.Password.Equals(loginCredentials.Password));
            if (user == null)
            {
                return RedirectToAction("FailedLogin", "Users");
            }

            HttpContext.Session.SetInt32("UserId", user.ID);

            return RedirectToAction("Index", "Home");
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.ID == id);
        }
    }
}
