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

    // GET: JobTitles/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null) return NotFound();

        var jobTitle = await _jobTitleService.GetEditModelViewByIdAsync(id.Value);

        if (jobTitle == null) return NotFound();

        return View(jobTitle);
    }

    // POST: JobTitles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditJobTitleModelView model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        Result result = await _jobTitleService.UpdateAsync(model);

        if(!result.Succeeded)
        {
            TempData[ToastMessages.Error] = $"Ocorreu um erro ao realizar a atualização: {string.Join(", ", result.Errors)}.";
            return View(model);
        }

        TempData[ToastMessages.Success] = "Cargo atualizado com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    // GET: JobTitles/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null) return NotFound();

        var jobTitle = await _jobTitleService.GetModelViewByIdAsync(id.Value);

        if (jobTitle == null) return NotFound();

        return View(jobTitle);
    }

    // POST: JobTitles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        if (Guid.Empty == id)
            return NotFound();

        Result result = await _jobTitleService.SoftDeleteAsync(id);

        if (!result.Succeeded)
        {
            TempData[ToastMessages.Error] = $"Ocorreu um erro ao realizar a atualização: {string.Join(", ", result.Errors)}.";
            return NotFound();
        }

        TempData[ToastMessages.Success] = "Cargo excluído com sucesso!";
        return RedirectToAction(nameof(Index));
    }

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
