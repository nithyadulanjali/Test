using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Oracle.ManagedDataAccess.Client;
using System;
using MSLSystem.Models;

namespace MSLSystem.Controllers
{
    public class VerifyOTPController : Controller
    {
        private readonly IConfiguration _configuration;

        public VerifyOTPController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyOTP(string email, string enteredOtp)
        {
            string connectionString = _configuration.GetConnectionString("OracleDbContext");

            try
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(1) FROM VERIFY_OTP WHERE USER_EMAIL = :USER_EMAIL AND OTP_NUM = :OTP_NUM";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(new OracleParameter("USER_EMAIL", OracleDbType.Varchar2)).Value = email;
                        command.Parameters.Add(new OracleParameter("OTP_NUM", OracleDbType.Varchar2)).Value = enteredOtp;

                        int matchCount = Convert.ToInt32(command.ExecuteScalar());

                        if (matchCount > 0)
                        {
                           
                            return RedirectToAction("Index", "RecoverPW");
                        }
                        else
                        {
                           
                            ViewBag.ErrorMessage = "Invalid OTP or email. Please try again.";
                            return View("Index");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying OTP: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while verifying OTP. Please try again later.";
                return View("Index");
            }
        }
    }
}
