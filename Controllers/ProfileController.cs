
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public ProfileController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

         var model = new ProfileViewModel
        {
            Email = user.Email,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber
            
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        user.Email = model.Email;
        user.UserName = model.UserName;
        user.PhoneNumber = model.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);
        
        if (result.Succeeded)
        {
             TempData["SuccessMessage"] = "Profile updated successfully.";
             return RedirectToAction("Index");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Failed to update profile.");
        }

        //return View("Index", model);
        return PartialView("_Partial/_UpdatepageParial", model);
    }
    //   // GET: عرض صفحة تغيير كلمة السر
    // public IActionResult ChangePassword()
    // {
    //     return View();
    // }

    [HttpPost]
     public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (result.Succeeded)
        {
            TempData["Success"] = "تم تغيير كلمة السر بنجاح.";
            return RedirectToAction("Index", "Profile");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        //return View(model);
        return PartialView("_Partial/_ChangePasswordPartial", model);
    }


}
