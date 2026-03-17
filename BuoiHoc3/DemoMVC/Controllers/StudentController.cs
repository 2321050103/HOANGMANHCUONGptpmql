using DemoMVC.Data;
using DemoMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DemoMVC.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================== READ ==================
        public IActionResult Index()
        {
            var students = _context.Students.ToList();
            return View(students);
        }

        // ================== CREATE ==================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student s)
        {
            if (ModelState.IsValid)
            {
                _context.Students.Add(s);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(s);
        }

        // ================== EDIT ==================
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var student = _context.Students.Find(id);

            if (student == null)
            {
                return View("NotFound");
            }

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Student s)
        {
            if (id != s.Id)
            {
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                _context.Students.Update(s);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(s);
        }

        // ================== DELETE ==================
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var student = _context.Students.Find(id);

            if (student == null)
            {
                return View("NotFound");
            }

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var student = _context.Students.Find(id);

            if (student == null)
            {
                return View("NotFound");
            }

            _context.Students.Remove(student);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}