using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OdinaryLevel.Data;
using OdinaryLevel.Models;

namespace OdinaryLevel.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var subject = await _context.Subjects
                .Include(s => s.Topics)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Check if user is admin
            bool isAdmin = User.IsInRole("Admin");
            ViewBag.IsAdmin = isAdmin;

            return View(subject);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create(int subjectId)
        {
            ViewBag.SubjectId = subjectId;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Topic topic)
        {
            if (ModelState.IsValid)
            {
                _context.Topics.Add(topic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = topic.SubjectId });
            }
            return View(topic);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound();
            }
            return View(topic);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Topic topic)
        {
            if (id != topic.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(topic);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = topic.SubjectId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TopicExists(topic.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View(topic);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound();
            }
            return View(topic);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = topic.SubjectId });
        }

        private bool TopicExists(int id)
        {
            return _context.Topics.Any(e => e.Id == id);
        }

        // Add this method to the SubjectsController class
        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Topics)
                .ToListAsync();

            bool isAdmin = User.IsInRole("Admin");
            ViewBag.IsAdmin = isAdmin;

            return View(subjects);
        }
    }
}