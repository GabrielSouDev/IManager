using AutoMapper;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Presentation.ViewModels.Companies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IManager.Web.Presentation.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Company> _companyRepository;

        public CompaniesController(IMapper mapper, IRepository<Company> companyRepository)
        {
            _mapper = mapper;
            _companyRepository = companyRepository;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            var companies = await _companyRepository.GetAllAsync();

            var model = _mapper.Map<IEnumerable<CompanyViewModel>>(companies);
            return View(model);
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _companyRepository.FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<CompanyViewModel>(company);

            return View(model);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCompanyViewModel company)
        {
            if (ModelState.IsValid)
            {
                var entity = _mapper.Map<Company>(company);
                await _companyRepository.AddAsync(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var company = await _companyRepository.GetByIdAsync(id.Value);

            if (company == null) return NotFound();

            var model = _mapper.Map<EditCompanyViewModel>(company);
            return View(model);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditCompanyViewModel company)
        {
            if (id != company.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {

                    var entity = await _companyRepository.GetByIdAsync(id);
                    if (entity is null) return NotFound();

                    _mapper.Map(company, entity);
                    await _companyRepository.UpdateAsync(entity);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CompanyExists(company.Id))
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
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _companyRepository
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<CompanyViewModel>(company);
            return View(model);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company != null)
            {
                await _companyRepository.DeleteAsync(company);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CompanyExists(Guid id)
        {
            return await _companyRepository.ExistsAsync(e => e.Id == id);
        }
    }
}
