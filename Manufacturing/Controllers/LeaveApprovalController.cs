using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class LeaveApprovalController : Controller
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
            List<LeaveRequest> list =
                new List<LeaveRequest>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT LR.*,
                                        E.FullName AS EmployeeName
                                 FROM LeaveRequests LR
                                 INNER JOIN Employees E
                                    ON LR.EmployeeId = E.EmployeeId
                                 ORDER BY LR.LeaveRequestId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    LeaveRequest leave = new LeaveRequest();

                    leave.LeaveRequestId =
                        Convert.ToInt32(dr["LeaveRequestId"]);

                    leave.EmployeeId =
                        Convert.ToInt32(dr["EmployeeId"]);

                    leave.LeaveType =
                        dr["LeaveType"].ToString();

                    leave.FromDate =
                        Convert.ToDateTime(dr["FromDate"]);

                    leave.ToDate =
                        Convert.ToDateTime(dr["ToDate"]);

                    leave.TotalDays =
                        Convert.ToInt32(dr["TotalDays"]);

                    leave.Reason =
                        dr["Reason"].ToString();

                    leave.RequestedDate =
                        Convert.ToDateTime(dr["RequestedDate"]);

                    leave.ApprovedBy =
                        dr["ApprovedBy"].ToString();

                    leave.ApprovalDate =
                        dr["ApprovalDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["ApprovalDate"]);

                    leave.Status =
                        dr["Status"].ToString();

                    leave.Remarks =
                        dr["Remarks"].ToString();

                    leave.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    leave.EmployeeName =
                        dr["EmployeeName"].ToString();

                    list.Add(leave);
                }

                con.Close();
            }

            return View(list);
        }

        //==================== Create GET ====================

        public IActionResult Create()
        {
            LoadEmployees();

            return View();
        }

        [HttpPost]
        public IActionResult Create(LeaveRequest leave)
        {
            ModelState.Remove("EmployeeName");

            if (!ModelState.IsValid)
            {
                LoadEmployees();
                return View(leave);
            }

            leave.TotalDays =
                (leave.ToDate - leave.FromDate).Days + 1;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO LeaveRequests
                (
                    EmployeeId,
                    LeaveType,
                    FromDate,
                    ToDate,
                    TotalDays,
                    Reason,
                    RequestedDate,
                    ApprovedBy,
                    ApprovalDate,
                    Status,
                    Remarks
                )
                VALUES
                (
                    @EmployeeId,
                    @LeaveType,
                    @FromDate,
                    @ToDate,
                    @TotalDays,
                    @Reason,
                    @RequestedDate,
                    @ApprovedBy,
                    @ApprovalDate,
                    @Status,
                    @Remarks
                )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@EmployeeId", leave.EmployeeId);

                cmd.Parameters.AddWithValue(
                    "@LeaveType", leave.LeaveType);

                cmd.Parameters.AddWithValue(
                    "@FromDate", leave.FromDate);

                cmd.Parameters.AddWithValue(
                    "@ToDate", leave.ToDate);

                cmd.Parameters.AddWithValue(
                    "@TotalDays", leave.TotalDays);

                cmd.Parameters.AddWithValue(
                    "@Reason", leave.Reason ?? "");

                cmd.Parameters.AddWithValue(
                    "@RequestedDate", leave.RequestedDate);

                cmd.Parameters.AddWithValue(
                    "@ApprovedBy", leave.ApprovedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@ApprovalDate",
                    (object?)leave.ApprovalDate ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@Status", leave.Status ?? "Pending");

                cmd.Parameters.AddWithValue(
                    "@Remarks", leave.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Leave Request Added Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Approve(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE LeaveRequests
                                 SET Status='Approved',
                                     ApprovalDate=GETDATE()
                                 WHERE LeaveRequestId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Leave Approved Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Reject(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE LeaveRequests
                                 SET Status='Rejected',
                                     ApprovalDate=GETDATE()
                                 WHERE LeaveRequestId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Leave Rejected Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            LoadEmployees();

            LeaveRequest leave =
                new LeaveRequest();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "SELECT * FROM LeaveRequests WHERE LeaveRequestId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    leave.LeaveRequestId =
                        Convert.ToInt32(dr["LeaveRequestId"]);

                    leave.EmployeeId =
                        Convert.ToInt32(dr["EmployeeId"]);

                    leave.LeaveType =
                        dr["LeaveType"].ToString();

                    leave.FromDate =
                        Convert.ToDateTime(dr["FromDate"]);

                    leave.ToDate =
                        Convert.ToDateTime(dr["ToDate"]);

                    leave.TotalDays =
                        Convert.ToInt32(dr["TotalDays"]);

                    leave.Reason =
                        dr["Reason"].ToString();

                    leave.RequestedDate =
                        Convert.ToDateTime(dr["RequestedDate"]);

                    leave.ApprovedBy =
                        dr["ApprovedBy"].ToString();

                    leave.ApprovalDate =
                        dr["ApprovalDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["ApprovalDate"]);

                    leave.Status =
                        dr["Status"].ToString();

                    leave.Remarks =
                        dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(leave);
        }

        [HttpPost]
        public IActionResult Edit(LeaveRequest leave)
        {
            ModelState.Remove("EmployeeName");

            if (!ModelState.IsValid)
            {
                LoadEmployees();
                return View(leave);
            }

            leave.TotalDays =
                (leave.ToDate - leave.FromDate).Days + 1;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE LeaveRequests SET
                                    EmployeeId=@EmployeeId,
                                    LeaveType=@LeaveType,
                                    FromDate=@FromDate,
                                    ToDate=@ToDate,
                                    TotalDays=@TotalDays,
                                    Reason=@Reason,
                                    RequestedDate=@RequestedDate,
                                    ApprovedBy=@ApprovedBy,
                                    ApprovalDate=@ApprovalDate,
                                    Status=@Status,
                                    Remarks=@Remarks
                                 WHERE LeaveRequestId=@LeaveRequestId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@LeaveRequestId",
                    leave.LeaveRequestId);

                cmd.Parameters.AddWithValue(
                    "@EmployeeId",
                    leave.EmployeeId);

                cmd.Parameters.AddWithValue(
                    "@LeaveType",
                    leave.LeaveType);

                cmd.Parameters.AddWithValue(
                    "@FromDate",
                    leave.FromDate);

                cmd.Parameters.AddWithValue(
                    "@ToDate",
                    leave.ToDate);

                cmd.Parameters.AddWithValue(
                    "@TotalDays",
                    leave.TotalDays);

                cmd.Parameters.AddWithValue(
                    "@Reason",
                    leave.Reason ?? "");

                cmd.Parameters.AddWithValue(
                    "@RequestedDate",
                    leave.RequestedDate);

                cmd.Parameters.AddWithValue(
                    "@ApprovedBy",
                    leave.ApprovedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@ApprovalDate",
                    (object?)leave.ApprovalDate ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@Status",
                    leave.Status ?? "Pending");

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    leave.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Leave Request Updated Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            LeaveRequest leave =
                new LeaveRequest();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT LR.*,
                                        E.FullName AS EmployeeName
                                 FROM LeaveRequests LR
                                 INNER JOIN Employees E
                                    ON LR.EmployeeId = E.EmployeeId
                                 WHERE LR.LeaveRequestId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    leave.LeaveRequestId =
                        Convert.ToInt32(dr["LeaveRequestId"]);

                    leave.EmployeeId =
                        Convert.ToInt32(dr["EmployeeId"]);

                    leave.LeaveType =
                        dr["LeaveType"].ToString();

                    leave.FromDate =
                        Convert.ToDateTime(dr["FromDate"]);

                    leave.ToDate =
                        Convert.ToDateTime(dr["ToDate"]);

                    leave.TotalDays =
                        Convert.ToInt32(dr["TotalDays"]);

                    leave.Reason =
                        dr["Reason"].ToString();

                    leave.RequestedDate =
                        Convert.ToDateTime(dr["RequestedDate"]);

                    leave.ApprovedBy =
                        dr["ApprovedBy"].ToString();

                    leave.ApprovalDate =
                        dr["ApprovalDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["ApprovalDate"]);

                    leave.Status =
                        dr["Status"].ToString();

                    leave.Remarks =
                        dr["Remarks"].ToString();

                    leave.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    leave.EmployeeName =
                        dr["EmployeeName"].ToString();
                }

                con.Close();
            }

            return View(leave);
        }

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "DELETE FROM LeaveRequests WHERE LeaveRequestId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Leave Request Deleted Successfully.";

            return RedirectToAction("Index");
        }
    }
}
