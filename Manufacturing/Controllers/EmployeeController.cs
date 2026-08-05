using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class EmployeeController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True";

        public IActionResult Index()
        {
            List<Employee> list = new List<Employee>();
            using(SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using(SqlCommand cmd= new SqlCommand(@"SELECT E.*, D.DepartmentName, S.ShiftName
                         FROM Employees E
                         INNER JOIN Department D
                            ON E.DepartmentId = D.DepartmentId
                         INNER JOIN Shifts S
                            ON E.ShiftId = S.ShiftId
                         ORDER BY E.EmployeeId DESC",con))
                {
                    using(SqlDataReader dr= cmd.ExecuteReader())
                    {
                        while(dr.Read())
                        {
                            list.Add(new Employee()
                            {
                                EmployeeId = Convert.ToInt32(dr["EmployeeId"]),
                                EmployeeCode = dr["EmployeeCode"].ToString(),
                                FullName = dr["FullName"].ToString(),
                                Gender = dr["Gender"].ToString(),
                                Mobile = dr["Mobile"].ToString(),
                                Email = dr["Email"].ToString(),
                                DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                                ShiftId = Convert.ToInt32(dr["ShiftId"]),
                                Designation = dr["Designation"].ToString(),
                                JoinDate = dr["JoinDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["JoinDate"]),
                                Salary = Convert.ToDecimal(dr["Salary"]),
                                AttendanceStatus = dr["AttendanceStatus"].ToString(),
                                Status = dr["Status"].ToString(),
                                Address = dr["Address"].ToString(),
                                CreatedDate = Convert.ToDateTime(dr["CreatedDate"]),
                                DepartmentName = dr["DepartmentName"].ToString(),
                                ShiftName = dr["ShiftName"].ToString(),
                            });
                        }
                     }
                }
            }
            return View(list);
        }

        public IActionResult Create()
        {
            LoadDropdown();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdown();
                return View(employee);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand(@"INSERT INTO Employees
                        (EmployeeCode,FullName,Gender,Mobile,Email,
                         DepartmentId,ShiftId,Designation,
                         JoinDate,Salary,AttendanceStatus,
                         Status,Address)
                        VALUES
                        (@EmployeeCode,@FullName,@Gender,@Mobile,@Email,
                         @DepartmentId,@ShiftId,@Designation,
                         @JoinDate,@Salary,@AttendanceStatus,
                         @Status,@Address)",con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeCode", employee.EmployeeCode);
                    cmd.Parameters.AddWithValue("@FullName", employee.FullName);
                    cmd.Parameters.AddWithValue("@Gender", employee.Gender ?? "");
                    cmd.Parameters.AddWithValue("@Mobile", employee.Mobile ?? "");
                    cmd.Parameters.AddWithValue("@Email", employee.Email ?? "");
                    cmd.Parameters.AddWithValue("@DepartmentId", employee.DepartmentId);
                    cmd.Parameters.AddWithValue("@ShiftId", employee.ShiftId);
                    cmd.Parameters.AddWithValue("@Designation", employee.Designation ?? "");
                    cmd.Parameters.AddWithValue("@JoinDate", (object?)employee.JoinDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Salary", employee.Salary);
                    cmd.Parameters.AddWithValue("@AttendanceStatus", employee.AttendanceStatus ?? "Present");
                    cmd.Parameters.AddWithValue("@Status", employee.Status ?? "Active");
                    cmd.Parameters.AddWithValue("@Address", employee.Address ?? "");
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                TempData["Success"] = "Employee Registered Successfully.";
            }
            return RedirectToAction("Index");
        }

      
        public void LoadDropdown()
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                List<SelectListItem> departmentList = new List<SelectListItem>();
                using(SqlCommand cmd= new SqlCommand("SELECT DepartmentId, DepartmentName FROM Department", con))
                {
                    using(SqlDataReader dr= cmd.ExecuteReader())
                    {
                        while(dr.Read())
                        {
                            departmentList.Add(new SelectListItem
                            {
                                Value = dr["DepartmentId"].ToString(),
                                Text = dr["DepartmentName"].ToString(),
                            });
                        }
                    }
                }

                List<SelectListItem> shiftList= new List<SelectListItem>();
                using(SqlCommand cmd= new SqlCommand("SELECT ShiftId, ShiftName FROM Shifts", con))
                {
                    using(SqlDataReader dr1= cmd.ExecuteReader())
                    {
                        while(dr1.Read())
                        {
                            shiftList.Add(new SelectListItem
                            {
                                Value = dr1["ShiftId"].ToString(),
                                Text = dr1["ShiftName"].ToString(),
                            });
                        }
                    }
                }
                ViewBag.DepartmentList = departmentList;
                ViewBag.ShiftList = shiftList;
            }
        }

        public IActionResult Edit(int id)
        {
            LoadDropdown();

            Employee employee = new Employee();
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand("SELECT * FROM Employees WHERE EmployeeId=@EmployeeId",con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", id);
                    con.Open();
                    using(SqlDataReader dr= cmd.ExecuteReader())
                    {
                        if(dr.Read())
                        {
                            employee.EmployeeId = Convert.ToInt32(dr["EmployeeId"]);
                            employee.EmployeeCode = dr["EmployeeCode"].ToString();
                            employee.FullName = dr["FullName"].ToString();
                            employee.Gender = dr["Gender"].ToString();
                            employee.Mobile = dr["Mobile"].ToString();
                            employee.Email = dr["Email"].ToString();
                            employee.DepartmentId = Convert.ToInt32(dr["DepartmentId"]);
                            employee.ShiftId = Convert.ToInt32(dr["ShiftId"]);
                            employee.Designation = dr["Designation"].ToString();
                            employee.JoinDate = dr["JoinDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["JoinDate"]);
                            employee.Salary = Convert.ToDecimal(dr["Salary"]);
                            employee.AttendanceStatus = dr["AttendanceStatus"].ToString();
                            employee.Status = dr["Status"].ToString();
                            employee.Address = dr["Address"].ToString();
                        }
                    }
                }
            }
           return View(employee);
        }

        [HttpPost]
        public IActionResult Edit(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdown();
                return View(employee);
            }
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand(@"UPDATE Employees SET
                        EmployeeCode=@EmployeeCode,
                        FullName=@FullName,
                        Gender=@Gender,
                        Mobile=@Mobile,
                        Email=@Email,
                        DepartmentId=@DepartmentId,
                        ShiftId=@ShiftId,
                        Designation=@Designation,
                        JoinDate=@JoinDate,
                        Salary=@Salary,
                        AttendanceStatus=@AttendanceStatus,
                        Status=@Status,
                        Address=@Address
                        WHERE EmployeeId=@EmployeeId", con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                    cmd.Parameters.AddWithValue("@EmployeeCode", employee.EmployeeCode);
                    cmd.Parameters.AddWithValue("@FullName", employee.FullName);
                    cmd.Parameters.AddWithValue("@Gender", employee.Gender ?? "");
                    cmd.Parameters.AddWithValue("@Mobile", employee.Mobile ?? "");
                    cmd.Parameters.AddWithValue("@Email", employee.Email ?? "");
                    cmd.Parameters.AddWithValue("@DepartmentId", employee.DepartmentId);
                    cmd.Parameters.AddWithValue("@ShiftId", employee.ShiftId);
                    cmd.Parameters.AddWithValue("@Designation", employee.Designation ?? "");
                    cmd.Parameters.AddWithValue("@JoinDate", (object?)employee.JoinDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Salary", employee.Salary);
                    cmd.Parameters.AddWithValue("@AttendanceStatus", employee.AttendanceStatus ?? "Present");
                    cmd.Parameters.AddWithValue("@Status", employee.Status ?? "Active");
                    cmd.Parameters.AddWithValue("@Address", employee.Address ?? "");

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                TempData["success"] = "Employee Update Sucessfully";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete (int id)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand("DELETE FROM Employees WHERE EmployeeId=@EmployeeId ",con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Attendance(int id)
        {
            List<Employee> employee = new List<Employee>();
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand(@"SELECT E.*, D.DepartmentName, S.ShiftName
                         FROM Employees E
                         INNER JOIN Department D
                         ON E.DepartmentId = D.DepartmentId
                         INNER JOIN Shifts S
                         ON E.ShiftId = S.ShiftId
                         ORDER BY E.EmployeeId DESC", con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while(dr.Read())
                        {
                            employee.Add(new Employee() 
                            {
                                EmployeeId = Convert.ToInt32(dr["EmployeeId"]),
                                EmployeeCode = dr["EmployeeCode"].ToString(),
                                FullName = dr["FullName"].ToString(),
                                DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                                ShiftId = Convert.ToInt32(dr["ShiftId"]),
                                DepartmentName = dr["DepartmentName"].ToString(),
                                ShiftName = dr["ShiftName"].ToString(),
                                AttendanceStatus = dr["AttendanceStatus"].ToString(),
                                Status = dr["Status"].ToString(),

                            });
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }
        
    }
}
