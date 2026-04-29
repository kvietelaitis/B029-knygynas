using Knygynas.Services;
using Knygynas.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Knygynas.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class WorkerController : Controller
{
    private readonly WorkerService _workerService;

    public WorkerController(WorkerService workerService)
    {
        _workerService = workerService;
    }

    public async Task<IActionResult> Index()
    {
        var workers = await _workerService.GetWorkersAsync();
        return View(workers);
    }


    public IActionResult Create()
    {
        return PartialView("_WorkerForm", new WorkerFormVm());
    }

    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var worker = await _workerService.GetWorkerByIdAsync(id);
        if (worker == null)
        {
            return NotFound();
        }

        var model = new WorkerFormVm
        {
            Id = worker.Id,
            Email = worker.Email ?? ""
        };

        return PartialView("_WorkerForm", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(WorkerFormVm model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_WorkerForm", model);
        }

        IdentityResult result;
        string? generatedPassword = null;
        if (string.IsNullOrWhiteSpace(model.Id))
        {
            var createResult = await _workerService.CreateWorkerAsync(model.Email);
            result = createResult.Result;
            generatedPassword = createResult.GeneratedPassword;
        }
        else
        {
            result = await _workerService.UpdateWorkerAsync(model.Id, model.Email);
        }

        if (!result.Succeeded)
        {
            AddIdentityErrors(result);
            return PartialView("_WorkerForm", model);
        }

        if (!string.IsNullOrWhiteSpace(generatedPassword))
        {
            TempData["WorkerPassword"] = generatedPassword;
            TempData["WorkerEmail"] = model.Email;
        }

        if (IsAjaxRequest())
        {
            return Json(new { redirectUrl = Url.Action(nameof(Index)) });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var success = await _workerService.DeleteWorkerAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    private bool IsAjaxRequest()
    {
        return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
}