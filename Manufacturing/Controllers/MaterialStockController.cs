using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class MaterialStockController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True;TrustServerCertificate=True";

        private void LoadMaterials()
        {
            List<SelectListItem> materialList = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT MaterialId, MaterialName FROM RawMaterials", con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    materialList.Add(new SelectListItem
                    {
                        Value = dr["MaterialId"].ToString(),
                        Text = dr["MaterialName"].ToString()
                    });
                }

                dr.Close();
            }

            ViewBag.MaterialList = materialList;
        }

        public IActionResult Index()
        {
            List<MaterialStock> list = new List<MaterialStock>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MS.*, RM.MaterialName
                         FROM MaterialStock MS
                         INNER JOIN RawMaterials RM
                         ON MS.MaterialId = RM.MaterialId
                         ORDER BY MS.StockId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    MaterialStock stock = new MaterialStock();

                    stock.StockId = Convert.ToInt32(dr["StockId"]);
                    stock.MaterialId = Convert.ToInt32(dr["MaterialId"]);
                    stock.CurrentStock = Convert.ToInt32(dr["CurrentStock"]);
                    stock.AddedStock = Convert.ToInt32(dr["AddedStock"]);
                    stock.UpdatedStock = Convert.ToInt32(dr["UpdatedStock"]);
                    stock.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);
                    stock.UpdatedBy = dr["UpdatedBy"].ToString();
                    stock.Remarks = dr["Remarks"].ToString();
                    stock.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

                    stock.MaterialName = dr["MaterialName"].ToString();

                    list.Add(stock);
                }

                con.Close();
            }

            return View(list);
        }
        public IActionResult Create()
        {
            LoadMaterials();
            return View();
        }

        [HttpPost]
        public IActionResult Create(MaterialStock stock)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO MaterialStock
                (MaterialId,CurrentStock,AddedStock,
                 UpdatedStock,UpdatedDate,UpdatedBy,Remarks)

                VALUES
                (@MaterialId,@CurrentStock,@AddedStock,
                 @UpdatedStock,@UpdatedDate,@UpdatedBy,@Remarks)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@MaterialId", stock.MaterialId);
                cmd.Parameters.AddWithValue("@CurrentStock", stock.CurrentStock);
                cmd.Parameters.AddWithValue("@AddedStock", stock.AddedStock);
                cmd.Parameters.AddWithValue("@UpdatedStock", stock.UpdatedStock);
                cmd.Parameters.AddWithValue("@UpdatedDate", stock.UpdatedDate);
                cmd.Parameters.AddWithValue("@UpdatedBy", stock.UpdatedBy ?? "");
                cmd.Parameters.AddWithValue("@Remarks", stock.Remarks ?? "");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            LoadMaterials();

            MaterialStock stock = new MaterialStock();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM MaterialStock WHERE StockId=@Id", con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    stock.StockId = Convert.ToInt32(dr["StockId"]);
                    stock.MaterialId = Convert.ToInt32(dr["MaterialId"]);
                    stock.CurrentStock = Convert.ToInt32(dr["CurrentStock"]);
                    stock.AddedStock = Convert.ToInt32(dr["AddedStock"]);
                    stock.UpdatedStock = Convert.ToInt32(dr["UpdatedStock"]);
                    stock.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);
                    stock.UpdatedBy = dr["UpdatedBy"].ToString();
                    stock.Remarks = dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(stock);
        }

        [HttpPost]
        public IActionResult Edit(MaterialStock stock)
        {
            if (!ModelState.IsValid)
            {
                LoadMaterials();
                return View(stock);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE MaterialStock SET
                        MaterialId=@MaterialId,
                        CurrentStock=@CurrentStock,
                        AddedStock=@AddedStock,
                        UpdatedStock=@UpdatedStock,
                        UpdatedDate=@UpdatedDate,
                        UpdatedBy=@UpdatedBy,
                        Remarks=@Remarks
                        WHERE StockId=@StockId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@StockId", stock.StockId);
                cmd.Parameters.AddWithValue("@MaterialId", stock.MaterialId);
                cmd.Parameters.AddWithValue("@CurrentStock", stock.CurrentStock);
                cmd.Parameters.AddWithValue("@AddedStock", stock.AddedStock);
                cmd.Parameters.AddWithValue("@UpdatedStock", stock.UpdatedStock);
                cmd.Parameters.AddWithValue("@UpdatedDate", stock.UpdatedDate);
                cmd.Parameters.AddWithValue("@UpdatedBy", stock.UpdatedBy ?? "");
                cmd.Parameters.AddWithValue("@Remarks", stock.Remarks ?? "");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            TempData["Success"] = "Material Stock Updated Successfully.";

            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            MaterialStock stock = new MaterialStock();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MS.*, RM.MaterialName
                         FROM MaterialStock MS
                         INNER JOIN RawMaterials RM
                         ON MS.MaterialId = RM.MaterialId
                         WHERE MS.StockId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    stock.StockId = Convert.ToInt32(dr["StockId"]);
                    stock.MaterialId = Convert.ToInt32(dr["MaterialId"]);
                    stock.CurrentStock = Convert.ToInt32(dr["CurrentStock"]);
                    stock.AddedStock = Convert.ToInt32(dr["AddedStock"]);
                    stock.UpdatedStock = Convert.ToInt32(dr["UpdatedStock"]);
                    stock.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);
                    stock.UpdatedBy = dr["UpdatedBy"].ToString();
                    stock.Remarks = dr["Remarks"].ToString();
                    stock.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

                    stock.MaterialName = dr["MaterialName"].ToString();
                }

                con.Close();
            }

            return View(stock);
        }
        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM MaterialStock WHERE StockId=@Id", con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            TempData["Success"] = "Material Stock Deleted Successfully.";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetSystemStock(int materialId)
        {
            int currentStock = 0;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // 1. Fetch latest stock from MaterialStock table (UpdatedStock or CurrentStock)
                string materialStockQuery = @"
                    SELECT TOP 1 
                        CASE 
                            WHEN ISNULL(UpdatedStock, 0) > 0 THEN UpdatedStock
                            WHEN ISNULL(CurrentStock, 0) > 0 THEN CurrentStock
                            ELSE ISNULL(UpdatedStock, 0)
                        END AS StockQty
                    FROM MaterialStock 
                    WHERE MaterialId = @MaterialId 
                    ORDER BY StockId DESC";

                SqlCommand cmd1 = new SqlCommand(materialStockQuery, con);
                cmd1.Parameters.AddWithValue("@MaterialId", materialId);

                object res1 = cmd1.ExecuteScalar();

                if (res1 != null && res1 != DBNull.Value)
                {
                    currentStock = Convert.ToInt32(res1);
                }
                else
                {
                    // 2. Fallback to RawMaterials table if no entry exists in MaterialStock table
                    string rawMatQuery = @"SELECT ISNULL(CurrentStock, 0) FROM RawMaterials WHERE MaterialId = @MaterialId";
                    SqlCommand cmd2 = new SqlCommand(rawMatQuery, con);
                    cmd2.Parameters.AddWithValue("@MaterialId", materialId);

                    object res2 = cmd2.ExecuteScalar();

                    if (res2 != null && res2 != DBNull.Value)
                    {
                        currentStock = Convert.ToInt32(res2);
                    }
                }

                con.Close();
            }

            return Json(new { success = true, stock = currentStock });
        }
    }
}
