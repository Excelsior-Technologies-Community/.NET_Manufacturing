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
                string role = HttpContext.Session.GetString("Role") ?? "";
                if (role == "Production Manager")
                {
                    return RedirectToAction("ProductionManagerDashboard");
                }
                if (role == "Shift Supervisor")
                {
                    return RedirectToAction("ShiftSupervisorDashboard");
                }
                if (role == "Machine Manager" || role == "Machine Supervisor")
                {
                    return RedirectToAction("MachineManagerDashboard");
                }
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

                                string role = HttpContext.Session.GetString("Role") ?? "";
                                if (role == "Production Manager")
                                {
                                    return RedirectToAction("ProductionManagerDashboard");
                                }
                                if (role == "Shift Supervisor")
                                {
                                    return RedirectToAction("ShiftSupervisorDashboard");
                                }
                                if (role == "Machine Manager" || role == "Machine Supervisor")
                                {
                                    return RedirectToAction("MachineManagerDashboard");
                                }

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

            string sessionRole = HttpContext.Session.GetString("Role") ?? "";
            if (sessionRole == "Production Manager")
            {
                return RedirectToAction("ProductionManagerDashboard");
            }
            if (sessionRole == "Shift Supervisor")
            {
                return RedirectToAction("ShiftSupervisorDashboard");
            }
            if (sessionRole == "Machine Manager" || sessionRole == "Machine Supervisor")
            {
                return RedirectToAction("MachineManagerDashboard");
            }

            int userId = int.Parse(userIdStr);
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

        public IActionResult ProductionManagerDashboard()
        {
            string userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login");
            }

            ProductionManagerDashboardViewModel model = new ProductionManagerDashboardViewModel();

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();

                    string query = "SELECT * FROM ProductionOrders ORDER BY ProductionOrderId DESC";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            DateTime today = DateTime.Today;
                            while (dr.Read())
                            {
                                ProductionOrder order = new ProductionOrder
                                {
                                    ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]),
                                    OrderNo = dr["OrderNo"] != DBNull.Value ? dr["OrderNo"].ToString()! : "",
                                    ProductName = dr["ProductName"] != DBNull.Value ? dr["ProductName"].ToString()! : "",
                                    Quantity = Convert.ToInt32(dr["Quantity"]),
                                    Unit = dr["Unit"] != DBNull.Value ? dr["Unit"].ToString()! : "",
                                    StartDate = dr["StartDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["StartDate"]),
                                    EndDate = dr["EndDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["EndDate"]),
                                    Priority = dr["Priority"] != DBNull.Value ? dr["Priority"].ToString()! : "",
                                    Status = dr["Status"] != DBNull.Value ? dr["Status"].ToString()! : "Pending",
                                    ApprovedBy = dr["ApprovedBy"] != DBNull.Value ? dr["ApprovedBy"].ToString()! : "",
                                    ApprovedDate = dr["ApprovedDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["ApprovedDate"]),
                                    Remarks = dr["Remarks"] != DBNull.Value ? dr["Remarks"].ToString()! : "",
                                    CreatedBy = dr["CreatedBy"] != DBNull.Value ? dr["CreatedBy"].ToString()! : "",
                                    CreatedDate = dr["CreatedDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dr["CreatedDate"]),
                                    ModifiedDate = dr["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["ModifiedDate"])
                                };

                                model.RecentOrders.Add(order);

                                string statusStr = (order.Status ?? "").Trim();

                                if (statusStr.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                                {
                                    model.PendingOrders.Add(order);
                                }
                                else if (statusStr.Equals("Approved", StringComparison.OrdinalIgnoreCase) || 
                                         statusStr.Equals("In Progress", StringComparison.OrdinalIgnoreCase) ||
                                         statusStr.Equals("Running", StringComparison.OrdinalIgnoreCase))
                                {
                                    model.RunningOrders.Add(order);
                                }
                                else if (statusStr.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                                {
                                    model.CompletedOrders.Add(order);
                                }

                                if (order.CreatedDate.Date == today || 
                                   (order.StartDate.HasValue && order.StartDate.Value.Date <= today && order.EndDate.HasValue && order.EndDate.Value.Date >= today))
                                {
                                    model.TodaysOrders.Add(order);
                                }
                            }
                        }
                    }

                    model.TotalOrders = model.RecentOrders.Count;
                    model.PendingOrdersCount = model.PendingOrders.Count;
                    model.RunningOrdersCount = model.RunningOrders.Count;
                    model.CompletedOrdersCount = model.CompletedOrders.Count;
                    model.TodaysProductionCount = model.TodaysOrders.Count;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error loading production manager dashboard: " + ex.Message;
            }

            return View(model);
        }

        public IActionResult ShiftSupervisorDashboard()
        {
            string userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login");
            }

            ShiftSupervisorDashboardViewModel model = new ShiftSupervisorDashboardViewModel();
            model.SupervisorName = HttpContext.Session.GetString("FullName") ?? "Shift Supervisor";

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();

                    // Load Shifts
                    string shiftSql = "SELECT TOP 10 * FROM Shifts ORDER BY ShiftId DESC";
                    using (SqlCommand cmd = new SqlCommand(shiftSql, con))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                model.Shifts.Add(new Shift
                                {
                                    ShiftId = Convert.ToInt32(dr["ShiftId"]),
                                    ShiftCode = dr["ShiftCode"] != DBNull.Value ? dr["ShiftCode"].ToString()! : "",
                                    ShiftName = dr["ShiftName"] != DBNull.Value ? dr["ShiftName"].ToString()! : "",
                                    StartTime = dr["StartTime"] != DBNull.Value ? TimeSpan.Parse(dr["StartTime"].ToString()!) : TimeSpan.Zero,
                                    EndTime = dr["EndTime"] != DBNull.Value ? TimeSpan.Parse(dr["EndTime"].ToString()!) : TimeSpan.Zero,
                                    TotalHours = dr["TotalHours"] != DBNull.Value ? Convert.ToDecimal(dr["TotalHours"]) : 0,
                                    SupervisorName = dr["SupervisorName"] != DBNull.Value ? dr["SupervisorName"].ToString()! : "",
                                    Status = dr["Status"] != DBNull.Value ? dr["Status"].ToString()! : "Active",
                                    Description = dr["Description"] != DBNull.Value ? dr["Description"].ToString()! : "",
                                    CreatedDate = dr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(dr["CreatedDate"]) : DateTime.Now
                                });
                            }
                        }
                    }

                    // Load Operator Assignments
                    string opSql = @"SELECT TOP 10 OA.*, PO.OrderNo, E.FullName, S.ShiftName
                                     FROM OperatorAssignments OA
                                     LEFT JOIN ProductionOrders PO ON OA.ProductionOrderId = PO.ProductionOrderId
                                     LEFT JOIN Employees E ON OA.EmployeeId = E.EmployeeId
                                     LEFT JOIN Shifts S ON OA.ShiftId = S.ShiftId
                                     ORDER BY OA.OperatorAssignmentId DESC";
                    using (SqlCommand cmd = new SqlCommand(opSql, con))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                model.RecentOperatorAssignments.Add(new OperatorAssignment
                                {
                                    OperatorAssignmentId = Convert.ToInt32(dr["OperatorAssignmentId"]),
                                    ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]),
                                    EmployeeId = Convert.ToInt32(dr["EmployeeId"]),
                                    ShiftId = Convert.ToInt32(dr["ShiftId"]),
                                    AssignedDate = Convert.ToDateTime(dr["AssignedDate"]),
                                    Status = dr["Status"] != DBNull.Value ? dr["Status"].ToString() : "",
                                    Remarks = dr["Remarks"] != DBNull.Value ? dr["Remarks"].ToString() : "",
                                    CreatedDate = Convert.ToDateTime(dr["CreatedDate"]),
                                    OrderNo = dr["OrderNo"] != DBNull.Value ? dr["OrderNo"].ToString() : "",
                                    EmployeeName = dr["FullName"] != DBNull.Value ? dr["FullName"].ToString() : "",
                                    ShiftName = dr["ShiftName"] != DBNull.Value ? dr["ShiftName"].ToString() : ""
                                });
                            }
                        }
                    }

                    // Load Active Production Orders
                    string poSql = "SELECT TOP 10 * FROM ProductionOrders ORDER BY ProductionOrderId DESC";
                    using (SqlCommand cmd = new SqlCommand(poSql, con))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                model.ActiveProductionOrders.Add(new ProductionOrder
                                {
                                    ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]),
                                    OrderNo = dr["OrderNo"] != DBNull.Value ? dr["OrderNo"].ToString()! : "",
                                    ProductName = dr["ProductName"] != DBNull.Value ? dr["ProductName"].ToString()! : "",
                                    Quantity = Convert.ToInt32(dr["Quantity"]),
                                    Unit = dr["Unit"] != DBNull.Value ? dr["Unit"].ToString()! : "",
                                    StartDate = dr["StartDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["StartDate"]),
                                    EndDate = dr["EndDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["EndDate"]),
                                    Priority = dr["Priority"] != DBNull.Value ? dr["Priority"].ToString()! : "",
                                    Status = dr["Status"] != DBNull.Value ? dr["Status"].ToString()! : "Pending",
                                    ApprovedBy = dr["ApprovedBy"] != DBNull.Value ? dr["ApprovedBy"].ToString()! : "",
                                    Remarks = dr["Remarks"] != DBNull.Value ? dr["Remarks"].ToString()! : "",
                                    CreatedBy = dr["CreatedBy"] != DBNull.Value ? dr["CreatedBy"].ToString()! : "",
                                    CreatedDate = dr["CreatedDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dr["CreatedDate"])
                                });
                            }
                        }
                    }

                    // Load Maintenance Requests
                    string maintSql = @"SELECT TOP 10 M.*, Mac.MachineName 
                                        FROM MaintenanceRequests M
                                        LEFT JOIN Machines Mac ON M.MachineId = Mac.MachineId
                                        ORDER BY M.RequestId DESC";
                    using (SqlCommand cmd = new SqlCommand(maintSql, con))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                model.RecentMaintenanceRequests.Add(new MaintenanceRequest
                                {
                                    RequestId = Convert.ToInt32(dr["RequestId"]),
                                    MachineId = Convert.ToInt32(dr["MachineId"]),
                                    RequestDate = Convert.ToDateTime(dr["RequestDate"]),
                                    ProblemDescription = dr["ProblemDescription"] != DBNull.Value ? dr["ProblemDescription"].ToString()! : "",
                                    Priority = dr["Priority"] != DBNull.Value ? dr["Priority"].ToString()! : "",
                                    RequestedBy = dr["RequestedBy"] != DBNull.Value ? dr["RequestedBy"].ToString()! : "",
                                    AssignedTo = dr["AssignedTo"] != DBNull.Value ? dr["AssignedTo"].ToString()! : "",
                                    Status = dr["Status"] != DBNull.Value ? dr["Status"].ToString()! : "",
                                    Remarks = dr["Remarks"] != DBNull.Value ? dr["Remarks"].ToString()! : "",
                                    CreatedDate = Convert.ToDateTime(dr["CreatedDate"]),
                                    MachineName = dr["MachineName"] != DBNull.Value ? dr["MachineName"].ToString()! : ""
                                });
                            }
                        }
                    }

                    // Counts
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Shifts", con))
                    {
                        model.TotalShifts = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Shifts WHERE Status='Active'", con))
                    {
                        model.ActiveShiftsCount = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM OperatorAssignments", con))
                    {
                        model.OperatorAssignmentsCount = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM MaintenanceRequests WHERE Status != 'Completed'", con))
                    {
                        model.PendingMaintenanceCount = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ProductionOrders WHERE Status='In Progress' OR Status='Approved' OR Status='Running'", con))
                    {
                        model.ActiveProductionOrdersCount = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Machines WHERE Status='Active' OR Status='Operational'", con))
                    {
                        model.ActiveMachinesCount = (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error loading Shift Supervisor Dashboard: " + ex.Message;
            }

            return View(model);
        }

        public IActionResult MachineManagerDashboard()
        {
            string userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login");
            }

            MachineManagerDashboardViewModel model = new MachineManagerDashboardViewModel();
            model.ManagerName = HttpContext.Session.GetString("FullName") ?? "Machine Manager";

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();

                    // 1. Machines Directory Summary
                    string macSql = "SELECT TOP 15 * FROM Machines ORDER BY MachineId DESC";
                    using (SqlCommand cmd = new SqlCommand(macSql, con))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                model.Machines.Add(new Machine
                                {
                                    MachineId = Convert.ToInt32(dr["MachineId"]),
                                    MachineCode = dr["MachineCode"] != DBNull.Value ? dr["MachineCode"].ToString()! : "",
                                    MachineName = dr["MachineName"] != DBNull.Value ? dr["MachineName"].ToString()! : "",
                                    MachineType = dr["MachineType"] != DBNull.Value ? dr["MachineType"].ToString()! : "",
                                    Manufacturer = dr["Manufacturer"] != DBNull.Value ? dr["Manufacturer"].ToString()! : "",
                                    Status = dr["Status"] != DBNull.Value ? dr["Status"].ToString()! : "Active",
                                    AllocationStatus = dr["AllocationStatus"] != DBNull.Value ? dr["AllocationStatus"].ToString()! : "",
                                    AllocatedTo = dr["AllocatedTo"] != DBNull.Value ? dr["AllocatedTo"].ToString()! : "",
                                    ApprovedBy = dr["ApprovedBy"] != DBNull.Value ? dr["ApprovedBy"].ToString()! : "",
                                    Remarks = dr["Remarks"] != DBNull.Value ? dr["Remarks"].ToString()! : "",
                                    CreatedDate = dr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(dr["CreatedDate"]) : DateTime.Now
                                });
                            }
                        }
                    }

                    // 2. Machine Assignments
                    string assignSql = @"SELECT TOP 10 MA.*, PO.OrderNo, M.MachineName, E.FullName
                                         FROM MachineAssignments MA
                                         LEFT JOIN ProductionOrders PO ON MA.ProductionOrderId = PO.ProductionOrderId
                                         LEFT JOIN Machines M ON MA.MachineId = M.MachineId
                                         LEFT JOIN Employees E ON MA.EmployeeId = E.EmployeeId
                                         ORDER BY MA.AssignmentId DESC";
                    using (SqlCommand cmd = new SqlCommand(assignSql, con))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                model.RecentAssignments.Add(new MachineAssignment
                                {
                                    AssignmentId = Convert.ToInt32(dr["AssignmentId"]),
                                    ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]),
                                    MachineId = Convert.ToInt32(dr["MachineId"]),
                                    AssignedDate = dr["AssignedDate"] != DBNull.Value ? Convert.ToDateTime(dr["AssignedDate"]) : DateTime.Today,
                                    Status = dr["Status"] != DBNull.Value ? dr["Status"].ToString() : "",
                                    Priority = dr["Priority"] != DBNull.Value ? dr["Priority"].ToString() : "",
                                    Remarks = dr["Remarks"] != DBNull.Value ? dr["Remarks"].ToString() : "",
                                    OrderNo = dr["OrderNo"] != DBNull.Value ? dr["OrderNo"].ToString() : "",
                                    MachineName = dr["MachineName"] != DBNull.Value ? dr["MachineName"].ToString() : "",
                                    EmployeeName = dr["FullName"] != DBNull.Value ? dr["FullName"].ToString() : ""
                                });
                            }
                        }
                    }

                    // 3. Maintenance Requests
                    string maintSql = @"SELECT TOP 10 M.*, Mac.MachineName 
                                        FROM MaintenanceRequests M
                                        LEFT JOIN Machines Mac ON M.MachineId = Mac.MachineId
                                        ORDER BY M.RequestId DESC";
                    using (SqlCommand cmd = new SqlCommand(maintSql, con))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                model.RecentMaintenanceRequests.Add(new MaintenanceRequest
                                {
                                    RequestId = Convert.ToInt32(dr["RequestId"]),
                                    MachineId = Convert.ToInt32(dr["MachineId"]),
                                    RequestDate = Convert.ToDateTime(dr["RequestDate"]),
                                    ProblemDescription = dr["ProblemDescription"] != DBNull.Value ? dr["ProblemDescription"].ToString()! : "",
                                    Priority = dr["Priority"] != DBNull.Value ? dr["Priority"].ToString()! : "",
                                    RequestedBy = dr["RequestedBy"] != DBNull.Value ? dr["RequestedBy"].ToString()! : "",
                                    AssignedTo = dr["AssignedTo"] != DBNull.Value ? dr["AssignedTo"].ToString()! : "",
                                    Status = dr["Status"] != DBNull.Value ? dr["Status"].ToString()! : "",
                                    Remarks = dr["Remarks"] != DBNull.Value ? dr["Remarks"].ToString()! : "",
                                    CreatedDate = Convert.ToDateTime(dr["CreatedDate"]),
                                    MachineName = dr["MachineName"] != DBNull.Value ? dr["MachineName"].ToString()! : ""
                                });
                            }
                        }
                    }

                    // 4. Machine Allocations
                    string allocSql = @"SELECT TOP 10 MA.*, M.MachineName, PO.OrderNo
                                        FROM MachineAllocation MA
                                        LEFT JOIN Machines M ON MA.MachineId = M.MachineId
                                        LEFT JOIN ProductionOrders PO ON MA.ProductionOrderId = PO.ProductionOrderId
                                        ORDER BY MA.AllocationId DESC";
                    using (SqlCommand cmd = new SqlCommand(allocSql, con))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                model.RecentAllocations.Add(new MachineAllocation
                                {
                                    AllocationId = Convert.ToInt32(dr["AllocationId"]),
                                    MachineId = Convert.ToInt32(dr["MachineId"]),
                                    ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]),
                                    AllocationDate = Convert.ToDateTime(dr["AllocationDate"]),
                                    Status = dr["Status"] != DBNull.Value ? dr["Status"].ToString()! : "",
                                    MachineName = dr["MachineName"] != DBNull.Value ? dr["MachineName"].ToString()! : "",
                                    OrderNo = dr["OrderNo"] != DBNull.Value ? dr["OrderNo"].ToString()! : ""
                                });
                            }
                        }
                    }

                    // 5. Machine Availabilities
                    string availSql = @"SELECT TOP 10 MA.*, M.MachineName
                                        FROM MachineAvailability MA
                                        LEFT JOIN Machines M ON MA.MachineId = M.MachineId
                                        ORDER BY MA.AvailabilityId DESC";
                    using (SqlCommand cmd = new SqlCommand(availSql, con))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                model.MachineAvailabilities.Add(new MachineAvailability
                                {
                                    AvailabilityId = Convert.ToInt32(dr["AvailabilityId"]),
                                    MachineId = Convert.ToInt32(dr["MachineId"]),
                                    AvailabilityStatus = dr["AvailabilityStatus"] != DBNull.Value ? dr["AvailabilityStatus"].ToString()! : "",
                                    AvailableFrom = Convert.ToDateTime(dr["AvailableFrom"]),
                                    AvailableTo = dr["AvailableTo"] == DBNull.Value ? null : Convert.ToDateTime(dr["AvailableTo"]),
                                    CurrentStatus = dr["CurrentStatus"] != DBNull.Value ? dr["CurrentStatus"].ToString()! : "",
                                    Remarks = dr["Remarks"] != DBNull.Value ? dr["Remarks"].ToString()! : "",
                                    MachineName = dr["MachineName"] != DBNull.Value ? dr["MachineName"].ToString()! : ""
                                });
                            }
                        }
                    }

                    // Aggregated Counts
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Machines", con))
                    {
                        model.TotalMachinesCount = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Machines WHERE Status='Active' OR Status='Operational'", con))
                    {
                        model.ActiveMachinesCount = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Machines WHERE Status='Under Maintenance' OR Status='Maintenance'", con))
                    {
                        model.MaintenanceMachinesCount = (int)cmd.ExecuteScalar();
                    }
                    model.InactiveMachinesCount = model.TotalMachinesCount - model.ActiveMachinesCount - model.MaintenanceMachinesCount;
                    if (model.InactiveMachinesCount < 0) model.InactiveMachinesCount = 0;

                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM MachineAssignments", con))
                    {
                        model.TotalAssignmentsCount = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM MachineAllocation", con))
                    {
                        model.TotalAllocationsCount = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM MaintenanceRequests WHERE Status='Pending' OR Status='Requested'", con))
                    {
                        model.PendingMaintenanceCount = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM MaintenanceRequests WHERE Status='In Progress' OR Status='Assigned'", con))
                    {
                        model.InProgressMaintenanceCount = (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error loading Machine Manager Dashboard: " + ex.Message;
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
