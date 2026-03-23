using IManager.Web.Data.Persistence;
using IManager.Web.Domain.Entities.TimeTrackings;
using IManager.Web.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IManager.Web.Presentation.Controllers
{
    public class TimeChecksController : Controller
    {
        private readonly IRepository<TimeCheck> _timeCheckRepository;

        public TimeChecksController(IRepository<TimeCheck> timeCheckRepository)
        {
            _timeCheckRepository = timeCheckRepository;
        }


        // GET: TimeChecks
        public async Task<IActionResult> Index()
        {
            var model = _timeCheckRepository.GetAllAsync(q => q.Include(t => t.TimeEntry));
            return View(model);
        }

        // GET: TimeChecks/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var timeCheck = await _timeCheckRepository.GetByIdAsync(id.Value, q => q
                                                        .Include(t => t.TimeEntry));
            if (timeCheck == null) return NotFound();

            return View(timeCheck);
        }

        // GET: TimeChecks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TimeChecks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TimeEntryId,Type,Timestamp,Id,CreatedAt,LastModified")] TimeCheck timeCheck)
        {
            if (ModelState.IsValid)
            {
                await _timeCheckRepository.AddAsync(timeCheck);

                return RedirectToAction(nameof(Index));
            }

            return View(timeCheck);
        }

        // GET: TimeChecks/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var timeCheck = await _timeCheckRepository.GetByIdAsync(id.Value);
            if (timeCheck == null) return NotFound();

            return View(timeCheck);
        }

        // POST: TimeChecks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("TimeEntryId,Type,Timestamp,Id,CreatedAt,LastModified")] TimeCheck timeCheck)
        {
            if (id != timeCheck.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _timeCheckRepository.UpdateAsync(timeCheck);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TimeCheckExistsAsync(timeCheck.Id))
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

            return View(timeCheck);
        }

        // GET: TimeChecks/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeCheck = await _timeCheckRepository.GetByIdAsync(id.Value, q => q.Include(t => t.TimeEntry));
            if (timeCheck == null) return NotFound();

            return View(timeCheck);
        }

        // POST: TimeChecks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var timeCheck = await _timeCheckRepository.GetByIdAsync(id);
            if (timeCheck != null)
                await _timeCheckRepository.DeleteAsync(timeCheck);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TimeCheckExistsAsync(Guid id)
        {
            return await _timeCheckRepository.ExistsAsync(t => t.Id == id);
        }
    }
}
