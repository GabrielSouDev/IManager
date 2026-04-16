using IManager.Web.Application.Interfaces;
using IManager.Web.Application.Services;
using IManager.Web.Domain.Consts;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Presentation.ViewModels.Departments;
using IManager.Web.Presentation.ViewModels.JobTitles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IManager.Web.Presentation.Controllers;

[Authorize(Policy = Poly.StaffOrAdmin)]
public class JobTitlesController : Controller
{
    private readonly  IJobTitleService _jobTitleService;

    public JobTitlesController(IJobTitleService jobTitleService)
    {
        _jobTitleService = jobTitleService;
    }

    // GET: JobTitles
    public async Task<IActionResult> Index([FromQuery] string search, [FromQuery] ActiveFilter active = ActiveFilter.Active, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        PagedResult<IndexJobTitleModelView> model;
        if (User.IsInRole(Role.Staff))
        {
            model = await _jobTitleService.GetPagedAsync(search, active, page, pageSize);
        }
        else
        {
            var companyId = Guid.Parse(User.FindFirst("CompanyId")!.Value);
            model = await _jobTitleService.GetPagedAsync(search, active, page, pageSize, companyId);
        }

        return View(model);
    }

    //// GET: JobTitles/Details/5
    //public async Task<IActionResult> Details(Guid? id)
    //{
    //    if (id == null) return NotFound();

    //    var jobTitle = await _JobTitleRepository.GetByIdAsync(id.Value, q => q.Include(d => d.Department));
    //    if (jobTitle == null) return NotFound();

    //    return View(jobTitle);
    //}

    //// GET: JobTitles/Create
    //public IActionResult Create()
    //{
    //    return View();
    //}

    //// POST: JobTitles/Create
    //// To protect from overposting attacks, enable the specific properties you want to bind to.
    //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create([Bind("Name,DepartmentId,IsHazard,IsUnhealthy,IsCommissioned,DailyHours,Id,CreatedAt,LastModified")] JobTitle jobTitle)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        jobTitle.Id = Guid.NewGuid();
    //        await _JobTitleRepository.AddAsync(jobTitle);
    //        return RedirectToAction(nameof(Index));
    //    }

    //    return View(jobTitle);
    //}

    //// GET: JobTitles/Edit/5
    //public async Task<IActionResult> Edit(Guid? id)
    //{
    //    if (id == null) return NotFound();

    //    var jobTitle = await _JobTitleRepository.GetByIdAsync(id.Value);

    //    if (jobTitle == null) return NotFound();

    //    return View(jobTitle);
    //}

    //// POST: JobTitles/Edit/5
    //// To protect from overposting attacks, enable the specific properties you want to bind to.
    //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(Guid id, [Bind("Name,DepartmentId,IsHazard,IsUnhealthy,IsCommissioned,DailyHours,Id,CreatedAt,LastModified")] JobTitle jobTitle)
    //{
    //    if (id != jobTitle.Id)
    //    {
    //        return NotFound();
    //    }

    //    if (ModelState.IsValid)
    //    {
    //        try
    //        {
    //            await _JobTitleRepository.UpdateAsync(jobTitle);
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //            if (!await JobTitleExistsAsync(jobTitle.Id))
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
    //    return View(jobTitle);
    //}

    //// GET: JobTitles/Delete/5
    //public async Task<IActionResult> Delete(Guid? id)
    //{
    //    if (id == null) return NotFound();

    //    var jobTitle = await _JobTitleRepository.GetByIdAsync(id.Value, q => q.Include(j => j.Department));

    //    if (jobTitle == null) return NotFound();

    //    return View(jobTitle);
    //}

    //// POST: JobTitles/Delete/5
    //[HttpPost, ActionName("Delete")]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> DeleteConfirmed(Guid id)
    //{
    //    var jobTitle = await _JobTitleRepository.GetByIdAsync(id);
    //    if (jobTitle != null)
    //        await _JobTitleRepository.SoftDeleteAsync(jobTitle);

    //    return RedirectToAction(nameof(Index));
    //}

    //private async Task<bool> JobTitleExistsAsync(Guid id)
    //{
    //    return await _JobTitleRepository.ExistsAsync(j => j.Id == id);
    //}
}
