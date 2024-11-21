using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using MSLSystem.Models;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using static MSLSystem.Models.Registration;

namespace MSLSystem.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IConfiguration _configuration;

        public RegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var companies = GetCompanies();
                ViewBag.CompanyList = new SelectList(companies, "COMPANY_CODE", "COMPANY_DES");

                return View(new Registration());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading companies: {ex.Message}");
                ModelState.AddModelError("", "There was an error loading the company list.");
                return View(new Registration());
            }
        }

        [HttpPost]
        public IActionResult Index(Registration registration)
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                    }
                }

                try
                {
                    var companies = GetCompanies();
                    ViewBag.CompanyList = new SelectList(companies, "COMPANY_CODE", "COMPANY_DES");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading companies: {ex.Message}");
                    ModelState.AddModelError("", "There was an error loading the company list.");
                }

                return View(registration);
            }

            if (registration.PASSWORD != registration.CON_PASSWORD)
            {
                ModelState.AddModelError("CON_PASSWORD", "The password and confirm password do not match.");

                try
                {
                    var companies = GetCompanies();
                    ViewBag.CompanyList = new SelectList(companies, "COMPANY_CODE", "COMPANY_DES");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading companies: {ex.Message}");
                    ModelState.AddModelError("", "There was an error loading the company list.");
                }

                return View(registration);
            }

            try
            {
                SaveToDatabase(registration);
                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data to Oracle: {ex.Message}");
                ModelState.AddModelError("", "There was an error saving the data.");

                try
                {
                    var companies = GetCompanies();
                    ViewBag.CompanyList = new SelectList(companies, "COMPANY_CODE", "COMPANY_DES");
                }
                catch (Exception innerEx)
                {
                    Console.WriteLine($"Error loading companies: {innerEx.Message}");
                    ModelState.AddModelError("", "There was an error loading the company list.");
                }

                return View(registration);
            }
        }

        private void SaveToDatabase(Registration registration)
        {
            string connectionString = _configuration.GetConnectionString("OracleDbContext");

            try
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO USER_REG (FIRST_NAME, LAST_NAME, DESIGNATION, CONTACT_NO, E_MAIL, PASSWORD, CON_PASSWORD, ADDRESS, GENDER, DOB, COMPANY_DES, COMPANY_CODE) " +
                                   "VALUES (:FIRST_NAME, :LAST_NAME, :DESIGNATION, :CONTACT_NO, :E_MAIL, :PASSWORD, :CON_PASSWORD, :ADDRESS, :GENDER, :DOB, :COMPANY_DES, :COMPANY_CODE)";

                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(new OracleParameter("FIRST_NAME", OracleDbType.Varchar2)).Value = registration.FIRST_NAME ?? (object)DBNull.Value;
                        command.Parameters.Add(new OracleParameter("LAST_NAME", OracleDbType.Varchar2)).Value = registration.LAST_NAME ?? (object)DBNull.Value;
                        command.Parameters.Add(new OracleParameter("DESIGNATION", OracleDbType.Varchar2)).Value = registration.DESIGNATION ?? (object)DBNull.Value;
                        command.Parameters.Add(new OracleParameter("CONTACT_NO", OracleDbType.Varchar2)).Value = registration.CONTACT_NO ?? (object)DBNull.Value;
                        command.Parameters.Add(new OracleParameter("E_MAIL", OracleDbType.Varchar2)).Value = registration.E_MAIL ?? (object)DBNull.Value;
                        command.Parameters.Add(new OracleParameter("PASSWORD", OracleDbType.Varchar2)).Value = registration.PASSWORD ?? (object)DBNull.Value;
                        command.Parameters.Add(new OracleParameter("CON_PASSWORD", OracleDbType.Varchar2)).Value = registration.CON_PASSWORD ?? (object)DBNull.Value;
                        command.Parameters.Add(new OracleParameter("ADDRESS", OracleDbType.Varchar2)).Value = registration.Address ?? (object)DBNull.Value;
                        command.Parameters.Add(new OracleParameter("GENDER", OracleDbType.Varchar2)).Value = registration.GENDER ?? (object)DBNull.Value;
                        command.Parameters.Add(new OracleParameter("DOB", OracleDbType.Varchar2)).Value = registration.DOB ?? (object)DBNull.Value;
                        command.Parameters.Add(new OracleParameter("COMPANY_DES", OracleDbType.Varchar2)).Value = registration.SelectedCompany ?? (object)DBNull.Value;
                        command.Parameters.Add(new OracleParameter("COMPANY_CODE", OracleDbType.Varchar2)).Value = registration.SelectedCompanyCode ?? (object)DBNull.Value;

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
                throw;
            }
        }

        public IActionResult Success()
        {
            return View();
        }

        private List<Company> GetCompanies()
        {
            List<Company> companyList = new List<Company>();
            string connectionString = _configuration.GetConnectionString("OracleDbContext");

            try
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COMP_CODE, NAME FROM COMPANY";

                    using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Company company = new Company
                                {
                                    COMPANY_CODE = reader.IsDBNull(0) ? null : reader.GetString(0),
                                    COMPANY_DES = reader.IsDBNull(1) ? null : reader.GetString(1)
                                };

                                companyList.Add(company);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading companies: {ex.Message}");
                throw;
            }

            return companyList;
        }
    }
}
