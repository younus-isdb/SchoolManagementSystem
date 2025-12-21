using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.ViewModels;
using Microsoft.AspNetCore.Http; // Add this

namespace SchoolManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly RoleManager<AppRole> _roleManager; // Add this

        // Updated constructor
        public AccountController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            ILogger<AccountController> logger,
            RoleManager<AppRole> roleManager) // Add this parameter
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager; // Initialize
        }

        [AllowAnonymous]
        public IActionResult Login(string ReturnUrl = "/")
        {
            return View(new LoginModel() { ReturnUrl = ReturnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser? user = null;

                if (model.UserNameOrEmail.Contains("@"))
                {
                    user = await _userManager.FindByEmailAsync(model.UserNameOrEmail);
                }

                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(model.UserNameOrEmail);
                }

                if (user == null)
                {
                    ModelState.AddModelError("UserNameOrEmail", "Invalid username or email");
                    return View(model);
                }

                // Check UserType
                if (string.IsNullOrEmpty(user.UserType) ||
                    !string.Equals(user.UserType, model.UserType, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("UserType",
                        $"This account is registered as {user.UserType ?? "Unknown"}, not {model.UserType}. Please select correct user type.");
                    return View(model);
                }

                // Student validation
                if (model.UserType.Equals("Student", StringComparison.OrdinalIgnoreCase))
                {
                    bool hasStudentErrors = false;

                    if (string.IsNullOrEmpty(model.Class))
                    {
                        ModelState.AddModelError("Class", "Class is required for students.");
                        hasStudentErrors = true;
                    }

                    if (string.IsNullOrEmpty(model.Section))
                    {
                        ModelState.AddModelError("Section", "Section is required for students.");
                        hasStudentErrors = true;
                    }

                    if (hasStudentErrors)
                        return View(model);
                }

                // Attempt login
                var signResult = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    model.Password,
                    model.RememberMe,
                    false);

                if (signResult.Succeeded)
                {
                    _logger.LogInformation($"{user.UserName} ({user.UserType}) logged in at {DateTime.Now}");

                    // Store in session
                    HttpContext.Session.SetString("UserType", user.UserType);
                    HttpContext.Session.SetString("UserId", user.Id.ToString());

                    if (user.UserType == "Student")
                    {
                        HttpContext.Session.SetString("Class", user.Class ?? "");
                        HttpContext.Session.SetString("Section", user.Section ?? "");
                    }

                    return RedirectPermanent(model.ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError("Password", "Invalid credentials");
                    return View(model);
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Register(string ReturnUrl = "/")
        {
            return View(new RegisterModel() { ReturnUrl = ReturnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Create user
                var user = new AppUser
                {
                    Id = Guid.NewGuid(),
                    UserName = model.Email,
                    Email = model.Email,
                    UserType = model.UserType,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber
                };

                // Set student fields if applicable
                if (model.UserType == "Student")
                {
                    if (string.IsNullOrEmpty(model.Class) || string.IsNullOrEmpty(model.Section))
                    {
                        ModelState.AddModelError("", "Class and Section are required for students.");
                        return View(model);
                    }

                    user.Class = model.Class;
                    user.Section = model.Section;
                    user.RollNumber = model.RollNumber;
                }

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                  
                    if (!await _roleManager.RoleExistsAsync(model.UserType))
                    {
                        await _roleManager.CreateAsync(new AppRole { Name = model.UserType });
                    }

                   
                    await _userManager.AddToRoleAsync(user, model.UserType);

                    _logger.LogInformation($"New {model.UserType} registered: {model.Email}");

                    TempData["SuccessMessage"] = $"Registration successful! Please login with your credentials.";
                    return RedirectToAction("Login", new { ReturnUrl = model.ReturnUrl });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation($"{HttpContext.User.Identity.Name} logged out at {DateTime.Now}");

            return Redirect("Login");
        }
    }
}