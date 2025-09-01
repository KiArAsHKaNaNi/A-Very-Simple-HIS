using A_Very_Simple_HIS.Models;
using A_Very_Simple_HIS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace A_Very_Simple_HIS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new List<UserViewModel>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                model.Add(new UserViewModel
                {
                    //Id = u.Id,
                    UserName = u.UserName,
                    FullName = u.FullName,
                    EmailAddress = u.Email,
                    Roles = roles.Select(r => new SelectListItem
                    {
                        Text = r,
                        Value = r
                    }).ToList()
                });
            }

            return View(model);
        }


        public async Task<ActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var model = new UserViewModel
            {
                Roles = roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                }).ToList()
            } ;
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserViewModel userViewModel)
        {
            
            
            if (ModelState.IsValid)
            {
                var newUser = new ApplicationUser
                {
                    UserName = userViewModel.UserName,
                    Email = userViewModel.EmailAddress,
                    FullName = userViewModel.FullName,
                };
                var result = await _userManager.CreateAsync(newUser, userViewModel.Password);
                if (result.Succeeded) 
                {
                    if (userViewModel.SelectedRole != null)
                    {
                        await _userManager.AddToRoleAsync(newUser, userViewModel.SelectedRole);
                    }

                    return RedirectToAction("Index");
                }
            }
            return View();


        }

        public async Task<ActionResult> Edit(string username)
        {
            if (string.IsNullOrEmpty(username))
                return NotFound();
            var roles = await _roleManager.Roles.ToListAsync();
            var user = await _userManager.FindByNameAsync(username);
            var model = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                EmailAddress = user.Email,
                FullName = user.FullName,
                Roles = roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                }).ToList()
            };
            return View(model);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
                userViewModel.Roles = _roleManager.Roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name,
                    Selected = r.Name == userViewModel.SelectedRole
                }).ToList();

            var user = await _userManager.FindByIdAsync(userViewModel.Id);
            if (user == null)
                return NotFound();

            user.UserName = userViewModel.UserName;
            user.Email = userViewModel.EmailAddress;
            user.FullName = userViewModel.FullName;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(userViewModel);
            }

            if (!string.IsNullOrWhiteSpace(userViewModel.Password))
            {
                var removePass = await _userManager.RemovePasswordAsync(user);
                if (!removePass.Succeeded)
                {
                    foreach (var error in removePass.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(userViewModel);
                }

                var addPass = await _userManager.AddPasswordAsync(user, userViewModel.Password);
                if (!addPass.Succeeded)
                {
                    foreach (var error in addPass.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(userViewModel);
                }
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var currentRole = currentRoles.FirstOrDefault();

            if (currentRole != userViewModel.SelectedRole)
            {
                if (currentRole != null)
                {
                    await _userManager.RemoveFromRoleAsync(user, currentRole);
                }

                if (!string.IsNullOrEmpty(userViewModel.SelectedRole))
                {
                    await _userManager.AddToRoleAsync(user, userViewModel.SelectedRole);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
