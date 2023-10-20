using DataAggregator.Web.App_Start;
using DataAggregator.Web.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers
{
    [Authorize(Roles = "Admin, UserManager")]
    public sealed class UsersAdminController : BaseController
    {
        public UsersAdminController()
        {
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public async Task<ActionResult> Index()
        {
            List<ApplicationUser> users = await UserManager.Users.OrderBy(u => u.Surname).ToListAsync();
            Dictionary<long, Department> departments = GetAllDepartments().ToDictionary(s => s.Id);
            //var roles=RoleManager.Roles.ToList();

            List<ViewUserViewModel> vm = users.Where(w=>w.LockDate==null).Select(s => new ViewUserViewModel
            {
                Id = s.Id,
                UserName = s.UserName,
                FirstName = s.FirstName,
                Surname = s.Surname,
                MiddleName = s.MiddleName,
                LockDate = s.LockDate,
                DepartmentName = s.DepartmentId.HasValue && departments.ContainsKey(s.DepartmentId.Value) ? departments[s.DepartmentId.Value].Name : string.Empty,
                Email = s.Email,
                Roles = s.Roles.Select(sr => sr.Role.Name).ToList()
                //s.Roles.Select(r=> roles.Where(ar=>ar.Id== r.RoleId).Select(sar=>sar.Name).Single()).ToList()//await UserManager.GetRolesAsync(s.Id)
            }).ToList();

            return View(vm);
        }

        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ApplicationUser user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            Department department = null;

            if(user.DepartmentId.HasValue)
                using (var context = new ApplicationDbContext())
                    department = context.Departments.FirstOrDefault(s => s.Id == user.DepartmentId.Value);

            var vm = new ViewUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                Surname = user.Surname,
                MiddleName = user.MiddleName,
                LockDate = user.LockDate,
                DepartmentName = department != null ? department.Name : string.Empty,
                Email = user.Email,
                Roles= ViewBag.RoleNames
            };

            return View(vm);
        }

        public async Task<ActionResult> Create()
        {
            var roles = await RoleManager.Roles.ToListAsync();
            ViewBag.RolesList = roles.Select(x => new AspNetRolesSelected
            {
                Name = x.Name,
                Category = x.Category,
                Description = x.Description
            }).ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = userViewModel.Name,
                    Email = userViewModel.Email,
                    Surname = userViewModel.Surname,
                    FirstName = userViewModel.FirstName,
                    MiddleName = userViewModel.MiddleName
                };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles.Except(new[] { "Admin", "UserManager" }).ToArray<string>());
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();

                }
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ApplicationUser user = await UserManager.FindByIdAsync(id);
            if (user == null)
                return HttpNotFound();

            EditUserViewModel vm = await CreateEditUserViewModelAsync(user);

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel editUser, params string[] selectedRole)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(editUser.Id);
            if (user == null)
                return HttpNotFound();
            
            EditUserViewModel rethrownModel = await CreateEditUserViewModelAsync(user);

            if (!ModelState.IsValid)
                return View(rethrownModel);

            user.Email = editUser.Email;
            user.Surname = editUser.Surname;
            user.FirstName = editUser.FirstName;
            user.MiddleName = editUser.MiddleName;
            user.MultipleAuthentication = editUser.MultipleAuthentication;
            user.DepartmentId = editUser.DepartmentId;

            DateTime parsedLockDate;
            if (editUser.Locked && DateTime.TryParseExact(editUser.LockDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedLockDate))
                user.LockDate = parsedLockDate;
            else
                user.LockDate = null;

            if (!string.IsNullOrEmpty(editUser.Password))
            {
                await UserManager.RemovePasswordAsync(user.Id);
                var changePasswordResult = await UserManager.AddPasswordAsync(user.Id, editUser.Password);
                if (!changePasswordResult.Succeeded)
                {
                    ModelState.AddModelError("", changePasswordResult.Errors.First());
                    return View(rethrownModel);
                }
            }

            selectedRole = selectedRole ?? new string[] { };

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Errors.First());
                return View(rethrownModel);
            }

            result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).Except(new []{"Admin", "UserManager"}).ToArray<string>());
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Errors.First());
                return View(rethrownModel);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetAllUsers()
        {
            var users = UserManager.Users
                .Select(u =>
                    new
                    {
                        u.Id,
                        Value = u.Surname + " " + u.FirstName + " " + u.MiddleName
                    }
                ).ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        private async Task<EditUserViewModel> CreateEditUserViewModelAsync(ApplicationUser user)
        {
            IList<string> userRoles = await UserManager.GetRolesAsync(user.Id);
            List<Department> departments = GetAllDepartments();
            var vm = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Surname = user.Surname,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                Locked = user.LockDate != null,
                LockDate = user.LockDate != null ? ((DateTime)user.LockDate).ToString("dd.MM.yyyy") : "",
                MultipleAuthentication = user.MultipleAuthentication,
                DepartmentId = user.DepartmentId,
                Departments = new SelectList(departments, "Id", "Name"),
                RolesList = RoleManager.Roles.ToList().Select(x => new AspNetRolesSelected
                {
                    Selected = userRoles.Contains(x.Name),
                    Name = x.Name,
                    Category = x.Category,
                    Description = x.Description
                })
            };

            return vm;
        }

        private static List<Department> GetAllDepartments()
        {
            using (var context = new ApplicationDbContext())
                return context.Departments.ToList();
        }
    }
}