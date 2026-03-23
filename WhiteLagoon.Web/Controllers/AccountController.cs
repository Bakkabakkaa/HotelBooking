using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers;

public class AccountController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
    {
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        LoginVM loginVm = new LoginVM()
        {
            RedirectUrl = returnUrl
        };
        
        return View(loginVm);
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).Wait();
        }

        RegisterVM registerVm = new()
        {
            RoleList = _roleManager.Roles.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Name
            })
        };
        
        return View(registerVm);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM registerVm)
    {
        ApplicationUser user = new()
        {
            Name = registerVm.Name,
            Email = registerVm.Email,
            PhoneNumber = registerVm.PhoneNumber,
            NormalizedEmail = registerVm.Email.ToUpper(),
            EmailConfirmed = true,
            UserName = registerVm.Email,
            CreatedAt = DateTime.Now
        };

        var result = await _userManager.CreateAsync(user, registerVm.Password);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(registerVm.Role))
            {
                await _userManager.AddToRoleAsync(user, registerVm.Role);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, SD.Role_Customer);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            if (string.IsNullOrEmpty(registerVm.RedirectUrl))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return LocalRedirect(registerVm.RedirectUrl);
            }
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        registerVm.RoleList = _roleManager.Roles.Select(x => new SelectListItem()
        {
            Text = x.Name,
            Value = x.Name
        });

        return View(registerVm);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM loginVM)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager
                .PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (string.IsNullOrEmpty(loginVM.RedirectUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return LocalRedirect(loginVM.RedirectUrl);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
            }
        }
        
        return View(loginVM);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}