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
             @Priority,'Pending',@Remarks,@CreatedBy,GETDATE())"))
                {

                    cmd.Parameters.AddWithValue("@OrderNo", model.OrderNo);
                    cmd.Parameters.AddWithValue("@ProductName", model.ProductName);
                    cmd.Parameters.AddWithValue("@Quantity", model.Quantity);
                    cmd.Parameters.AddWithValue("@Unit", model.Unit);
                    cmd.Parameters.AddWithValue("@StartDate", model.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", model.EndDate);
                    cmd.Parameters.AddWithValue("@Priority", model.Priority);
                    cmd.Parameters.AddWithValue("@Remarks", model.Remarks);
                    cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
