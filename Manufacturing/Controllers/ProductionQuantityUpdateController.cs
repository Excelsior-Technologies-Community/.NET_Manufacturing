using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class ProductionQuantityUpdateController : Controller
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
                    @"SELECT ProductionOrderId, OrderNo
                      FROM ProductionOrders
                      ORDER BY OrderNo",
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
                    @"SELECT MachineId, MachineName
                      FROM Machines
                      ORDER BY MachineName",
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
                    @"SELECT EmployeeId, FullName AS EmployeeName
                      FROM Employees
                      ORDER BY FullName",
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
            List<ProductionQuantityUpdate> list =
                new List<ProductionQuantityUpdate>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT PQU.*,
                                        PO.OrderNo,
                                        M.MachineName,
                                        E.FullName AS EmployeeName
                                 FROM ProductionQuantityUpdates PQU
                                 INNER JOIN ProductionOrders PO
                                    ON PQU.ProductionOrderId =
                                       PO.ProductionOrderId
                                 INNER JOIN Machines M
                                    ON PQU.MachineId = M.MachineId
                                 INNER JOIN Employees E
                                    ON PQU.EmployeeId = E.EmployeeId
                                 ORDER BY PQU.QuantityUpdateId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    ProductionQuantityUpdate item =
                        new ProductionQuantityUpdate();

                    item.QuantityUpdateId =
                        Convert.ToInt32(dr["QuantityUpdateId"]);

                    item.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    item.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    item.EmployeeId =
                        Convert.ToInt32(dr["EmployeeId"]);

                    item.UpdateDate =
                        Convert.ToDateTime(dr["UpdateDate"]);

                    item.PlannedQuantity =
                        Convert.ToInt32(dr["PlannedQuantity"]);

                    item.ProducedQuantity =
                        Convert.ToInt32(dr["ProducedQuantity"]);

                    item.RejectedQuantity =
                        Convert.ToInt32(dr["RejectedQuantity"]);

                    item.RemainingQuantity =
                        Convert.ToInt32(dr["RemainingQuantity"]);

                    item.Remarks =
                        dr["Remarks"].ToString();

                    item.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    item.OrderNo =
                        dr["OrderNo"].ToString();

                    item.MachineName =
                        dr["MachineName"].ToString();

                    item.EmployeeName =
                        dr["EmployeeName"].ToString();

                    list.Add(item);
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
        public IActionResult Create(ProductionQuantityUpdate item)
        {
            ModelState.Remove("OrderNo");
            ModelState.Remove("MachineName");
            ModelState.Remove("EmployeeName");

            if (!ModelState.IsValid)
            {
                LoadDropdown();
                return View(item);
            }

            item.RemainingQuantity =
                item.PlannedQuantity -
                item.ProducedQuantity -
                item.RejectedQuantity;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO ProductionQuantityUpdates
                (
                    ProductionOrderId,
                    MachineId,
                    EmployeeId,
                    UpdateDate,
                    PlannedQuantity,
                    ProducedQuantity,
                    RejectedQuantity,
                    RemainingQuantity,
                    Remarks
                )
                VALUES
                (
                    @ProductionOrderId,
                    @MachineId,
                    @EmployeeId,
                    @UpdateDate,
                    @PlannedQuantity,
                    @ProducedQuantity,
                    @RejectedQuantity,
                    @RemainingQuantity,
                    @Remarks
                )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@ProductionOrderId",
                    item.ProductionOrderId);

                cmd.Parameters.AddWithValue(
                    "@MachineId",
                    item.MachineId);

                cmd.Parameters.AddWithValue(
                    "@EmployeeId",
                    item.EmployeeId);

                cmd.Parameters.AddWithValue(
                    "@UpdateDate",
                    item.UpdateDate);

                cmd.Parameters.AddWithValue(
                    "@PlannedQuantity",
                    item.PlannedQuantity);

                cmd.Parameters.AddWithValue(
                    "@ProducedQuantity",
                    item.ProducedQuantity);

                cmd.Parameters.AddWithValue(
                    "@RejectedQuantity",
                    item.RejectedQuantity);

                cmd.Parameters.AddWithValue(
                    "@RemainingQuantity",
                    item.RemainingQuantity);

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    item.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Production Quantity Updated Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            LoadDropdown();

            ProductionQuantityUpdate item =
                new ProductionQuantityUpdate();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    @"SELECT *
                      FROM ProductionQuantityUpdates
                      WHERE QuantityUpdateId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    item.QuantityUpdateId =
                        Convert.ToInt32(dr["QuantityUpdateId"]);

                    item.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    item.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    item.EmployeeId =
                        Convert.ToInt32(dr["EmployeeId"]);

                    item.UpdateDate =
                        Convert.ToDateTime(dr["UpdateDate"]);

                    item.PlannedQuantity =
                        Convert.ToInt32(dr["PlannedQuantity"]);

                    item.ProducedQuantity =
                        Convert.ToInt32(dr["ProducedQuantity"]);

                    item.RejectedQuantity =
                        Convert.ToInt32(dr["RejectedQuantity"]);

                    item.RemainingQuantity =
                        Convert.ToInt32(dr["RemainingQuantity"]);

                    item.Remarks =
                        dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(ProductionQuantityUpdate item)
        {
            ModelState.Remove("OrderNo");
            ModelState.Remove("MachineName");
            ModelState.Remove("EmployeeName");

            if (!ModelState.IsValid)
            {
                LoadDropdown();
                return View(item);
            }

            // Recalculate Remaining Quantity
            item.RemainingQuantity =
                item.PlannedQuantity -
                item.ProducedQuantity -
                item.RejectedQuantity;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE ProductionQuantityUpdates SET
                                    ProductionOrderId=@ProductionOrderId,
                                    MachineId=@MachineId,
                                    EmployeeId=@EmployeeId,
                                    UpdateDate=@UpdateDate,
                                    PlannedQuantity=@PlannedQuantity,
                                    ProducedQuantity=@ProducedQuantity,
                                    RejectedQuantity=@RejectedQuantity,
                                    RemainingQuantity=@RemainingQuantity,
                                    Remarks=@Remarks
                                 WHERE QuantityUpdateId=@QuantityUpdateId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@QuantityUpdateId",
                    item.QuantityUpdateId);

                cmd.Parameters.AddWithValue(
                    "@ProductionOrderId",
                    item.ProductionOrderId);

                cmd.Parameters.AddWithValue(
                    "@MachineId",
                    item.MachineId);

                cmd.Parameters.AddWithValue(
                    "@EmployeeId",
                    item.EmployeeId);

                cmd.Parameters.AddWithValue(
                    "@UpdateDate",
                    item.UpdateDate);

                cmd.Parameters.AddWithValue(
                    "@PlannedQuantity",
                    item.PlannedQuantity);

                cmd.Parameters.AddWithValue(
                    "@ProducedQuantity",
                    item.ProducedQuantity);

                cmd.Parameters.AddWithValue(
                    "@RejectedQuantity",
                    item.RejectedQuantity);

                cmd.Parameters.AddWithValue(
                    "@RemainingQuantity",
                    item.RemainingQuantity);

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    item.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Production Quantity Updated Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            ProductionQuantityUpdate item =
                new ProductionQuantityUpdate();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT PQU.*,
                                        PO.OrderNo,
                                        M.MachineName,
                                        E.FullName AS EmployeeName
                                 FROM ProductionQuantityUpdates PQU
                                 INNER JOIN ProductionOrders PO
                                    ON PQU.ProductionOrderId =
                                       PO.ProductionOrderId
                                 INNER JOIN Machines M
                                    ON PQU.MachineId = M.MachineId
                                 INNER JOIN Employees E
                                    ON PQU.EmployeeId = E.EmployeeId
                                 WHERE PQU.QuantityUpdateId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    item.QuantityUpdateId =
                        Convert.ToInt32(dr["QuantityUpdateId"]);

                    item.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    item.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    item.EmployeeId =
                        Convert.ToInt32(dr["EmployeeId"]);

                    item.UpdateDate =
                        Convert.ToDateTime(dr["UpdateDate"]);

                    item.PlannedQuantity =
                        Convert.ToInt32(dr["PlannedQuantity"]);

                    item.ProducedQuantity =
                        Convert.ToInt32(dr["ProducedQuantity"]);

                    item.RejectedQuantity =
                        Convert.ToInt32(dr["RejectedQuantity"]);

                    item.RemainingQuantity =
                        Convert.ToInt32(dr["RemainingQuantity"]);

                    item.Remarks =
                        dr["Remarks"].ToString();

                    item.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    item.OrderNo =
                        dr["OrderNo"].ToString();

                    item.MachineName =
                        dr["MachineName"].ToString();

                    item.EmployeeName =
                        dr["EmployeeName"].ToString();
                }

                con.Close();
            }

            return View(item);
        }
        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    @"DELETE FROM ProductionQuantityUpdates
                      WHERE QuantityUpdateId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Production Quantity Update Deleted Successfully.";

            return RedirectToAction("Index");
        }
    }
}
