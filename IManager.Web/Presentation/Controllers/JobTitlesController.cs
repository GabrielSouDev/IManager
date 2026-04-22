using IManager.Web.Application.Interfaces;
using IManager.Web.Application.Services;
using IManager.Web.Domain.Consts;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Presentation.ViewModels.Account;
using IManager.Web.Presentation.ViewModels.Departments;
using IManager.Web.Presentation.ViewModels.JobTitles;
using IManager.Web.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IManager.Web.Presentation.Controllers;

[Authorize(Policy = Poly.StaffOrAdmin)]
public class JobTitlesController : Controller
{
    private readonly  IJobTitleService _jobTitleService;
    private readonly ICompanyService _companiesService;
    private readonly IDepartmentService _departmentsService;

    public JobTitlesController(IJobTitleService jobTitleService, ICompanyService companiesService, IDepartmentService departmentsService)
    {
        _jobTitleService = jobTitleService;
        _companiesService = companiesService;
        _departmentsService = departmentsService;
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

    // GET: JobTitles/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null) return NotFound();

        DetailsJobTitleModelView model = await _jobTitleService.GetDetailsModelView(id.Value);
        if (model == null) return NotFound();

        return View(model);
    }

    // GET: JobTitles/Create
    public async Task<IActionResult> Create()
    {
        var model = new CreateJobTitleModelView();

        await AddViewBagHierarchyViewModel(model);
        return View(model);
    }

    // POST: JobTitles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateJobTitleModelView model)
    {
        if (!ModelState.IsValid)
        {
            await AddViewBagHierarchyViewModel(model);
            return View(model);
        }

        Result result = await _jobTitleService.AddJobTitle(model);

        if(!result.Succeeded)
        {
            await AddViewBagHierarchyViewModel(model);
            TempData[ToastMessages.Error] = $"Ocorreu um erro ao realizar o cadastro: {string.Join(", ", result.Errors)}.";
            return View(model);
        }

        TempData[ToastMessages.Success] = "Cargo cadastrado com sucesso!";

        return RedirectToAction(nameof(Index));
    }

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

    #region Helpers
    private async Task AddViewBagHierarchyViewModel(CreateJobTitleModelView model)
    {
        if (User.IsInRole(Role.Staff))
            ViewBag.Companies = await _companiesService.GetCompaniesHierarchyViewModelAsync();
        else
        {
            var companyId = GetCompanyId();
            ViewBag.Departments = await _departmentsService.GetDepartmentsHierarchyViewModelAsync(companyId);
            model.CompanyId = companyId;
        }
    }

    private Guid GetCompanyId() => Guid.Parse(User.FindFirst("CompanyId")!.Value);
    #endregion
}
