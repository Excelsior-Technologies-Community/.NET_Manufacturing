using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class MachineAllocationController : Controller
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
            List<MachineAllocation> list = new List<MachineAllocation>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MA.*,
                                        M.MachineName,
                                        PO.OrderNo
                                 FROM MachineAllocations MA
                                 INNER JOIN Machines M
                                    ON MA.MachineId = M.MachineId
                                 INNER JOIN ProductionOrders PO
                                    ON MA.ProductionOrderId = PO.ProductionOrderId
                                 ORDER BY MA.AllocationId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    MachineAllocation allocation = new MachineAllocation();

                    allocation.AllocationId =
                        Convert.ToInt32(dr["AllocationId"]);

                    allocation.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    allocation.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    allocation.AllocationDate =
                        Convert.ToDateTime(dr["AllocationDate"]);

                    allocation.StartTime =
                        (TimeSpan)dr["StartTime"];

                    allocation.EndTime =
                        dr["EndTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["EndTime"];

                    allocation.AllocatedBy =
                        dr["AllocatedBy"].ToString();

                    allocation.Status =
                        dr["Status"].ToString();

                    allocation.Remarks =
                        dr["Remarks"].ToString();

                    allocation.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    allocation.MachineName =
                        dr["MachineName"].ToString();

                    allocation.OrderNo =
                        dr["OrderNo"].ToString();

                    list.Add(allocation);
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
        public IActionResult Create(MachineAllocation allocation)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdown();
                return View(allocation);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO MachineAllocations
                (
                    MachineId,
                    ProductionOrderId,
                    AllocationDate,
                    StartTime,
                    EndTime,
                    AllocatedBy,
                    Status,
                    Remarks
                )
                VALUES
                (
                    @MachineId,
                    @ProductionOrderId,
                    @AllocationDate,
                    @StartTime,
                    @EndTime,
                    @AllocatedBy,
                    @Status,
                    @Remarks
                )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@MachineId", allocation.MachineId);

                cmd.Parameters.AddWithValue(
                    "@ProductionOrderId", allocation.ProductionOrderId);

                cmd.Parameters.AddWithValue(
                    "@AllocationDate", allocation.AllocationDate);

                cmd.Parameters.AddWithValue(
                    "@StartTime", allocation.StartTime);

                cmd.Parameters.AddWithValue(
                    "@EndTime",
                    (object?)allocation.EndTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@AllocatedBy", allocation.AllocatedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@Status", allocation.Status ?? "Allocated");

                cmd.Parameters.AddWithValue(
                    "@Remarks", allocation.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Machine Allocated Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            LoadDropdown();

            MachineAllocation allocation = new MachineAllocation();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "SELECT * FROM MachineAllocations WHERE AllocationId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    allocation.AllocationId =
                        Convert.ToInt32(dr["AllocationId"]);
                       

                    allocation.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    allocation.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    allocation.AllocationDate =
                        Convert.ToDateTime(dr["AllocationDate"]);

                    allocation.StartTime =
                        (TimeSpan)dr["StartTime"];

                    allocation.EndTime =
                        dr["EndTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["EndTime"];

                    allocation.AllocatedBy =
                        dr["AllocatedBy"].ToString();

                    allocation.Status =
                        dr["Status"].ToString();

                    allocation.Remarks =
                        dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(allocation);
        }

        [HttpPost]
        public IActionResult Edit(MachineAllocation allocation)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdown();
                return View(allocation);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE MachineAllocations SET
                                    MachineId=@MachineId,
                                    ProductionOrderId=@ProductionOrderId,
                                    AllocationDate=@AllocationDate,
                                    StartTime=@StartTime,
                                    EndTime=@EndTime,
                                    AllocatedBy=@AllocatedBy,
                                    Status=@Status,
                                    Remarks=@Remarks
                                 WHERE AllocationId=@AllocationId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@AllocationId", allocation.AllocationId);

                cmd.Parameters.AddWithValue(
                    "@MachineId", allocation.MachineId);

                cmd.Parameters.AddWithValue(
                    "@ProductionOrderId", allocation.ProductionOrderId);

                cmd.Parameters.AddWithValue(
                    "@AllocationDate", allocation.AllocationDate);

                cmd.Parameters.AddWithValue(
                    "@StartTime", allocation.StartTime);

                cmd.Parameters.AddWithValue(
                    "@EndTime",
                    (object?)allocation.EndTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@AllocatedBy", allocation.AllocatedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@Status", allocation.Status ?? "Allocated");

                cmd.Parameters.AddWithValue(
                    "@Remarks", allocation.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Machine Allocation Updated Successfully.";

            return RedirectToAction("Index");
        }


        public IActionResult Details(int id)
        {
            MachineAllocation allocation = new MachineAllocation();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MA.*,
                                        M.MachineName,
                                        PO.OrderNo
                                 FROM MachineAllocations MA
                                 INNER JOIN Machines M
                                    ON MA.MachineId = M.MachineId
                                 INNER JOIN ProductionOrders PO
                                    ON MA.ProductionOrderId = PO.ProductionOrderId
                                 WHERE MA.AllocationId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    allocation.AllocationId =
                        Convert.ToInt32(dr["AllocationId"]);

                    allocation.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    allocation.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    allocation.AllocationDate =
                        Convert.ToDateTime(dr["AllocationDate"]);

                    allocation.StartTime =
                        (TimeSpan)dr["StartTime"];

                    allocation.EndTime =
                        dr["EndTime"] == DBNull.Value
                        ? null
                        : (TimeSpan?)dr["EndTime"];

                    allocation.AllocatedBy =
                        dr["AllocatedBy"].ToString();

                    allocation.Status =
                        dr["Status"].ToString();

                    allocation.Remarks =
                        dr["Remarks"].ToString();

                    allocation.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    allocation.MachineName =
                        dr["MachineName"].ToString();

                    allocation.OrderNo =
                        dr["OrderNo"].ToString();
                }

                con.Close();
            }

            return View(allocation);
        }

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "DELETE FROM MachineAllocations WHERE AllocationId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Machine Allocation Deleted Successfully.";

            return RedirectToAction("Index");
        }
    }
}
