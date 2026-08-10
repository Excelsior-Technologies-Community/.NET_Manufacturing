using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class CapacityPlanningController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True";

        private void LoadDropdown()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                List<SelectListItem> orderList = new List<SelectListItem>();

                SqlCommand cmd = new SqlCommand("SELECT ProductionOrderId,OrderNo FROM ProductionOrders", con);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    orderList.Add(new SelectListItem
                    {
                        Value = dr["ProductionOrderId"].ToString(),
                        Text = dr["OrderNo"].ToString()
                    });
                }

                dr.Close();

                List<SelectListItem> machineList = new List<SelectListItem>();

                SqlCommand cmd1 = new SqlCommand("SELECT MachineId,MachineName FROM Machines", con);

                SqlDataReader dr1 = cmd1.ExecuteReader();

                while (dr1.Read())
                {
                    machineList.Add(new SelectListItem
                    {
                        Value = dr1["MachineId"].ToString(),
                        Text = dr1["MachineName"].ToString()
                    });
                }

                dr1.Close();

                ViewBag.OrderList = orderList;
                ViewBag.MachineList = machineList;
            }
        }

        public IActionResult Index()
        {
            List<CapacityPlanning> list = new List<CapacityPlanning>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT CP.*,
                                PO.OrderNo,
                                M.MachineName
                         FROM CapacityPlanning CP
                         INNER JOIN ProductionOrders PO
                            ON CP.ProductionOrderId = PO.ProductionOrderId
                         INNER JOIN Machines M
                            ON CP.MachineId = M.MachineId
                         ORDER BY CP.CapacityPlanId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    CapacityPlanning capacity = new CapacityPlanning();

                    capacity.CapacityPlanId = Convert.ToInt32(dr["CapacityPlanId"]);
                    capacity.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                    capacity.MachineId = Convert.ToInt32(dr["MachineId"]);
                    capacity.AvailableHours = Convert.ToDecimal(dr["AvailableHours"]);
                    capacity.RequiredHours = Convert.ToDecimal(dr["RequiredHours"]);
                    capacity.CapacityStatus = dr["CapacityStatus"].ToString();
                    capacity.PlanningDate = Convert.ToDateTime(dr["PlanningDate"]);
                    capacity.Remarks = dr["Remarks"].ToString();
                    capacity.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

                    capacity.OrderNo = dr["OrderNo"].ToString();
                    capacity.MachineName = dr["MachineName"].ToString();

                    list.Add(capacity);
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
        public IActionResult Create(CapacityPlanning capacity)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO CapacityPlanning
                (MachineId,ProductionOrderId,AvailableHours,
                RequiredHours,CapacityStatus,PlanningDate,Remarks)

                VALUES
                (@MachineId,@ProductionOrderId,@AvailableHours,
                @RequiredHours,@CapacityStatus,@PlanningDate,@Remarks)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@MachineId", capacity.MachineId);
                cmd.Parameters.AddWithValue("@ProductionOrderId", capacity.ProductionOrderId);
                cmd.Parameters.AddWithValue("@AvailableHours", capacity.AvailableHours);
                cmd.Parameters.AddWithValue("@RequiredHours", capacity.RequiredHours);
                cmd.Parameters.AddWithValue("@CapacityStatus", capacity.CapacityStatus);
                cmd.Parameters.AddWithValue("@PlanningDate", capacity.PlanningDate);
                cmd.Parameters.AddWithValue("@Remarks", capacity.Remarks ?? "");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            LoadDropdown();

            CapacityPlanning capacity = new CapacityPlanning();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT * FROM CapacityPlanning WHERE CapacityPlanId=@CapacityPlanId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CapacityPlanId", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    capacity.CapacityPlanId = Convert.ToInt32(dr["CapacityPlanId"]);
                    capacity.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                    capacity.MachineId = Convert.ToInt32(dr["MachineId"]);
                    capacity.AvailableHours = Convert.ToDecimal(dr["AvailableHours"]);
                    capacity.RequiredHours = Convert.ToDecimal(dr["RequiredHours"]);
                    capacity.CapacityStatus = dr["CapacityStatus"].ToString();
                    capacity.PlanningDate = Convert.ToDateTime(dr["PlanningDate"]);
                    capacity.Remarks = dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(capacity);
        }

        [HttpPost]
        public IActionResult Edit(CapacityPlanning capacity)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdown();
                return View(capacity);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE CapacityPlanning SET
                        ProductionOrderId=@ProductionOrderId,
                        MachineId=@MachineId,
                        AvailableHours=@AvailableHours,
                        RequiredHours=@RequiredHours,
                        CapacityStatus=@CapacityStatus,
                        PlanningDate=@PlanningDate,
                        Remarks=@Remarks
                        WHERE CapacityPlanId=@CapacityPlanId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@CapacityPlanId", capacity.CapacityPlanId);
                cmd.Parameters.AddWithValue("@ProductionOrderId", capacity.ProductionOrderId);
                cmd.Parameters.AddWithValue("@MachineId", capacity.MachineId);
                cmd.Parameters.AddWithValue("@AvailableHours", capacity.AvailableHours);
                cmd.Parameters.AddWithValue("@RequiredHours", capacity.RequiredHours);
                cmd.Parameters.AddWithValue("@CapacityStatus", capacity.CapacityStatus ?? "");
                cmd.Parameters.AddWithValue("@PlanningDate", capacity.PlanningDate);
                cmd.Parameters.AddWithValue("@Remarks", capacity.Remarks ?? "");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            TempData["Success"] = "Capacity Planning Updated Successfully.";

            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            CapacityPlanning capacity = new CapacityPlanning();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT CP.*,
                                PO.OrderNo,
                                M.MachineName
                         FROM CapacityPlanning CP
                         INNER JOIN ProductionOrders PO
                            ON CP.ProductionOrderId = PO.ProductionOrderId
                         INNER JOIN Machines M
                            ON CP.MachineId = M.MachineId
                         WHERE CP.CapacityPlanId=@CapacityPlanId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CapacityPlanId", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    capacity.CapacityPlanId = Convert.ToInt32(dr["CapacityPlanId"]);
                    capacity.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                    capacity.MachineId = Convert.ToInt32(dr["MachineId"]);
                    capacity.AvailableHours = Convert.ToDecimal(dr["AvailableHours"]);
                    capacity.RequiredHours = Convert.ToDecimal(dr["RequiredHours"]);
                    capacity.CapacityStatus = dr["CapacityStatus"].ToString();
                    capacity.PlanningDate = Convert.ToDateTime(dr["PlanningDate"]);
                    capacity.Remarks = dr["Remarks"].ToString();
                    capacity.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

                    capacity.OrderNo = dr["OrderNo"].ToString();
                    capacity.MachineName = dr["MachineName"].ToString();
                }

                con.Close();
            }

            return View(capacity);
        }
        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM CapacityPlanning WHERE CapacityPlanId=@CapacityPlanId", con);

                cmd.Parameters.AddWithValue("@CapacityPlanId", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            TempData["Success"] = "Capacity Planning Deleted Successfully.";

            return RedirectToAction("Index");
        }
    }
}
