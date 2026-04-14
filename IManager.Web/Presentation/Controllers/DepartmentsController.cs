using AutoMapper;
using IManager.Web.Data.Persistence;
using IManager.Web.Domain.Consts;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Presentation.ViewModels.Departments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IManager.Web.Presentation.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IMapper _mapper;

        public DepartmentsController(IRepository<Department> departmentRepository, IRepository<Company> companyRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var model = await _departmentRepository.GetAllAsync(q => q.Include(d => d.Company));
            return View(model);
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentRepository.GetByIdAsync(id.Value, q => q.Include(d => d.Company));
            if (department == null) return NotFound();

            return View(department);
        }

        // GET: Departments/Create
        public async Task<IActionResult> Create()
        {
            var companyId = Guid.Parse(User.FindFirst("CompanyId")!.Value);

            if (User.IsInRole(Role.Staff))
            {
                var companies = await _companyRepository.GetAllAsync();
                ViewBag.Companies = _mapper.Map<IEnumerable<CompanyViewModel>>(companies);
            }
                

            var model = new CreateDepartmentViewModel
            {
                CompanyId = companyId
            };
            return View(model);
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDepartmentViewModel department)
        {
            Console.WriteLine(department.CompanyId);

            var entity = _mapper.Map<Department>(department);
            if (ModelState.IsValid)
            {
                await _departmentRepository.AddAsync(entity);

                return RedirectToAction(nameof(Index));
            }

            return View(entity);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentRepository.GetByIdAsync(id.Value);
            if (department == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = department.CompanyId;
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,CompanyId,Id,CreatedAt,LastModified")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _departmentRepository.UpdateAsync(department);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DepartmentExistsAsync(department.Id))
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
            ViewData["CompanyId"] = department.CompanyId;
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentRepository.GetByIdAsync(id.Value, q => q.Include(d => d.Company));
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department != null)
            {
               await _departmentRepository.SoftDeleteAsync(department);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> DepartmentExistsAsync(Guid id)
        {
            return await _departmentRepository.ExistsAsync(d => d.Id == id);
        }
    }
}
