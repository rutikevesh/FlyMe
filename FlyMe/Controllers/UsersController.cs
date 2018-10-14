using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlyMe.Data;
using FlyMe.Models;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;

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
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager != null) 
               if (!ViewBag.IsManager)

            {
                return Unauthorized();
            }

            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Details/5
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
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserName,Password,FirstName,LastName,Age,Email,IsManager")] User user)
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

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
            UsersController.CheckIfLoginAndManager(this, _context);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("ID,UserName,Password,FirstName,LastName,Age,Email")] User user)
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ModelState.IsValid)
            {
                user.IsManager = false;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
			
            return View();
        }

        public IActionResult Search(string UserName, string FirstName, string LastName)
        {
            var users = _context.User.AsQueryable();
            if (UserName != null) users = users.Where(s => s.UserName.Equals(UserName));
            if (FirstName != null) users = users.Where(s => s.FirstName.StartsWith(FirstName));
            if (LastName != null) users = users.Where(s => s.LastName.EndsWith(LastName));
            var result = users.ToList(); // execute query
            return View(result);
        }

        // GET: Users/Create
        public IActionResult SignUpAsManager()
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUpAsManager([Bind("ID,UserName,Password,FirstName,Age,LastName,Email")] User user)
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

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
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

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
        public async Task<IActionResult> Edit(int id, [Bind("ID,UserName,Password,FirstName,LastName,Age,Email,IsManager")] User user)
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

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
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

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
            UsersController.CheckIfLoginAndManager(this, _context);

            if (ViewBag.IsManager == null || !ViewBag.IsManager)
            {
                return Unauthorized();
            }

            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Login()
        {
            UsersController.CheckIfLoginAndManager(this, _context);

            return View();
        }

        public ActionResult FailedLogin()
        {
            UsersController.CheckIfLoginAndManager(this, _context);
            return View("FailedLogin");
        }

        public ActionResult FailedLogout()
        {
            UsersController.CheckIfLoginAndManager(this, _context);
            return View("FailedLogout");
        }

        public ActionResult Logout()
        {
            UsersController.CheckIfLoginAndManager(this, _context);
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

        /// <summary>
        /// Used to check if there is a logged in user and if he is a manager
        /// </summary>
        public static void CheckIfLoginAndManager(Controller controller, FlyMeContext context)
        {
            int? currentUserId = controller.HttpContext.Session.GetInt32("UserId");

            if (currentUserId != null)
            {
                var currentUser = context.User.FirstOrDefault(u => u.ID == currentUserId);

                if (currentUser != null)
                {
                    controller.ViewBag.IsLogin = true;
                    controller.ViewBag.IsManager = currentUser.IsManager;
                    controller.ViewBag.LoginUserDisplayName = $"{currentUser.FirstName} {currentUser.LastName}";
                }
            }
        }

    }
}
