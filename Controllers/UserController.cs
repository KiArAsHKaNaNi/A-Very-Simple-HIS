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

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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
