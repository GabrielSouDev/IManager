using IManager.Web.Data.Persistence;
using IManager.Web.Domain.Entities.Payrolls;
using IManager.Web.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IManager.Web.Presentation.Controllers
{
    public class PayrollsController : Controller
    {
        private readonly IRepository<Payroll> _payrollRepository;

        public PayrollsController(IRepository<Payroll> payrollRepository)
        {
            this._payrollRepository = payrollRepository;
        }

        // GET: Payrolls
        public async Task<IActionResult> Index()
        {
            var model = await _payrollRepository.GetAllAsync(q => q.Include(p=>p.Company));
            return View(model);
        }

        // GET: Payrolls/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var payroll = await _payrollRepository.GetByIdAsync(id.Value, q => q.Include(p => p.Company));

            if (payroll == null) return NotFound();

            return View(payroll);
        }

        // GET: Payrolls/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Payrolls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,PeriodStart,PeriodEnd,Id,CreatedAt,LastModified")] Payroll payroll)
        {
            if (ModelState.IsValid)
            {
                await _payrollRepository.AddAsync(payroll);
                return RedirectToAction(nameof(Index));
            }

            return View(payroll);
        }

        // GET: Payrolls/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var payroll = await _payrollRepository.GetByIdAsync(id.Value);
            if (payroll == null) return NotFound();

            return View(payroll);
        }

        // POST: Payrolls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CompanyId,PeriodStart,PeriodEnd,Id,CreatedAt,LastModified")] Payroll payroll)
        {
            if (id != payroll.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _payrollRepository.UpdateAsync(payroll);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await PayrollExistsAsync(payroll.Id))
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
            return View(payroll);
        }

        // GET: Payrolls/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var payroll = await _payrollRepository.GetByIdAsync(id.Value, q => q.Include(p => p.Company));

            if (payroll == null) return NotFound();

            return View(payroll);
        }

        // POST: Payrolls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var payroll = await _payrollRepository.GetByIdAsync(id);
            if (payroll != null)
            {
                await _payrollRepository.SoftDeleteAsync(payroll);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> PayrollExistsAsync(Guid id)
        {
            return await _payrollRepository.ExistsAsync(p => p.Id == id);
        }
    }
}
