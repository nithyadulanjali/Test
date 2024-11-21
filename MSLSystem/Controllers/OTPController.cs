using Microsoft.AspNetCore.Mvc;
using MSLSystem.Models;
using Oracle.ManagedDataAccess.Client;
using System;

namespace MSLSystem.Controllers
{
    public class OTPController : Controller
    {
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;

        public OTPController(EmailService emailService, IConfiguration configuration)
        {
            _emailService = emailService;
            _configuration = configuration;
        }

        private string GenerateOtp(int length = 6)
        {
            var random = new Random();
            var otp = random.Next(0, (int)Math.Pow(10, length)).ToString($"D{length}");
            return otp;
        }

        public IActionResult SendOtp(string email)
        {
            
            string connectionString = _configuration.GetConnectionString("OracleDbContext");

            try
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string checkQuery = "SELECT COUNT(1) FROM VERIFY_OTP WHERE USER_EMAIL = :USER_EMAIL";
                    using (var command = new OracleCommand(checkQuery, connection))
                    {
                        command.Parameters.Add(new OracleParameter("USER_EMAIL", OracleDbType.Varchar2)).Value = email;
                        int emailExists = Convert.ToInt32(command.ExecuteScalar());

                        if (emailExists == 0)
                        {
                          
                            ViewBag.ErrorMessage = "The email address provided does not match our records.";
                            return View(); 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking email: {ex.Message}");
                return StatusCode(500, "Error processing OTP request.");
            }

         
            string otp = GenerateOtp();
            bool result = _emailService.SendEmail(email, otp);

            if (result)
            {
                
                try
                {
                    using (var connection = new OracleConnection(connectionString))
                    {
                        connection.Open();

                        string updateQuery = "UPDATE VERIFY_OTP SET OTP_NUM = :OTP_NUM WHERE USER_EMAIL = :USER_EMAIL";
                        using (var command = new OracleCommand(updateQuery, connection))
                        {
                            command.Parameters.Add(new OracleParameter("OTP_NUM", OracleDbType.Varchar2)).Value = otp;
                            command.Parameters.Add(new OracleParameter("USER_EMAIL", OracleDbType.Varchar2)).Value = email;

                            command.ExecuteNonQuery();
                            Console.WriteLine("OTP successfully updated in the database.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating OTP: {ex.Message}");
                    return StatusCode(500, "Error processing OTP request.");
                }

                return RedirectToAction("Index", "VerifyOTP");
            }
            else
            {
                return StatusCode(500, "Error sending OTP.");
            }
        }
    }
}
