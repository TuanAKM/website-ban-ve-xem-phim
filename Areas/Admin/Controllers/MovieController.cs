using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniCinema.Data;
using MiniCinema.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MiniCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovieController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Phims.ToListAsync());
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Phim phim)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Phims.AnyAsync(m => m.MaPhim == phim.MaPhim))
                {
                    ModelState.AddModelError("MaPhim", "Mã phim này đã bị trùng. Vui lòng nhập mã khác!");
                    return View(phim);
                }

                _context.Add(phim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(phim);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var phim = await _context.Phims.FirstOrDefaultAsync(m => m.MaPhim == id);
            if (phim == null) return NotFound();
            return View(phim);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var phim = await _context.Phims.FindAsync(id);
            if (phim == null) return NotFound();
            return View(phim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Phim phim)
        {
            if (id != phim.MaPhim) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phim);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Phims.AnyAsync(m => m.MaPhim == phim.MaPhim)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(phim);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var phim = await _context.Phims.FirstOrDefaultAsync(m => m.MaPhim == id);
            if (phim == null) return NotFound();
            
            // Check in use
            var hasShowtime = await _context.SuatChieus.AnyAsync(s => s.PhimId == id);
            ViewBag.HasShowtime = hasShowtime;
            
            return View(phim);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var phim = await _context.Phims.FindAsync(id);
            if (phim != null)
            {
                _context.Phims.Remove(phim);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
