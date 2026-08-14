using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class MachineReleaseController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True;TrustServerCertificate=True";

        private void LoadDropdown()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                List<SelectListItem> machineList = new List<SelectListItem>();

                SqlCommand machineCmd = new SqlCommand(
                    "SELECT MachineId, MachineName FROM Machines ORDER BY MachineName",
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

                List<SelectListItem> orderList = new List<SelectListItem>();

                SqlCommand orderCmd = new SqlCommand(
                    "SELECT ProductionOrderId, OrderNo FROM ProductionOrders ORDER BY OrderNo",
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
            List<MachineRelease> list = new List<MachineRelease>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MR.*,
                                        M.MachineName,
                                        PO.OrderNo
                                 FROM MachineReleases MR
                                 INNER JOIN Machines M
                                    ON MR.MachineId = M.MachineId
                                 INNER JOIN ProductionOrders PO
                                    ON MR.ProductionOrderId = PO.ProductionOrderId
                                 ORDER BY MR.ReleaseId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    MachineRelease release = new MachineRelease();

                    release.ReleaseId =
                        Convert.ToInt32(dr["ReleaseId"]);

                    release.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    release.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    release.ReleaseDate =
                        Convert.ToDateTime(dr["ReleaseDate"]);

                    release.ReleasedBy =
                        dr["ReleasedBy"].ToString();

                    release.RunningHours =
                        Convert.ToDecimal(dr["RunningHours"]);

                    release.Status =
                        dr["Status"].ToString();

                    release.Remarks =
                        dr["Remarks"].ToString();

                    release.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    release.MachineName =
                        dr["MachineName"].ToString();

                    release.OrderNo =
                        dr["OrderNo"].ToString();

                    list.Add(release);
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
        public IActionResult Create(MachineRelease release)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdown();
                return View(release);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO MachineReleases
                (
                    MachineId,
                    ProductionOrderId,
                    ReleaseDate,
                    ReleasedBy,
                    RunningHours,
                    Status,
                    Remarks
                )
                VALUES
                (
                    @MachineId,
                    @ProductionOrderId,
                    @ReleaseDate,
                    @ReleasedBy,
                    @RunningHours,
                    @Status,
                    @Remarks
                )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@MachineId", release.MachineId);

                cmd.Parameters.AddWithValue(
                    "@ProductionOrderId", release.ProductionOrderId);

                cmd.Parameters.AddWithValue(
                    "@ReleaseDate", release.ReleaseDate);

                cmd.Parameters.AddWithValue(
                    "@ReleasedBy", release.ReleasedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@RunningHours", release.RunningHours);

                cmd.Parameters.AddWithValue(
                    "@Status", release.Status ?? "Released");

                cmd.Parameters.AddWithValue(
                    "@Remarks", release.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Machine Released Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            LoadDropdown();

            MachineRelease release = new MachineRelease();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "SELECT * FROM MachineReleases WHERE ReleaseId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    release.ReleaseId =
                        Convert.ToInt32(dr["ReleaseId"]);

                    release.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    release.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    release.ReleaseDate =
                        Convert.ToDateTime(dr["ReleaseDate"]);

                    release.ReleasedBy =
                        dr["ReleasedBy"].ToString();

                    release.RunningHours =
                        Convert.ToDecimal(dr["RunningHours"]);

                    release.Status =
                        dr["Status"].ToString();

                    release.Remarks =
                        dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(release);
        }

        [HttpPost]
        public IActionResult Edit(MachineRelease release)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdown();
                return View(release);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE MachineReleases SET
                                    MachineId=@MachineId,
                                    ProductionOrderId=@ProductionOrderId,
                                    ReleaseDate=@ReleaseDate,
                                    ReleasedBy=@ReleasedBy,
                                    RunningHours=@RunningHours,
                                    Status=@Status,
                                    Remarks=@Remarks
                                 WHERE ReleaseId=@ReleaseId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@ReleaseId", release.ReleaseId);

                cmd.Parameters.AddWithValue(
                    "@MachineId", release.MachineId);

                cmd.Parameters.AddWithValue(
                    "@ProductionOrderId", release.ProductionOrderId);

                cmd.Parameters.AddWithValue(
                    "@ReleaseDate", release.ReleaseDate);

                cmd.Parameters.AddWithValue(
                    "@ReleasedBy", release.ReleasedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@RunningHours", release.RunningHours);

                cmd.Parameters.AddWithValue(
                    "@Status", release.Status ?? "Released");

                cmd.Parameters.AddWithValue(
                    "@Remarks", release.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Machine Release Updated Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            MachineRelease release = new MachineRelease();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MR.*,
                                        M.MachineName,
                                        PO.OrderNo
                                 FROM MachineReleases MR
                                 INNER JOIN Machines M
                                    ON MR.MachineId = M.MachineId
                                 INNER JOIN ProductionOrders PO
                                    ON MR.ProductionOrderId = PO.ProductionOrderId
                                 WHERE MR.ReleaseId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    release.ReleaseId =
                        Convert.ToInt32(dr["ReleaseId"]);

                    release.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    release.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    release.ReleaseDate =
                        Convert.ToDateTime(dr["ReleaseDate"]);

                    release.ReleasedBy =
                        dr["ReleasedBy"].ToString();

                    release.RunningHours =
                        Convert.ToDecimal(dr["RunningHours"]);

                    release.Status =
                        dr["Status"].ToString();

                    release.Remarks =
                        dr["Remarks"].ToString();

                    release.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    release.MachineName =
                        dr["MachineName"].ToString();

                    release.OrderNo =
                        dr["OrderNo"].ToString();
                }

                con.Close();
            }

            return View(release);
        }
        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "DELETE FROM MachineReleases WHERE ReleaseId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Machine Release Deleted Successfully.";

            return RedirectToAction("Index");
        }
    }
}
