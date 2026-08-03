using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;

namespace Manufacturing.Controllers
{
    public class AccountController : Controller
    {
        private readonly string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True";

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            try
            {
                user.Username = (user.Username ?? "").Trim();
                user.Email = (user.Email ?? "").Trim();
                user.FullName = (user.FullName ?? "").Trim();
                user.Password = (user.Password ?? "").Trim();
                user.Mobile = (user.Mobile ?? "").Trim();

                using (SqlConnection con = new SqlConnection(cs))
                {
                    string check = "SELECT COUNT(*) FROM Users WHERE Username=@Username OR Email=@Email";
                    using (SqlCommand checkCmd = new SqlCommand(check, con))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", user.Username);
                        checkCmd.Parameters.AddWithValue("@Email", user.Email);
                        con.Open();

                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            ViewBag.Error = "Username or Email already exists. Please choose a different username or email.";
                            return View(user);
                        }
                    }

                    string query = @"INSERT INTO Users (FullName, Email, Mobile, Username, Password, Role, IsActive)
                                    VALUES (@FullName, @Email, @Mobile, @Username, @Password, @Role, 1)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@FullName", user.FullName);
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        cmd.Parameters.AddWithValue("@Mobile", user.Mobile);
                        cmd.Parameters.AddWithValue("@Username", user.Username);
                        cmd.Parameters.AddWithValue("@Password", user.Password);
                        cmd.Parameters.AddWithValue("@Role", string.IsNullOrEmpty(user.Role) ? "Machine Operator" : user.Role);

                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["Success"] = "Registration Successful! Please login with your credentials.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error registering user: " + ex.Message;
                return View(user);
            }
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            try
            {
                string inputUser = (Username ?? "").Trim();
                string inputPass = (Password ?? "").Trim();

                using (SqlConnection con = new SqlConnection(cs))
                {
                    string query = "SELECT * FROM Users WHERE (Username=@Username OR Email=@Username) AND Password=@Password AND IsActive=1";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", inputUser);
                        cmd.Parameters.AddWithValue("@Password", inputPass);

                        con.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                HttpContext.Session.SetString("UserId", dr["UserId"].ToString() ?? "");
                                HttpContext.Session.SetString("FullName", dr["FullName"] != DBNull.Value ? dr["FullName"].ToString()! : "");
                                HttpContext.Session.SetString("Role", dr["Role"] != DBNull.Value ? dr["Role"].ToString()! : "");
                                HttpContext.Session.SetString("Username", dr["Username"] != DBNull.Value ? dr["Username"].ToString()! : "");
                                HttpContext.Session.SetString("Email", dr["Email"] != DBNull.Value ? dr["Email"].ToString()! : "");

                                return RedirectToAction("Dashboard");
                            }
                        }
                    }
                }

                ViewBag.Error = "Invalid Username or Password, or your account is inactive.";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Connection Error: " + ex.Message;
                return View();
            }
        }

        public IActionResult Dashboard()
        {
            string userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login");
            }

            int userId = int.Parse(userIdStr);
            string sessionRole = HttpContext.Session.GetString("Role") ?? "";
            string sessionFullName = HttpContext.Session.GetString("FullName") ?? "";

            DashboardViewModel model = new DashboardViewModel
            {
                UserId = userId,
                FullName = sessionFullName,
                Role = sessionRole,
                Username = HttpContext.Session.GetString("Username") ?? "",
                Email = HttpContext.Session.GetString("Email") ?? ""
            };

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();

                    string userSql = "SELECT * FROM Users WHERE UserId=@UserId";
                    using (SqlCommand uCmd = new SqlCommand(userSql, con))
                    {
                        uCmd.Parameters.AddWithValue("@UserId", userId);
                        using (SqlDataReader uDr = uCmd.ExecuteReader())
                        {
                            if (uDr.Read())
                            {
                                model.FullName = uDr["FullName"].ToString();
                                model.Email = uDr["Email"] != DBNull.Value ? uDr["Email"].ToString() : "";
                                model.Mobile = uDr["Mobile"] != DBNull.Value ? uDr["Mobile"].ToString() : "";
                                model.Username = uDr["Username"] != DBNull.Value ? uDr["Username"].ToString() : "";
                                model.Role = uDr["Role"].ToString();
                            }
                        }
                    }

                    string totalSql = "SELECT COUNT(*) FROM Users";
                    using (SqlCommand cmd = new SqlCommand(totalSql, con))
                    {
                        model.TotalUsersCount = (int)cmd.ExecuteScalar();
                    }

                    string activeSql = "SELECT COUNT(*) FROM Users WHERE IsActive=1";
                    using (SqlCommand cmd = new SqlCommand(activeSql, con))
                    {
                        model.ActiveUsersCount = (int)cmd.ExecuteScalar();
                    }
                    model.InactiveUsersCount = model.TotalUsersCount - model.ActiveUsersCount;

                    string roleBreakdownSql = @"SELECT Role, COUNT(*) as TotalCount, SUM(CASE WHEN IsActive=1 THEN 1 ELSE 0 END) as ActiveCount 
                                               FROM Users GROUP BY Role";
                    using (SqlCommand cmd = new SqlCommand(roleBreakdownSql, con))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                model.RoleStats.Add(new RoleStat
                                {
                                    RoleName = dr["Role"].ToString(),
                                    UserCount = Convert.ToInt32(dr["TotalCount"]),
                                    ActiveCount = Convert.ToInt32(dr["ActiveCount"])
                                });
                            }
                        }
                    }

                    string userListSql = "SELECT TOP 50 UserId, FullName, Email, Mobile, Username, Role, IsActive FROM Users ORDER BY UserId DESC";
                    using (SqlCommand cmd = new SqlCommand(userListSql, con))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                model.RecentUsers.Add(new User
                                {
                                    UserId = Convert.ToInt32(dr["UserId"]),
                                    FullName = dr["FullName"].ToString(),
                                    Email = dr["Email"] != DBNull.Value ? dr["Email"].ToString() : "",
                                    Mobile = dr["Mobile"] != DBNull.Value ? dr["Mobile"].ToString() : "",
                                    Username = dr["Username"] != DBNull.Value ? dr["Username"].ToString() : "",
                                    Role = dr["Role"].ToString(),
                                    IsActive = Convert.ToBoolean(dr["IsActive"])
                                });
                            }
                        }
                    }

                    PopulateDynamicRoleMetrics(model, con);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Database loading error: " + ex.Message;
            }

            return View(model);
        }

        private void PopulateDynamicRoleMetrics(DashboardViewModel model, SqlConnection con)
        {
            int GetRoleCount(string roleName)
            {
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Role=@Role AND IsActive=1", con))
                {
                    cmd.Parameters.AddWithValue("@Role", roleName);
                    return (int)cmd.ExecuteScalar();
                }
            }

            switch (model.Role)
            {
                case "Admin":
                    model.Metric1Label = "Total Registered Staff";
                    model.Metric1Count = model.TotalUsersCount;
                    model.Metric2Label = "Active System Roles";
                    model.Metric2Count = model.RoleStats.Count;
                    model.Metric3Label = "Active Account Users";
                    model.Metric3Count = model.ActiveUsersCount;
                    model.Metric4Label = "System Health";
                    model.Metric4Count = 100;
                    break;

                case "Production Manager":
                    model.Metric1Label = "Active Machine Operators";
                    model.Metric1Count = GetRoleCount("Machine Operator");
                    model.Metric2Label = "Shift Supervisors";
                    model.Metric2Count = GetRoleCount("Shift Supervisor");
                    model.Metric3Label = "Quality Inspectors";
                    model.Metric3Count = GetRoleCount("Quality Inspector");
                    model.Metric4Label = "Machine Supervisors";
                    model.Metric4Count = GetRoleCount("Machine Supervisor");
                    break;

                case "Inventory Manager":
                    model.Metric1Label = "Store Managers";
                    model.Metric1Count = GetRoleCount("Store Manager");
                    model.Metric2Label = "Inventory Controllers";
                    model.Metric2Count = GetRoleCount("Inventory Manager");
                    model.Metric3Label = "Active Warehouses";
                    model.Metric3Count = model.ActiveUsersCount;
                    model.Metric4Label = "Total Stock Personnel";
                    model.Metric4Count = model.Metric1Count + model.Metric2Count;
                    break;

                case "Machine Supervisor":
                    model.Metric1Label = "Machine Operators Assigned";
                    model.Metric1Count = GetRoleCount("Machine Operator");
                    model.Metric2Label = "Shift Supervisors Active";
                    model.Metric2Count = GetRoleCount("Shift Supervisor");
                    model.Metric3Label = "Active Machine Supervisors";
                    model.Metric3Count = GetRoleCount("Machine Supervisor");
                    model.Metric4Label = "Operational Units";
                    model.Metric4Count = model.ActiveUsersCount;
                    break;

                case "Shift Supervisor":
                    model.Metric1Label = "Active Operators On Duty";
                    model.Metric1Count = GetRoleCount("Machine Operator");
                    model.Metric2Label = "Machine Supervisors";
                    model.Metric2Count = GetRoleCount("Machine Supervisor");
                    model.Metric3Label = "Quality Inspectors";
                    model.Metric3Count = GetRoleCount("Quality Inspector");
                    model.Metric4Label = "Shift Team Count";
                    model.Metric4Count = model.Metric1Count + model.Metric2Count + model.Metric3Count;
                    break;

                case "Machine Operator":
                    model.Metric1Label = "Active Operators";
                    model.Metric1Count = GetRoleCount("Machine Operator");
                    model.Metric2Label = "Machine Supervisors";
                    model.Metric2Count = GetRoleCount("Machine Supervisor");
                    model.Metric3Label = "Quality Inspectors";
                    model.Metric3Count = GetRoleCount("Quality Inspector");
                    model.Metric4Label = "Active Shifts";
                    model.Metric4Count = GetRoleCount("Shift Supervisor");
                    break;

                case "Quality Inspector":
                    model.Metric1Label = "Active Inspectors";
                    model.Metric1Count = GetRoleCount("Quality Inspector");
                    model.Metric2Label = "Operators Monitored";
                    model.Metric2Count = GetRoleCount("Machine Operator");
                    model.Metric3Label = "Supervisors Monitored";
                    model.Metric3Count = GetRoleCount("Machine Supervisor");
                    model.Metric4Label = "Production Managers";
                    model.Metric4Count = GetRoleCount("Production Manager");
                    break;

                case "Store Manager":
                    model.Metric1Label = "Store Managers";
                    model.Metric1Count = GetRoleCount("Store Manager");
                    model.Metric2Label = "Inventory Managers";
                    model.Metric2Count = GetRoleCount("Inventory Manager");
                    model.Metric3Label = "Active Users Directory";
                    model.Metric3Count = model.ActiveUsersCount;
                    model.Metric4Label = "Total Accounts";
                    model.Metric4Count = model.TotalUsersCount;
                    break;

                default:
                    model.Metric1Label = "Total Users";
                    model.Metric1Count = model.TotalUsersCount;
                    model.Metric2Label = "Active Users";
                    model.Metric2Count = model.ActiveUsersCount;
                    model.Metric3Label = "Inactive Users";
                    model.Metric3Count = model.InactiveUsersCount;
                    model.Metric4Label = "Roles Count";
                    model.Metric4Count = model.RoleStats.Count;
                    break;
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
