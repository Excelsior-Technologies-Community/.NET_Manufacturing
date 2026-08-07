using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class ProductionScheduleController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True;TrustServerCertificate=True";

        private void LoadOrders()
        {
            List<SelectListItem> orderList = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT ProductionOrderId, OrderNo FROM ProductionOrders", con);

                con.Open();

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
            }

            ViewBag.OrderList = orderList;
        }

        public IActionResult Index()
        {
            List<ProductionSchedules> list = new List<ProductionSchedules>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT PS.*,PO.OrderNo,PO.ProductName,PO.Quantity
                                 FROM ProductionSchedules PS
                                 INNER JOIN ProductionOrders PO
                                 ON PS.ProductionOrderId=PO.ProductionOrderId
                                 ORDER BY PS.ScheduleId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    ProductionSchedules ps = new ProductionSchedules();

                    ps.ScheduleId = Convert.ToInt32(dr["ScheduleId"]);
                    ps.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                    ps.StartDate = Convert.ToDateTime(dr["StartDate"]);
                    ps.EndDate = Convert.ToDateTime(dr["EndDate"]);
                    ps.Priority = dr["Priority"].ToString();
                    ps.Status = dr["Status"].ToString();
                    ps.Remarks = dr["Remarks"].ToString();
                    ps.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

                    ps.OrderNo = dr["OrderNo"].ToString();
                    ps.ProductName = dr["ProductName"].ToString();
                    ps.Quantity = Convert.ToInt32(dr["Quantity"]);

                    list.Add(ps);
                }

                con.Close();
            }

            return View(list);
        }
        public IActionResult Create()
        {
            LoadOrders();
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductionSchedules schedule)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO ProductionSchedules
                (ProductionOrderId,StartDate,EndDate,Priority,Status,Remarks)
                VALUES
                (@ProductionOrderId,@StartDate,@EndDate,@Priority,@Status,@Remarks)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ProductionOrderId", schedule.ProductionOrderId);
                cmd.Parameters.AddWithValue("@StartDate", schedule.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", schedule.EndDate);
                cmd.Parameters.AddWithValue("@Priority", schedule.Priority ?? "");
                cmd.Parameters.AddWithValue("@Status", schedule.Status ?? "Scheduled");
                cmd.Parameters.AddWithValue("@Remarks", schedule.Remarks ?? "");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            LoadOrders();

            ProductionSchedules schedule = new ProductionSchedules();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM ProductionSchedules WHERE ScheduleId=@Id", con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    schedule.ScheduleId = Convert.ToInt32(dr["ScheduleId"]);
                    schedule.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                    schedule.StartDate = Convert.ToDateTime(dr["StartDate"]);
                    schedule.EndDate = Convert.ToDateTime(dr["EndDate"]);
                    schedule.Priority = dr["Priority"].ToString();
                    schedule.Status = dr["Status"].ToString();
                    schedule.Remarks = dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(schedule);
        }

        [HttpPost]
        public IActionResult Edit(ProductionSchedules schedule)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE ProductionSchedules SET
                                ProductionOrderId=@ProductionOrderId,
                                StartDate=@StartDate,
                                EndDate=@EndDate,
                                Priority=@Priority,
                                Status=@Status,
                                Remarks=@Remarks
                                WHERE ScheduleId=@ScheduleId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ScheduleId", schedule.ScheduleId);
                cmd.Parameters.AddWithValue("@ProductionOrderId", schedule.ProductionOrderId);
                cmd.Parameters.AddWithValue("@StartDate", schedule.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", schedule.EndDate);
                cmd.Parameters.AddWithValue("@Priority", schedule.Priority ?? "");
                cmd.Parameters.AddWithValue("@Status", schedule.Status ?? "");
                cmd.Parameters.AddWithValue("@Remarks", schedule.Remarks ?? "");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            ProductionSchedules schedule = new ProductionSchedules();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT PS.*,PO.OrderNo,PO.ProductName,PO.Quantity
                                 FROM ProductionSchedules PS
                                 INNER JOIN ProductionOrders PO
                                 ON PS.ProductionOrderId=PO.ProductionOrderId
                                 WHERE PS.ScheduleId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    schedule.ScheduleId = Convert.ToInt32(dr["ScheduleId"]);
                    schedule.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                    schedule.StartDate = Convert.ToDateTime(dr["StartDate"]);
                    schedule.EndDate = Convert.ToDateTime(dr["EndDate"]);
                    schedule.Priority = dr["Priority"].ToString();
                    schedule.Status = dr["Status"].ToString();
                    schedule.Remarks = dr["Remarks"].ToString();
                    schedule.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

                    schedule.OrderNo = dr["OrderNo"].ToString();
                    schedule.ProductName = dr["ProductName"].ToString();
                    schedule.Quantity = Convert.ToInt32(dr["Quantity"]);
                }

                con.Close();
            }

            return View(schedule);
        }

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM ProductionSchedules WHERE ScheduleId=@Id", con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Index");
        }
    }
}
