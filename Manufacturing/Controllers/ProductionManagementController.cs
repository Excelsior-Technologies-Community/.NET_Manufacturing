using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class ProductionManagementController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True";

        public IActionResult Index()
        {
            List<ProductionOrder> list = new List<ProductionOrder>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT * FROM ProductionOrders ORDER BY ProductionOrderId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    ProductionOrder order = new ProductionOrder();

                    order.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                    order.OrderNo = dr["OrderNo"].ToString();
                    order.ProductName = dr["ProductName"].ToString();
                    order.Quantity = Convert.ToInt32(dr["Quantity"]);
                    order.Unit = dr["Unit"].ToString();

                    order.StartDate = dr["StartDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["StartDate"]);

                    order.EndDate = dr["EndDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["EndDate"]);

                    order.Priority = dr["Priority"].ToString();
                    order.Status = dr["Status"].ToString();
                    order.ApprovedBy = dr["ApprovedBy"].ToString();

                    order.ApprovedDate = dr["ApprovedDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["ApprovedDate"]);

                    order.Remarks = dr["Remarks"].ToString();
                    order.CreatedBy = dr["CreatedBy"].ToString();

                    order.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

                    order.ModifiedDate = dr["ModifiedDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["ModifiedDate"]);

                    list.Add(order);
                }

                con.Close();
            }

            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductionOrder model)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd = new SqlCommand(@"INSERT INTO ProductionOrders
            (OrderNo,ProductName,Quantity,Unit,StartDate,EndDate,
             Priority,Status,Remarks,CreatedBy,CreatedDate)

            VALUES
            (@OrderNo,@ProductName,@Quantity,@Unit,@StartDate,@EndDate,
             @Priority,'Pending',@Remarks,@CreatedBy,GETDATE())", con))
                {

                    cmd.Parameters.AddWithValue("@OrderNo", model.OrderNo ?? "");
                    cmd.Parameters.AddWithValue("@ProductName", model.ProductName ?? "");
                    cmd.Parameters.AddWithValue("@Quantity", model.Quantity);
                    cmd.Parameters.AddWithValue("@Unit", model.Unit ?? "");
                    cmd.Parameters.AddWithValue("@StartDate", (object?)model.StartDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EndDate", (object?)model.EndDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Priority", model.Priority ?? "");
                    cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? "");
                    cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy ?? "");

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            ProductionOrder order = new ProductionOrder();
            using (SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM ProductionOrders WHERE ProductionOrderId=@ProductionOrderId", con))
                {
                    cmd.Parameters.AddWithValue("@ProductionOrderId", id);
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            order.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                            order.OrderNo = dr["OrderNo"].ToString();
                            order.ProductName = dr["ProductName"].ToString();
                            order.Quantity = Convert.ToInt32(dr["Quantity"]);
                            order.Unit = dr["Unit"].ToString();

                            order.StartDate = dr["StartDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(dr["StartDate"]);

                            order.EndDate = dr["EndDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(dr["EndDate"]);

                            order.Priority = dr["Priority"].ToString();
                            order.Status = dr["Status"].ToString();
                            order.Remarks = dr["Remarks"].ToString();
                            order.CreatedBy = dr["CreatedBy"].ToString();
                        }
                    }
                }
            }
            return View(order);
        }

        [HttpPost]
        public IActionResult Edit(ProductionOrder order)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand(@"UPDATE ProductionOrders
                         SET
                            OrderNo=@OrderNo,
                            ProductName=@ProductName,
                            Quantity=@Quantity,
                            Unit=@Unit,
                            StartDate=@StartDate,
                            EndDate=@EndDate,
                            Priority=@Priority,
                            Status=@Status,
                            Remarks=@Remarks,
                            ModifiedDate=GETDATE()
                         WHERE ProductionOrderId=@ProductionOrderId", con))
                {
                    cmd.Parameters.AddWithValue("@ProductionOrderId", order.ProductionOrderId);
                    cmd.Parameters.AddWithValue("@OrderNo", order.OrderNo ?? "");
                    cmd.Parameters.AddWithValue("@ProductName", order.ProductName ?? "");
                    cmd.Parameters.AddWithValue("@Quantity", order.Quantity);
                    cmd.Parameters.AddWithValue("@Unit", order.Unit ?? "");
                    cmd.Parameters.AddWithValue("@StartDate", (object?)order.StartDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EndDate", (object?)order.EndDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Priority", order.Priority ?? "");
                    cmd.Parameters.AddWithValue("@Status", order.Status ?? "Pending");
                    cmd.Parameters.AddWithValue("@Remarks", order.Remarks ?? "");

                    con.Open();
                    cmd.ExecuteNonQuery();

                }
                TempData["Success"] = "Production Order Updated Successfully.";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Approve(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand(@"UPDATE ProductionOrders
                            SET Status='Approved',
                                ApprovedBy='Admin',
                                ApprovedDate=GETDATE()
                            WHERE ProductionOrderId=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Cancel(int id)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE ProductionOrders SET Status='Cancelled' WHERE ProductionOrderId=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

    }
}
