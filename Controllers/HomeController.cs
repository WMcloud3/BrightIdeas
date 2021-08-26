using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BrightIdeas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BrightIdeas.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;

        public HomeController(MyContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(user => user.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in User!!!!!");

                    return View("Index");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

                _context.Users.Add(newUser);
                _context.SaveChanges();

                // "Dashboard" or "Index"
                return RedirectToAction("Dashboard");
            }
            // "Dashboard" or "Index"
            return View("Index");
        }

        [HttpPost("checkLogin")]
        public IActionResult CheckLogin(LoginUser login)
        {
            if(ModelState.IsValid)
            {
                //var could be User if it helps readablity
                var userInDb = _context.Users.FirstOrDefault(user => user.Email == login.LoginEmail);

                if(userInDb == null )
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Login!!!!!");

                    return View("Index");
                }
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();

                var result = hasher.VerifyHashedPassword(login, userInDb.Password, login.LoginPassword);

                if(result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Login!!!!!");

                    return View("Index");
                }

                HttpContext.Session.SetInt32("userId", userInDb.UserId);
                Console.WriteLine(HttpContext.Session.GetInt32("userId"));
                return RedirectToAction("Dashboard");
            }

            return View("Index");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? loggedUserId = HttpContext.Session.GetInt32("userId");
            if(loggedUserId == null) return RedirectToAction("Index");

            ViewBag.LoggedUser = _context.Users.FirstOrDefault(user => user.UserId == loggedUserId);
            ViewBag.AllIdeas = _context.Ideas
                .Include(idea => idea.Creator)
                .Include(idea => idea.Likes)
                .ToList();

            return View();
        }

        [HttpPost("submitIdea")]
        public IActionResult SubmitIdea(Idea newIdea)
        {
            int? loggedUserId = HttpContext.Session.GetInt32("userId");
            if(loggedUserId == null) return RedirectToAction("Index");

            if(ModelState.IsValid)
            {
                newIdea.UserId = (int)loggedUserId;
                _context.Add(newIdea);
                _context.SaveChanges();

                return RedirectToAction("Dashboard");
            }
            ViewBag.LoggedUser = _context.Users.FirstOrDefault(user => user.UserId == loggedUserId);
            ViewBag.AllIdeas = _context.Ideas
                .Include(idea => idea.Creator)
                .Include(idea => idea.Likes)
                .ToList();

            return View("Dashboard");
        }

        [HttpGet("idea/{id}")]
        public IActionResult SingleIdea(int id)
        {
            int? loggedUserId = HttpContext.Session.GetInt32("userId");
            if(loggedUserId == null) return RedirectToAction("Index");

            ViewBag.LoggedUser = _context.Users.FirstOrDefault(user => user.UserId == loggedUserId);
            ViewBag.SingleIdea = _context.Ideas
                .Include(idea => idea.Creator)
                .Include(idea => idea.Likes)
                    .ThenInclude(likes => likes.User)
                .FirstOrDefault(idea => idea.IdeaId == id);

            return View();
        }

        [HttpGet("user/{id}")]
        public IActionResult SingleUser(int id)
        {
            int? loggedUserId = HttpContext.Session.GetInt32("userId");
            if(loggedUserId == null) return RedirectToAction("Index");

            ViewBag.LoggedUser = _context.Users.FirstOrDefault(user => user.UserId == loggedUserId);
            ViewBag.SingleUser = _context.Users
                .Include(user => user.CreatedIdeas)
                .Include(user => user.LikesGiven)
                    .ThenInclude(likes => likes.User)
                .FirstOrDefault(user => user.UserId == id);

            return View();
        }

        [HttpGet("idea/{id}/delete")]
        public IActionResult DeleteIdea(int id)
        {
            Idea deleteMe = _context.Ideas
                .FirstOrDefault(d => d.IdeaId == id);

            _context.Ideas.Remove(deleteMe);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [HttpGet("/idea/{id}/like")]
        public IActionResult AddLike(int id)
        {
            int loggedUserId = (int) HttpContext.Session.GetInt32("userId");

            Like like = new Like();

            like.UserId = loggedUserId;
            like.IdeaId = id;

            _context.Likes.Add(like);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
