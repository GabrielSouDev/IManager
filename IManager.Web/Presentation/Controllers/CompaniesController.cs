using IManager.Web.Application.Interfaces;
using IManager.Web.Domain.Consts;
using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IManager.Web.Presentation.Controllers;

[Authorize(Roles = Role.Staff)]
public class CompaniesController : Controller
{
    private readonly ICompanyService _companyService;

    public CompaniesController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    // GET: Companies
    public async Task<IActionResult> Index([FromQuery] string search, [FromQuery] ActiveFilter active = ActiveFilter.Active, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var model = await _companyService.GetPagedAsync(search, active, page, pageSize);

        return View(model);
    }

    // GET: Companies/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        CompanyViewModel model = await _companyService.GetViewModelByIdAsync(id.Value);
        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    // GET: Companies/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Companies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCompanyViewModel company)
    {
        if (!ModelState.IsValid)
        {
            return View(company);
        }

        Result result = await _companyService.AddAsync(company);

        if (!result.Succeeded){
            TempData[ToastMessages.Error] = $"Erro em criar Empresa: {string.Join(", ",result.Errors)}";
            return View(company);
        }

        TempData[ToastMessages.Success] = "Empresa criada com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    // GET: Companies/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null) return NotFound();

        var model = await _companyService.GetEditViewModelByIdAsync(id.Value);
        if(model == null) return NotFound();

        return View(model);
    }

    // POST: Companies/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditCompanyViewModel company)
    {
        if (id != company.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            return View(company);
        }
        
        var result = await _companyService.UpdateAsync(id, company);
        if (!result.Succeeded)
        {
            TempData[ToastMessages.Error] = $"Erro ao atualizar Empresa: {string.Join(", ", result.Errors)}";
            return View(company);
        }

        TempData[ToastMessages.Success] = "Empresa atualizada com sucesso!";

        return RedirectToAction(nameof(Index));
        
    }

    // GET: Companies/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        CompanyViewModel model = await _companyService.GetViewModelByIdAsync(id.Value);
        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    // POST: Companies/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        if (id == Guid.Empty)
        {
            return NotFound();
        }
        Result result = await _companyService.SoftDeleteAsync(id);
        if(!result.Succeeded)
        {
            TempData[ToastMessages.Error] = $"Erro ao desativar empresa: {string.Join(", ", result.Errors)}";
            return NotFound();
        }

        TempData[ToastMessages.Success] = "Empresa atualizada com sucesso!";
        return RedirectToAction(nameof(Index));
    }
}
