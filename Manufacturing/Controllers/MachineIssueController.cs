using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class MachineIssueController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True;TrustServerCertificate=True";

        private void LoadDropdown()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                List<SelectListItem> machineList =
                    new List<SelectListItem>();

                SqlCommand machineCmd = new SqlCommand(
                    @"SELECT MachineId, MachineName
                      FROM Machines
                      ORDER BY MachineName",
                    con);

                SqlDataReader dr = machineCmd.ExecuteReader();

                while (dr.Read())
                {
                    machineList.Add(new SelectListItem
                    {
                        Value = dr["MachineId"].ToString(),
                        Text = dr["MachineName"].ToString()
                    });
                }

                dr.Close();

                List<SelectListItem> orderList =
                    new List<SelectListItem>();

                SqlCommand orderCmd = new SqlCommand(
                    @"SELECT ProductionOrderId, OrderNo
                      FROM ProductionOrders
                      ORDER BY OrderNo",
                    con);

                SqlDataReader dr1 = orderCmd.ExecuteReader();

                while (dr1.Read())
                {
                    orderList.Add(new SelectListItem
                    {
                        Value = dr1["ProductionOrderId"].ToString(),
                        Text = dr1["OrderNo"].ToString()
                    });
                }

                dr1.Close();

                ViewBag.MachineList = machineList;
                ViewBag.OrderList = orderList;
            }
        }

        public IActionResult Index()
        {
            List<MachineIssue> list =
                new List<MachineIssue>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MI.*,
                                        M.MachineName,
                                        PO.OrderNo
                                 FROM MachineIssues MI
                                 INNER JOIN Machines M
                                    ON MI.MachineId = M.MachineId
                                 INNER JOIN ProductionOrders PO
                                    ON MI.ProductionOrderId =
                                       PO.ProductionOrderId
                                 ORDER BY MI.IssueId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    MachineIssue issue =
                        new MachineIssue();

                    issue.IssueId =
                        Convert.ToInt32(dr["IssueId"]);

                    issue.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    issue.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    issue.ReportedBy =
                        dr["ReportedBy"].ToString();

                    issue.IssueDate =
                        Convert.ToDateTime(dr["IssueDate"]);

                    issue.IssueType =
                        dr["IssueType"].ToString();

                    issue.ProblemDescription =
                        dr["ProblemDescription"].ToString();

                    issue.Priority =
                        dr["Priority"].ToString();

                    issue.Status =
                        dr["Status"].ToString();

                    issue.ResolvedDate =
                        dr["ResolvedDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["ResolvedDate"]);

                    issue.Remarks =
                        dr["Remarks"].ToString();

                    issue.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    issue.MachineName =
                        dr["MachineName"].ToString();

                    issue.OrderNo =
                        dr["OrderNo"].ToString();

                    list.Add(issue);
                }

                con.Close();
            }

            return View(list);
        }

        public IActionResult Create()
        {
            LoadDropdown();

            return View();
        }

        [HttpPost]
        public IActionResult Create(MachineIssue issue)
        {
            ModelState.Remove("MachineName");
            ModelState.Remove("OrderNo");

            if (!ModelState.IsValid)
            {
                LoadDropdown();

                return View(issue);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO MachineIssues
                (
                    MachineId,
                    ProductionOrderId,
                    ReportedBy,
                    IssueDate,
                    IssueType,
                    ProblemDescription,
                    Priority,
                    Status,
                    ResolvedDate,
                    Remarks
                )
                VALUES
                (
                    @MachineId,
                    @ProductionOrderId,
                    @ReportedBy,
                    @IssueDate,
                    @IssueType,
                    @ProblemDescription,
                    @Priority,
                    @Status,
                    @ResolvedDate,
                    @Remarks
                )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@MachineId",
                    issue.MachineId);

                cmd.Parameters.AddWithValue(
                    "@ProductionOrderId",
                    issue.ProductionOrderId);

                cmd.Parameters.AddWithValue(
                    "@ReportedBy",
                    issue.ReportedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@IssueDate",
                    issue.IssueDate);

                cmd.Parameters.AddWithValue(
                    "@IssueType",
                    issue.IssueType ?? "");

                cmd.Parameters.AddWithValue(
                    "@ProblemDescription",
                    issue.ProblemDescription);

                cmd.Parameters.AddWithValue(
                    "@Priority",
                    issue.Priority ?? "Medium");

                cmd.Parameters.AddWithValue(
                    "@Status",
                    issue.Status ?? "Open");

                cmd.Parameters.AddWithValue(
                    "@ResolvedDate",
                    (object?)issue.ResolvedDate ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    issue.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Machine Issue Recorded Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            LoadDropdown();

            MachineIssue issue =
                new MachineIssue();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    @"SELECT *
                      FROM MachineIssues
                      WHERE IssueId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    issue.IssueId =
                        Convert.ToInt32(dr["IssueId"]);

                    issue.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    issue.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    issue.ReportedBy =
                        dr["ReportedBy"].ToString();

                    issue.IssueDate =
                        Convert.ToDateTime(dr["IssueDate"]);

                    issue.IssueType =
                        dr["IssueType"].ToString();

                    issue.ProblemDescription =
                        dr["ProblemDescription"].ToString();

                    issue.Priority =
                        dr["Priority"].ToString();

                    issue.Status =
                        dr["Status"].ToString();

                    issue.ResolvedDate =
                        dr["ResolvedDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["ResolvedDate"]);

                    issue.Remarks =
                        dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(issue);
        }

        [HttpPost]
        public IActionResult Edit(MachineIssue issue)
        {
            ModelState.Remove("MachineName");
            ModelState.Remove("OrderNo");

            if (!ModelState.IsValid)
            {
                LoadDropdown();

                return View(issue);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE MachineIssues SET
                                    MachineId=@MachineId,
                                    ProductionOrderId=@ProductionOrderId,
                                    ReportedBy=@ReportedBy,
                                    IssueDate=@IssueDate,
                                    IssueType=@IssueType,
                                    ProblemDescription=@ProblemDescription,
                                    Priority=@Priority,
                                    Status=@Status,
                                    ResolvedDate=@ResolvedDate,
                                    Remarks=@Remarks
                                 WHERE IssueId=@IssueId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@IssueId",
                    issue.IssueId);

                cmd.Parameters.AddWithValue(
                    "@MachineId",
                    issue.MachineId);

                cmd.Parameters.AddWithValue(
                    "@ProductionOrderId",
                    issue.ProductionOrderId);

                cmd.Parameters.AddWithValue(
                    "@ReportedBy",
                    issue.ReportedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@IssueDate",
                    issue.IssueDate);

                cmd.Parameters.AddWithValue(
                    "@IssueType",
                    issue.IssueType ?? "");

                cmd.Parameters.AddWithValue(
                    "@ProblemDescription",
                    issue.ProblemDescription);

                cmd.Parameters.AddWithValue(
                    "@Priority",
                    issue.Priority ?? "Medium");

                cmd.Parameters.AddWithValue(
                    "@Status",
                    issue.Status ?? "Open");

                cmd.Parameters.AddWithValue(
                    "@ResolvedDate",
                    (object?)issue.ResolvedDate ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    issue.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Machine Issue Updated Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            MachineIssue issue =
                new MachineIssue();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MI.*,
                                        M.MachineName,
                                        PO.OrderNo
                                 FROM MachineIssues MI
                                 INNER JOIN Machines M
                                    ON MI.MachineId = M.MachineId
                                 INNER JOIN ProductionOrders PO
                                    ON MI.ProductionOrderId =
                                       PO.ProductionOrderId
                                 WHERE MI.IssueId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    issue.IssueId =
                        Convert.ToInt32(dr["IssueId"]);

                    issue.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    issue.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    issue.ReportedBy =
                        dr["ReportedBy"].ToString();

                    issue.IssueDate =
                        Convert.ToDateTime(dr["IssueDate"]);

                    issue.IssueType =
                        dr["IssueType"].ToString();

                    issue.ProblemDescription =
                        dr["ProblemDescription"].ToString();

                    issue.Priority =
                        dr["Priority"].ToString();

                    issue.Status =
                        dr["Status"].ToString();

                    issue.ResolvedDate =
                        dr["ResolvedDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["ResolvedDate"]);

                    issue.Remarks =
                        dr["Remarks"].ToString();

                    issue.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    issue.MachineName =
                        dr["MachineName"].ToString();

                    issue.OrderNo =
                        dr["OrderNo"].ToString();
                }

                con.Close();
            }

            return View(issue);
        }

        //==================== Delete ====================

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    @"DELETE FROM MachineIssues
                      WHERE IssueId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Machine Issue Deleted Successfully.";

            return RedirectToAction("Index");
        }
        public IActionResult Resolve(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE MachineIssues
                                 SET Status='Resolved',
                                     ResolvedDate=CAST(GETDATE() AS DATE)
                                 WHERE IssueId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Machine Issue Resolved Successfully.";

            return RedirectToAction("Index");
        }
    }
}
