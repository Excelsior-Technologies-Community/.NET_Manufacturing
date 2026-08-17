using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class ProductionWorkStatusController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True;TrustServerCertificate=True";

        private void LoadDropdown()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

               List<SelectListItem> orderList =
                    new List<SelectListItem>();

                SqlCommand orderCmd = new SqlCommand(
                    "SELECT ProductionOrderId, OrderNo FROM ProductionOrders ORDER BY OrderNo",
                    con);

                SqlDataReader dr = orderCmd.ExecuteReader();

                while (dr.Read())
                {
                    orderList.Add(new SelectListItem
                    {
                        Value = dr["ProductionOrderId"].ToString(),
                        Text = dr["OrderNo"].ToString()
                    });
                }

                dr.Close();

                List<SelectListItem> machineList =
                    new List<SelectListItem>();

                SqlCommand machineCmd = new SqlCommand(
                    "SELECT MachineId, MachineName FROM Machines ORDER BY MachineName",
                    con);

                SqlDataReader dr1 = machineCmd.ExecuteReader();

                while (dr1.Read())
                {
                    machineList.Add(new SelectListItem
                    {
                        Value = dr1["MachineId"].ToString(),
                        Text = dr1["MachineName"].ToString()
                    });
                }

                dr1.Close();

                List<SelectListItem> employeeList =
                    new List<SelectListItem>();

                SqlCommand employeeCmd = new SqlCommand(
                    "SELECT EmployeeId, FullName AS EmployeeName FROM Employees ORDER BY FullName",
                    con);

                SqlDataReader dr2 = employeeCmd.ExecuteReader();

                while (dr2.Read())
                {
                    employeeList.Add(new SelectListItem
                    {
                        Value = dr2["EmployeeId"].ToString(),
                        Text = dr2["EmployeeName"].ToString()
                    });
                }

                dr2.Close();

                ViewBag.OrderList = orderList;
                ViewBag.MachineList = machineList;
                ViewBag.EmployeeList = employeeList;
            }
        }

        public IActionResult Index()
        {
            List<ProductionWorkStatus> list =
                new List<ProductionWorkStatus>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT PWS.*,
                                        PO.OrderNo,
                                        M.MachineName,
                                        E.FullName AS EmployeeName
                                 FROM ProductionWorkStatus PWS
                                 INNER JOIN ProductionOrders PO
                                    ON PWS.ProductionOrderId =
                                       PO.ProductionOrderId
                                 INNER JOIN Machines M
                                    ON PWS.MachineId = M.MachineId
                                 INNER JOIN Employees E
                                    ON PWS.EmployeeId = E.EmployeeId
                                 ORDER BY PWS.WorkStatusId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    ProductionWorkStatus work =
                        new ProductionWorkStatus();

                    work.WorkStatusId =
                        Convert.ToInt32(dr["WorkStatusId"]);

                    work.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    work.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    work.EmployeeId =
                        Convert.ToInt32(dr["EmployeeId"]);

                    work.StartDate =
                        dr["StartDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["StartDate"]);

                    work.StartTime =
                        dr["StartTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["StartTime"];

                    work.PauseDate =
                        dr["PauseDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["PauseDate"]);

                    work.PauseTime =
                        dr["PauseTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["PauseTime"];

                    work.ResumeDate =
                        dr["ResumeDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["ResumeDate"]);

                    work.ResumeTime =
                        dr["ResumeTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["ResumeTime"];

                    work.CompletionDate =
                        dr["CompletionDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["CompletionDate"]);

                    work.CompletionTime =
                        dr["CompletionTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["CompletionTime"];

                    work.Status =
                        dr["Status"].ToString();

                    work.Remarks =
                        dr["Remarks"].ToString();
             
                    work.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    work.OrderNo =
                        dr["OrderNo"].ToString();

                    work.MachineName =
                        dr["MachineName"].ToString();

                    work.EmployeeName =
                        dr["EmployeeName"].ToString();

                    list.Add(work);
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
        public IActionResult Create(ProductionWorkStatus work)
        {
            ModelState.Remove("OrderNo");
            ModelState.Remove("MachineName");
            ModelState.Remove("EmployeeName");

            if (!ModelState.IsValid)
            {
                LoadDropdown();
                return View(work);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO ProductionWorkStatus
                (
                    ProductionOrderId,
                    MachineId,
                    EmployeeId,
                    StartDate,
                    StartTime,
                    PauseDate,
                    PauseTime,
                    ResumeDate,
                    ResumeTime,
                    CompletionDate,
                    CompletionTime,
                    Status,
                    Remarks
                )
                VALUES
                (
                    @ProductionOrderId,
                    @MachineId,
                    @EmployeeId,
                    @StartDate,
                    @StartTime,
                    @PauseDate,
                    @PauseTime,
                    @ResumeDate,
                    @ResumeTime,
                    @CompletionDate,
                    @CompletionTime,
                    @Status,
                    @Remarks
                )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@ProductionOrderId",
                    work.ProductionOrderId);

                cmd.Parameters.AddWithValue(
                    "@MachineId",
                    work.MachineId);

                cmd.Parameters.AddWithValue(
                    "@EmployeeId",
                    work.EmployeeId);

                cmd.Parameters.AddWithValue(
                    "@StartDate",
                    (object?)work.StartDate ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@StartTime",
                    (object?)work.StartTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@PauseDate",
                    (object?)work.PauseDate ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@PauseTime",
                    (object?)work.PauseTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@ResumeDate",
                    (object?)work.ResumeDate ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@ResumeTime",
                    (object?)work.ResumeTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@CompletionDate",
                    (object?)work.CompletionDate ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@CompletionTime",
                    (object?)work.CompletionTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@Status",
                    work.Status ?? "Assigned");

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    work.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Production Work Status Added Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Start(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE ProductionWorkStatus
                                 SET StartDate = CAST(GETDATE() AS DATE),
                                     StartTime = CAST(GETDATE() AS TIME),
                                     Status = 'Running'
                                 WHERE WorkStatusId = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Production Started Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Pause(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE ProductionWorkStatus
                                 SET PauseDate = CAST(GETDATE() AS DATE),
                                     PauseTime = CAST(GETDATE() AS TIME),
                                     Status = 'Paused'
                                 WHERE WorkStatusId = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Production Paused Successfully.";

            return RedirectToAction("Index");
        }


        public IActionResult Resume(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE ProductionWorkStatus
                                 SET ResumeDate = CAST(GETDATE() AS DATE),
                                     ResumeTime = CAST(GETDATE() AS TIME),
                                     Status = 'Running'
                                 WHERE WorkStatusId = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Production Resumed Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Complete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE ProductionWorkStatus
                                 SET CompletionDate = CAST(GETDATE() AS DATE),
                                     CompletionTime = CAST(GETDATE() AS TIME),
                                     Status = 'Completed'
                                 WHERE WorkStatusId = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Production Completed Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            LoadDropdown();

            ProductionWorkStatus work =
                new ProductionWorkStatus();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "SELECT * FROM ProductionWorkStatus WHERE WorkStatusId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    work.WorkStatusId =
                        Convert.ToInt32(dr["WorkStatusId"]);

                    work.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    work.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    work.EmployeeId =
                        Convert.ToInt32(dr["EmployeeId"]);

                    work.StartDate =
                        dr["StartDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["StartDate"]);

                    work.StartTime =
                        dr["StartTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["StartTime"];

                    work.PauseDate =
                        dr["PauseDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["PauseDate"]);

                    work.PauseTime =
                        dr["PauseTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["PauseTime"];

                    work.ResumeDate =
                        dr["ResumeDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["ResumeDate"]);

                    work.ResumeTime =
                        dr["ResumeTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["ResumeTime"];

                    work.CompletionDate =
                        dr["CompletionDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["CompletionDate"]);

                    work.CompletionTime =
                        dr["CompletionTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["CompletionTime"];

                    work.Status =
                        dr["Status"].ToString();

                    work.Remarks =
                        dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(work);
        }
        
        [HttpPost]
        public IActionResult Edit(ProductionWorkStatus work)
        {
            ModelState.Remove("OrderNo");
            ModelState.Remove("MachineName");
            ModelState.Remove("EmployeeName");

            if (!ModelState.IsValid)
            {
                LoadDropdown();
                return View(work);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE ProductionWorkStatus SET
                                    ProductionOrderId=@ProductionOrderId,
                                    MachineId=@MachineId,
                                    EmployeeId=@EmployeeId,
                                    StartDate=@StartDate,
                                    StartTime=@StartTime,
                                    PauseDate=@PauseDate,
                                    PauseTime=@PauseTime,
                                    ResumeDate=@ResumeDate,
                                    ResumeTime=@ResumeTime,
                                    CompletionDate=@CompletionDate,
                                    CompletionTime=@CompletionTime,
                                    Status=@Status,
                                    Remarks=@Remarks
                                 WHERE WorkStatusId=@WorkStatusId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@WorkStatusId",
                    work.WorkStatusId);

                cmd.Parameters.AddWithValue(
                    "@ProductionOrderId",
                    work.ProductionOrderId);

                cmd.Parameters.AddWithValue(
                    "@MachineId",
                    work.MachineId);

                cmd.Parameters.AddWithValue(
                    "@EmployeeId",
                    work.EmployeeId);

                cmd.Parameters.AddWithValue(
                    "@StartDate",
                    (object?)work.StartDate ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@StartTime",
                    (object?)work.StartTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@PauseDate",
                    (object?)work.PauseDate ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@PauseTime",
                    (object?)work.PauseTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@ResumeDate",
                    (object?)work.ResumeDate ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@ResumeTime",
                    (object?)work.ResumeTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@CompletionDate",
                    (object?)work.CompletionDate ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@CompletionTime",
                    (object?)work.CompletionTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@Status",
                    work.Status ?? "Assigned");

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    work.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Production Work Status Updated Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            ProductionWorkStatus work =
                new ProductionWorkStatus();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT PWS.*,
                                        PO.OrderNo,
                                        M.MachineName,
                                        E.FullName AS EmployeeName
                                 FROM ProductionWorkStatus PWS
                                 INNER JOIN ProductionOrders PO
                                    ON PWS.ProductionOrderId =
                                       PO.ProductionOrderId
                                 INNER JOIN Machines M
                                    ON PWS.MachineId = M.MachineId
                                 INNER JOIN Employees E
                                    ON PWS.EmployeeId = E.EmployeeId
                                 WHERE PWS.WorkStatusId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    work.WorkStatusId =
                        Convert.ToInt32(dr["WorkStatusId"]);

                    work.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    work.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    work.EmployeeId =
                        Convert.ToInt32(dr["EmployeeId"]);

                    work.StartDate =
                        dr["StartDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["StartDate"]);

                    work.StartTime =
                        dr["StartTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["StartTime"];

                    work.PauseDate =
                        dr["PauseDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["PauseDate"]);

                    work.PauseTime =
                        dr["PauseTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["PauseTime"];

                    work.ResumeDate =
                        dr["ResumeDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["ResumeDate"]);

                    work.ResumeTime =
                        dr["ResumeTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["ResumeTime"];

                    work.CompletionDate =
                        dr["CompletionDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["CompletionDate"]);

                    work.CompletionTime =
                        dr["CompletionTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["CompletionTime"];

                    work.Status =
                        dr["Status"].ToString();

                    work.Remarks =
                        dr["Remarks"].ToString();

                    work.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    work.OrderNo =
                        dr["OrderNo"].ToString();

                    work.MachineName =
                        dr["MachineName"].ToString();

                    work.EmployeeName =
                        dr["EmployeeName"].ToString();
                }

                con.Close();
            }

            return View(work);
        }

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "DELETE FROM ProductionWorkStatus WHERE WorkStatusId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Production Work Status Deleted Successfully.";

            return RedirectToAction("Index");
        }
    }
}
