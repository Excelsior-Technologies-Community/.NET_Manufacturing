
using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class MaterialRequirementPlanningController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True;TrustServerCertificate=True";

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

                List<SelectListItem> materialList = new List<SelectListItem>();

                SqlCommand cmd1 = new SqlCommand("SELECT MaterialId,MaterialName FROM RawMaterials", con);

                SqlDataReader dr1 = cmd1.ExecuteReader();

                while (dr1.Read())
                {
                    materialList.Add(new SelectListItem
                    {
                        Value = dr1["MaterialId"].ToString(),
                        Text = dr1["MaterialName"].ToString()
                    });
                }

                dr1.Close();

                ViewBag.OrderList = orderList;
                ViewBag.MaterialList = materialList;
            }
        }

        public IActionResult Index()
        {
            List<MaterialRequirementPlanning> list = new List<MaterialRequirementPlanning>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MRP.*,
                                PO.OrderNo,
                                RM.MaterialName
                         FROM MaterialRequirementPlanning MRP
                         INNER JOIN ProductionOrders PO
                            ON MRP.ProductionOrderId = PO.ProductionOrderId
                         INNER JOIN RawMaterials RM
                            ON MRP.MaterialId = RM.MaterialId
                         ORDER BY MRP.MaterialPlanId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    MaterialRequirementPlanning material = new MaterialRequirementPlanning();

                    material.MaterialPlanId = Convert.ToInt32(dr["MaterialPlanId"]);
                    material.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                    material.MaterialId = Convert.ToInt32(dr["MaterialId"]);
                    material.RequiredQuantity = Convert.ToInt32(dr["RequiredQuantity"]);
                    material.AvailableQuantity = Convert.ToInt32(dr["AvailableQuantity"]);
                    material.ShortageQuantity = Convert.ToInt32(dr["ShortageQuantity"]);
                    material.Status = dr["Status"].ToString();
                    material.PlanningDate = Convert.ToDateTime(dr["PlanningDate"]);
                    material.Remarks = dr["Remarks"].ToString();
                    material.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

                    material.OrderNo = dr["OrderNo"].ToString();
                    material.MaterialName = dr["MaterialName"].ToString();

                    list.Add(material);
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
        public IActionResult Create(MaterialRequirementPlanning material)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO MaterialRequirementPlanning
                (ProductionOrderId,MaterialId,RequiredQuantity,
                AvailableQuantity,ShortageQuantity,
                Status,PlanningDate,Remarks)

                VALUES
                (@ProductionOrderId,@MaterialId,@RequiredQuantity,
                @AvailableQuantity,@ShortageQuantity,
                @Status,@PlanningDate,@Remarks)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ProductionOrderId", material.ProductionOrderId);
                cmd.Parameters.AddWithValue("@MaterialId", material.MaterialId);
                cmd.Parameters.AddWithValue("@RequiredQuantity", material.RequiredQuantity);
                cmd.Parameters.AddWithValue("@AvailableQuantity", material.AvailableQuantity);
                cmd.Parameters.AddWithValue("@ShortageQuantity", material.ShortageQuantity);
                cmd.Parameters.AddWithValue("@Status", material.Status ?? "Pending");
                cmd.Parameters.AddWithValue("@PlanningDate", material.PlanningDate);
                cmd.Parameters.AddWithValue("@Remarks", material.Remarks ?? "");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            LoadDropdown();

            MaterialRequirementPlanning material = new MaterialRequirementPlanning();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM MaterialRequirementPlanning WHERE MaterialPlanId=@Id", con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    material.MaterialPlanId = Convert.ToInt32(dr["MaterialPlanId"]);
                    material.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                    material.MaterialId = Convert.ToInt32(dr["MaterialId"]);
                    material.RequiredQuantity = Convert.ToInt32(dr["RequiredQuantity"]);
                    material.AvailableQuantity = Convert.ToInt32(dr["AvailableQuantity"]);
                    material.ShortageQuantity = Convert.ToInt32(dr["ShortageQuantity"]);
                    material.Status = dr["Status"].ToString();
                    material.PlanningDate = Convert.ToDateTime(dr["PlanningDate"]);
                    material.Remarks = dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(material);
        }

        [HttpPost]
        public IActionResult Edit(MaterialRequirementPlanning material)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdown();
                return View(material);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE MaterialRequirementPlanning SET
                        ProductionOrderId=@ProductionOrderId,
                        MaterialId=@MaterialId,
                        RequiredQuantity=@RequiredQuantity,
                        AvailableQuantity=@AvailableQuantity,
                        ShortageQuantity=@ShortageQuantity,
                        Status=@Status,
                        PlanningDate=@PlanningDate,
                        Remarks=@Remarks
                        WHERE MaterialPlanId=@MaterialPlanId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@MaterialPlanId", material.MaterialPlanId);
                cmd.Parameters.AddWithValue("@ProductionOrderId", material.ProductionOrderId);
                cmd.Parameters.AddWithValue("@MaterialId", material.MaterialId);
                cmd.Parameters.AddWithValue("@RequiredQuantity", material.RequiredQuantity);
                cmd.Parameters.AddWithValue("@AvailableQuantity", material.AvailableQuantity);
                cmd.Parameters.AddWithValue("@ShortageQuantity", material.ShortageQuantity);
                cmd.Parameters.AddWithValue("@Status", material.Status ?? "");
                cmd.Parameters.AddWithValue("@PlanningDate", material.PlanningDate);
                cmd.Parameters.AddWithValue("@Remarks", material.Remarks ?? "");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            TempData["Success"] = "Material Requirement Updated Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            MaterialRequirementPlanning material = new MaterialRequirementPlanning();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MRP.*,
                                PO.OrderNo,
                                RM.MaterialName
                         FROM MaterialRequirementPlanning MRP
                         INNER JOIN ProductionOrders PO
                            ON MRP.ProductionOrderId = PO.ProductionOrderId
                         INNER JOIN RawMaterials RM
                            ON MRP.MaterialId = RM.MaterialId
                         WHERE MRP.MaterialPlanId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    material.MaterialPlanId = Convert.ToInt32(dr["MaterialPlanId"]);
                    material.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                    material.MaterialId = Convert.ToInt32(dr["MaterialId"]);
                    material.RequiredQuantity = Convert.ToInt32(dr["RequiredQuantity"]);
                    material.AvailableQuantity = Convert.ToInt32(dr["AvailableQuantity"]);
                    material.ShortageQuantity = Convert.ToInt32(dr["ShortageQuantity"]);
                    material.Status = dr["Status"].ToString();
                    material.PlanningDate = Convert.ToDateTime(dr["PlanningDate"]);
                    material.Remarks = dr["Remarks"].ToString();
                    material.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

                    material.OrderNo = dr["OrderNo"].ToString();
                    material.MaterialName = dr["MaterialName"].ToString();
                }

                con.Close();
            }

            return View(material);
        }

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM MaterialRequirementPlanning WHERE MaterialPlanId=@Id", con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            TempData["Success"] = "Material Requirement Deleted Successfully.";

            return RedirectToAction("Index");
        }
    }
}
