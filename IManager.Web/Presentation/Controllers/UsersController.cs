using IManager.Web.Application.Interfaces;
using IManager.Web.Domain.Consts;
using IManager.Web.Presentation.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;

namespace IManager.Web.Presentation.Controllers
{
    public class UsersController : Controller
    {
        private readonly IAccountService _accountService;

        public UsersController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // GET: Users
        public async Task<IActionResult> Index([FromQuery] string search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            PagedResult<AccountViewModel> model;
            if (User.IsInRole(Role.Staff))
                model = await _accountService.GetAllAccountViewModelAsync(page, pageSize, search: search);
            else
            {
                var companyId = Guid.Parse(User.FindFirst("CompanyId")!.Value);
                model = await _accountService.GetAllAccountViewModelAsync(page, pageSize, companyId, search);
            }

            return View(model);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProfile = await _accountService.GetAccountDetailsViewModelByIdAsync(id.Value);
            if (userProfile == null)
            {
                return NotFound();
            }

            return View(userProfile);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            
            var model = await _accountService.GetEditAccountViewModelByIdAsync(id.Value);

            if (model == null) return NotFound();

            ViewBag.Departments = await _accountService.GetDepartmentsHierarchyViewModelAsync(model.CompanyId);
            return View(model);
        }

        //// POST: Users/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, [Bind("FullName,DocumentNumber,BirthDate,IsActive,DeletedAt,CompanyId,JobTitleId,BaseSalary,Id,CreatedAt,LastModified")] UserProfile userProfile)
        //{
        //    if (id != userProfile.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(userProfile);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!UserProfileExists(userProfile.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "DocumentNumber", userProfile.CompanyId);
        //    ViewData["JobTitleId"] = new SelectList(_context.JobTitles, "Id", "Name", userProfile.JobTitleId);
        //    return View(userProfile);
        //}

        //// GET: Users/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var userProfile = await _context.UserProfiles
        //        .Include(u => u.Company)
        //        .Include(u => u.JobTitle)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (userProfile == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(userProfile);
        //}

        //// POST: Users/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var userProfile = await _context.UserProfiles.FindAsync(id);
        //    if (userProfile != null)
        //    {
        //        _context.UserProfiles.Remove(userProfile);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool UserProfileExists(Guid id)
        //{
        //    return _context.UserProfiles.Any(e => e.Id == id);
        //}
    }
}