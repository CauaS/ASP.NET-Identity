using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SemIdentity.Models;
using SemIdentity.Models.LoginViewModel;
using SemIdentity.Models.ManagerViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemIdentity.Controllers
{
    public class ManagerController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ManagerController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [TempData]
        public string StatusMessage { get; set; }
        
        [HttpGet]
        public async Task<IActionResult> Index() 
        {
            var user = await _userManager.GetUserAsync(User); //User = obj disponibilizado pelo controler base, contem info do user logado
            if(user == null)
            {
                throw new ApplicationException($"Não foi possível carregar o user com o ID '{_userManager.GetUserId(User)}' ");
            }

            //Maperar os dados sobtidos do user para a model
            var model = new IndexViewModel
            {
                Email = user.Email,
                IsEmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(IndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                //pegando os dados do usuário logadp
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Não foi possível carregar o user com o ID '{_userManager.GetUserId(User)}' ");
                }

                var userEmail = user.Email; //email do usuário logado no sistema

                if(userEmail != model.Email)
                {
                    var result = await _userManager.SetEmailAsync(user, model.Email);

                    if (!result.Succeeded)
                    {
                        throw new ApplicationException($"Falha ao alterar o email ao usuário com ID '{user.Id}' ");
                    }
                }

                var userTelefone = user.PhoneNumber; //email do usuário logado no sistema

                if (userTelefone != model.PhoneNumber)
                {
                    var result = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);

                    if (!result.Succeeded)
                    {
                        throw new ApplicationException($"Falha ao alterar o telefone ao usuário com ID '{user.Id}' ");
                    }
                }
                StatusMessage = "Seu perfil foi atualizado.";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Não foi possível carregar o user com o ID '{_userManager.GetUserId(User)}' ");
            }
            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) {
            if (ModelState.IsValid)
            {
                return View(model);
            }
            //pegando os dados do usuário logadp
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Não foi possível carregar o user com o ID '{_userManager.GetUserId(User)}' ");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                foreach(var erro in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, erro.Description);
                }

                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Sua senha foi alterada com sucesso!";

            return RedirectToAction(nameof(ChangePassword));
                
            }
            
        }
    }
