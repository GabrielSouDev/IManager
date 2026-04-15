using AutoMapper;
using IManager.Web.Application.Interfaces;
using IManager.Web.Application.Services;
using IManager.Web.Data.Persistence;
using IManager.Web.Domain.Consts;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Presentation.ViewModels.Departments;
using IManager.Web.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IManager.Web.Presentation.Controllers
{
    public class DepartmentsController : Controller
    {
        public readonly ICompanyService _companyService;
        public readonly IDepartmentService _departmentService;

        public DepartmentsController(ICompanyService companyService, IDepartmentService departmentService)
        {
            _companyService = companyService;
            _departmentService = departmentService;
        }

        // GET: Departments
        public async Task<IActionResult> Index([FromQuery] string search, [FromQuery] ActiveFilter active = ActiveFilter.Active, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            PagedResult<DepartmentViewModel> model;
            if(User.IsInRole(Role.Staff))
            {
                model = await _departmentService.GetPagedAsync(search, active, page, pageSize);
            }
            else
            {
                var companyId = Guid.Parse(User.FindFirst("CompanyId")!.Value);
                model = await _departmentService.GetPagedAsync(search, active, page, pageSize, companyId);
            }

            return View(model);
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentService.GetViewModelByIdAsync(id.Value);
            if (department == null) return NotFound();

            return View(department);
        }

        // GET: Departments/Create
        public async Task<IActionResult> Create()
        {
            var companyId = Guid.Parse(User.FindFirst("CompanyId")!.Value);

            if (User.IsInRole(Role.Staff))
            {
                var companies = await _companyService.GetCompaniesViewModelsAsync();
                ViewBag.Companies = companies;
            }

            var model = new CreateDepartmentViewModel
            {
                CompanyId = companyId
            };
            return View(model);
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDepartmentViewModel department)
        {
            if (!ModelState.IsValid)
            {
                if (User.IsInRole(Role.Staff))
                {
                    var companies = await _companyService.GetCompaniesViewModelsAsync();
                    ViewBag.Companies = companies;
                }
                return View(department);
            }

            Result result = await _departmentService.AddAsync(department);
            
            if (!result.Succeeded)
            {
                if (User.IsInRole(Role.Staff))
                {
                    var companies = await _companyService.GetCompaniesViewModelsAsync();
                    ViewBag.Companies = companies;
                }
                TempData[ToastMessages.Error] = $"Falha ao cadastrar setor: {string.Join(", ", result.Errors)}.";

                return View(department);
            }

            TempData[ToastMessages.Success] = "Setor criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentService.GetEditViewModelByIdAsync(id.Value);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditDepartmentViewModel department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(department);
            }

            Result result = await _departmentService.UpdateAsync(department);
            if(!result.Succeeded)
            {
                TempData[ToastMessages.Error] = $"Erro ao editar setor: {string.Join(", ", result.Errors)}";
                return View(department);
            }

            TempData[ToastMessages.Success] = "Setor atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        //// GET: Departments/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var department = await _departmentRepository.GetByIdAsync(id.Value, q => q.Include(d => d.Company));
        //    if (department == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(department);
        //}

        //// POST: Departments/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var department = await _departmentRepository.GetByIdAsync(id);
        //    if (department != null)
        //    {
        //       await _departmentRepository.SoftDeleteAsync(department);
        //    }

        //    return RedirectToAction(nameof(Index));
        //}

        //private async Task<bool> DepartmentExistsAsync(Guid id)
        //{
        //    return await _departmentRepository.ExistsAsync(d => d.Id == id);
        //}
    }
}
