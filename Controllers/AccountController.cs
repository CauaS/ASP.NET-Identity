using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SemIdentity.Models;
using SemIdentity.Models.AccountViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SemIdentity.Models.LoginViewModel;

namespace SemIdentity.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager; //param ApplicationUser representa o usuário Identity na app
        private readonly SignInManager<ApplicationUser> _signInManager;

        //recebe por injeção de dependência
        public AccountController( UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet] //metodo tipo get responsável por pegar as informações da View Register
        [AllowAnonymous] // permite pessoas n autorizadas
        public IActionResult Register(string returnUrl = null)
        {
            ViewData [ "ReturnUrl" ] = returnUrl;
            return View();
        }

        [HttpPost] // recebe os dados do forms
        [AllowAnonymous] //por que a pessoa ainda nao está logada
        [ValidateAntiForgeryToken]
        // Action do tipo public, async, recebe uma task de IActionResult, recebe a model, e recebe a string de returnUrl
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if(result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home"); //actionName, //routeValue
                    }
                }
                foreach (var erro in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, erro.Description); // retorna erros para tela
                }
                
            }

            return View(model);
        }
        
        //action resposável por carregar a view Login, e limpar cookie externo
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;

            // passar o nome do esquema de autentificação
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var resuslt = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (resuslt.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tentativa de Login inválida!");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        private IActionResult RedirectToLocal( string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home ");
            }
        }
    }
}
