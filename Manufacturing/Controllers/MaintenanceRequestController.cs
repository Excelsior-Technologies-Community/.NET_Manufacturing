using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class MaintenanceRequestController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True;TrustServerCertificate=True";

        private void LoadMachines()
        {
            List<SelectListItem> machineList =
                new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MachineId, MachineName
                                 FROM Machines
                                 ORDER BY MachineName";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    machineList.Add(new SelectListItem
                    {
                        Value = dr["MachineId"].ToString(),
                        Text = dr["MachineName"].ToString()
                    });
                }

                dr.Close();
            }

            ViewBag.MachineList = machineList;
        }

        public IActionResult Index()
        {
            List<MaintenanceRequest> list =
                new List<MaintenanceRequest>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MR.*,
                                        M.MachineName
                                 FROM MaintenanceRequests MR
                                 INNER JOIN Machines M
                                    ON MR.MachineId = M.MachineId
                                 ORDER BY MR.RequestId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    MaintenanceRequest request =
                        new MaintenanceRequest();

                    request.RequestId =
                        Convert.ToInt32(dr["RequestId"]);

                    request.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    request.RequestDate =
                        Convert.ToDateTime(dr["RequestDate"]);

                    request.ProblemDescription =
                        dr["ProblemDescription"].ToString();

                    request.Priority =
                        dr["Priority"].ToString();

                    request.RequestedBy =
                        dr["RequestedBy"].ToString();

                    request.AssignedTo =
                        dr["AssignedTo"].ToString();

                    request.Status =
                        dr["Status"].ToString();

                    request.CompletionDate =
                        dr["CompletionDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["CompletionDate"]);

                    request.Remarks =
                        dr["Remarks"].ToString();

                    request.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    request.MachineName =
                        dr["MachineName"].ToString();

                    list.Add(request);
                }

                con.Close();
            }

            return View(list);
        }

        public IActionResult Create()
        {
            LoadMachines();

            return View();
        }

        [HttpPost]
        public IActionResult Create(MaintenanceRequest request)
        {
            if (!ModelState.IsValid)
            {
                LoadMachines();

                return View(request);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO MaintenanceRequests
                (
                    MachineId,
                    RequestDate,
                    ProblemDescription,
                    Priority,
                    RequestedBy,
                    AssignedTo,
                    Status,
                    CompletionDate,
                    Remarks
                )
                VALUES
                (
                    @MachineId,
                    @RequestDate,
                    @ProblemDescription,
                    @Priority,
                    @RequestedBy,
                    @AssignedTo,
                    @Status,
                    @CompletionDate,
                    @Remarks
                )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@MachineId",
                    request.MachineId);

                cmd.Parameters.AddWithValue(
                    "@RequestDate",
                    request.RequestDate);

                cmd.Parameters.AddWithValue(
                    "@ProblemDescription",
                    request.ProblemDescription);

                cmd.Parameters.AddWithValue(
                    "@Priority",
                    request.Priority ?? "Medium");

                cmd.Parameters.AddWithValue(
                    "@RequestedBy",
                    request.RequestedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@AssignedTo",
                    request.AssignedTo ?? "");

                cmd.Parameters.AddWithValue(
                    "@Status",
                    request.Status ?? "Pending");

                cmd.Parameters.AddWithValue(
                    "@CompletionDate",
                    (object?)request.CompletionDate ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    request.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Maintenance Request Added Successfully.";

            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            LoadMachines();

            MaintenanceRequest request =
                new MaintenanceRequest();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "SELECT * FROM MaintenanceRequests WHERE RequestId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    request.RequestId =
                        Convert.ToInt32(dr["RequestId"]);

                    request.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    request.RequestDate =
                        Convert.ToDateTime(dr["RequestDate"]);

                    request.ProblemDescription =
                        dr["ProblemDescription"].ToString();

                    request.Priority =
                        dr["Priority"].ToString();

                    request.RequestedBy =
                        dr["RequestedBy"].ToString();

                    request.AssignedTo =
                        dr["AssignedTo"].ToString();

                    request.Status =
                        dr["Status"].ToString();

                    request.CompletionDate =
                        dr["CompletionDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["CompletionDate"]);

                    request.Remarks =
                        dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(request);
        }

        [HttpPost]
        public IActionResult Edit(MaintenanceRequest request)
        {
            if (!ModelState.IsValid)
            {
                LoadMachines();

                return View(request);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE MaintenanceRequests SET
                                    MachineId=@MachineId,
                                    RequestDate=@RequestDate,
                                    ProblemDescription=@ProblemDescription,
                                    Priority=@Priority,
                                    RequestedBy=@RequestedBy,
                                    AssignedTo=@AssignedTo,
                                    Status=@Status,
                                    CompletionDate=@CompletionDate,
                                    Remarks=@Remarks
                                 WHERE RequestId=@RequestId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@RequestId",
                    request.RequestId);

                cmd.Parameters.AddWithValue(
                    "@MachineId",
                    request.MachineId);

                cmd.Parameters.AddWithValue(
                    "@RequestDate",
                    request.RequestDate);

                cmd.Parameters.AddWithValue(
                    "@ProblemDescription",
                    request.ProblemDescription);

                cmd.Parameters.AddWithValue(
                    "@Priority",
                    request.Priority ?? "Medium");

                cmd.Parameters.AddWithValue(
                    "@RequestedBy",
                    request.RequestedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@AssignedTo",
                    request.AssignedTo ?? "");

                cmd.Parameters.AddWithValue(
                    "@Status",
                    request.Status ?? "Pending");

                cmd.Parameters.AddWithValue(
                    "@CompletionDate",
                    (object?)request.CompletionDate ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    request.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Maintenance Request Updated Successfully.";

            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            MaintenanceRequest request =
                new MaintenanceRequest();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MR.*,
                                        M.MachineName
                                 FROM MaintenanceRequests MR
                                 INNER JOIN Machines M
                                    ON MR.MachineId = M.MachineId
                                 WHERE MR.RequestId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    request.RequestId =
                        Convert.ToInt32(dr["RequestId"]);

                    request.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    request.RequestDate =
                        Convert.ToDateTime(dr["RequestDate"]);

                    request.ProblemDescription =
                        dr["ProblemDescription"].ToString();

                    request.Priority =
                        dr["Priority"].ToString();

                    request.RequestedBy =
                        dr["RequestedBy"].ToString();

                    request.AssignedTo =
                        dr["AssignedTo"].ToString();

                    request.Status =
                        dr["Status"].ToString();

                    request.CompletionDate =
                        dr["CompletionDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["CompletionDate"]);

                    request.Remarks =
                        dr["Remarks"].ToString();

                    request.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    request.MachineName =
                        dr["MachineName"].ToString();
                }

                con.Close();
            }

            return View(request);
        }
        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "DELETE FROM MaintenanceRequests WHERE RequestId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Maintenance Request Deleted Successfully.";

            return RedirectToAction("Index");
        }
    }
}
