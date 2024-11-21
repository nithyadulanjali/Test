using Microsoft.AspNetCore.Mvc;
using MSLSystem.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Text.RegularExpressions;

namespace MSLSystem.Controllers
{
    public class RecoverPWController : Controller
    {
        private readonly string _connectionString;

        public RecoverPWController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbContext");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string email, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View("Index");
            }

            // Password length check
            if (newPassword.Length != 8)
            {
                ModelState.AddModelError("", "Password must be exactly 8 characters long.");
                return View("Index");
            }

            // Password complexity check using regex
            var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8}$";
            if (!Regex.IsMatch(newPassword, passwordPattern))
            {
                ModelState.AddModelError("", "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
                return View("Index");
            }

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                var command = new OracleCommand("UPDATE USER_REG SET password = :password, CON_PASSWORD = :CON_PASSWORD WHERE E_MAIL = :email", connection);

                command.Parameters.Add(new OracleParameter("password", newPassword));
                command.Parameters.Add(new OracleParameter("CON_PASSWORD", confirmPassword));
                command.Parameters.Add(new OracleParameter("email", email));

                try
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        // Redirect to Login/Index page on successful password change
                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        ModelState.AddModelError("", "No user found with the provided email address.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while updating the password.");
                }
            }

            return View("Index");
        }
    }
}

