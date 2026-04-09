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

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditAccountViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _accountService.GetDepartmentsHierarchyViewModelAsync(model.CompanyId);
                return View(model);
            }

            var result = await _accountService.EditAccountAsync(id, model);
                
            if (result.Succeeded)
            {
                TempData[ToastMessages.Success] = "Dados atualizados com sucesso!";

                return RedirectToAction("Details", new {id});
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

                            ViewBag.Departments = await _accountService.GetDepartmentsHierarchyViewModelAsync(model.CompanyId);
            return View(model);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
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

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _accountService.DeleteAsync(id);

            if (!result.Succeeded)
            {
                TempData["Error"] = result.Errors.FirstOrDefault()
                    ?? "Erro ao desativar usuário.";

                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Usuário desativado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}