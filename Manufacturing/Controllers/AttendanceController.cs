using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class AttendanceController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True;TrustServerCertificate=True";

        private void LoadEmployees()
        {
            List<SelectListItem> employeeList =
                new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT EmployeeId, FullName AS EmployeeName
                                 FROM Employees
                                 ORDER BY FullName";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    employeeList.Add(new SelectListItem
                    {
                        Value = dr["EmployeeId"].ToString(),
                        Text = dr["EmployeeName"].ToString()
                    });
                }

                dr.Close();
            }

            ViewBag.EmployeeList = employeeList;
        }

        public IActionResult Index()
        {
            List<Attendance> list = new List<Attendance>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT A.*,
                                        E.FullName AS EmployeeName
                                 FROM Attendances A
                                 INNER JOIN Employees E
                                    ON A.EmployeeId = E.EmployeeId
                                 ORDER BY A.AttendanceDate DESC,
                                          A.AttendanceId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Attendance attendance = new Attendance();

                    attendance.AttendanceId =
                        Convert.ToInt32(dr["AttendanceId"]);

                    attendance.EmployeeId =
                        Convert.ToInt32(dr["EmployeeId"]);

                    attendance.AttendanceDate =
                        Convert.ToDateTime(dr["AttendanceDate"]);

                    attendance.Status =
                        dr["Status"].ToString();

                    attendance.CheckInTime =
                        dr["CheckInTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["CheckInTime"];

                    attendance.CheckOutTime =
                        dr["CheckOutTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["CheckOutTime"];

                    attendance.Remarks =
                        dr["Remarks"].ToString();

                    attendance.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    attendance.EmployeeName =
                        dr["EmployeeName"].ToString();

                    list.Add(attendance);
                }

                con.Close();
            }

            return View(list);
        }

        public IActionResult Create()
        {
            LoadEmployees();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Attendance attendance)
        {
            ModelState.Remove("EmployeeName");

            if (!ModelState.IsValid)
            {
                LoadEmployees();
                return View(attendance);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO Attendances
                (
                    EmployeeId,
                    AttendanceDate,
                    Status,
                    CheckInTime,
                    CheckOutTime,
                    Remarks
                )
                VALUES
                (
                    @EmployeeId,
                    @AttendanceDate,
                    @Status,
                    @CheckInTime,
                    @CheckOutTime,
                    @Remarks
                )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@EmployeeId",
                    attendance.EmployeeId);

                cmd.Parameters.AddWithValue(
                    "@AttendanceDate",
                    attendance.AttendanceDate);

                cmd.Parameters.AddWithValue(
                    "@Status",
                    attendance.Status ?? "Present");

                cmd.Parameters.AddWithValue(
                    "@CheckInTime",
                    (object?)attendance.CheckInTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@CheckOutTime",
                    (object?)attendance.CheckOutTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    attendance.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Attendance Marked Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            LoadEmployees();

            Attendance attendance = new Attendance();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "SELECT * FROM Attendances WHERE AttendanceId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    attendance.AttendanceId =
                        Convert.ToInt32(dr["AttendanceId"]);

                    attendance.EmployeeId =
                        Convert.ToInt32(dr["EmployeeId"]);

                    attendance.AttendanceDate =
                        Convert.ToDateTime(dr["AttendanceDate"]);

                    attendance.Status =
                        dr["Status"].ToString();

                    attendance.CheckInTime =
                        dr["CheckInTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["CheckInTime"];

                    attendance.CheckOutTime =
                        dr["CheckOutTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["CheckOutTime"];

                    attendance.Remarks =
                        dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(attendance);
        }

        [HttpPost]
        public IActionResult Edit(Attendance attendance)
        {
            ModelState.Remove("EmployeeName");

            if (!ModelState.IsValid)
            {
                LoadEmployees();
                return View(attendance);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE Attendances SET
                                    EmployeeId=@EmployeeId,
                                    AttendanceDate=@AttendanceDate,
                                    Status=@Status,
                                    CheckInTime=@CheckInTime,
                                    CheckOutTime=@CheckOutTime,
                                    Remarks=@Remarks
                                 WHERE AttendanceId=@AttendanceId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@AttendanceId",
                    attendance.AttendanceId);

                cmd.Parameters.AddWithValue(
                    "@EmployeeId",
                    attendance.EmployeeId);

                cmd.Parameters.AddWithValue(
                    "@AttendanceDate",
                    attendance.AttendanceDate);

                cmd.Parameters.AddWithValue(
                    "@Status",
                    attendance.Status ?? "Present");

                cmd.Parameters.AddWithValue(
                    "@CheckInTime",
                    (object?)attendance.CheckInTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@CheckOutTime",
                    (object?)attendance.CheckOutTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    attendance.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Attendance Updated Successfully.";

            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            Attendance attendance = new Attendance();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT A.*,
                                        E.FullName AS EmployeeName
                                 FROM Attendances A
                                 INNER JOIN Employees E
                                    ON A.EmployeeId = E.EmployeeId
                                 WHERE A.AttendanceId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    attendance.AttendanceId =
                        Convert.ToInt32(dr["AttendanceId"]);

                    attendance.EmployeeId =
                        Convert.ToInt32(dr["EmployeeId"]);

                    attendance.AttendanceDate =
                        Convert.ToDateTime(dr["AttendanceDate"]);

                    attendance.Status =
                        dr["Status"].ToString();

                    attendance.CheckInTime =
                        dr["CheckInTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["CheckInTime"];

                    attendance.CheckOutTime =
                        dr["CheckOutTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["CheckOutTime"];

                    attendance.Remarks =
                        dr["Remarks"].ToString();

                    attendance.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    attendance.EmployeeName =
                        dr["EmployeeName"].ToString();
                }

                con.Close();
            }

            return View(attendance);
        }
        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "DELETE FROM Attendances WHERE AttendanceId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Attendance Deleted Successfully.";

            return RedirectToAction("Index");
        }
    }
}
