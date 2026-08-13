using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class StockAdjustmentController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True;TrustServerCertificate=True";

        private void LoadMaterials()
        {
            List<SelectListItem> materialList = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MaterialId, MaterialName
                                 FROM RawMaterials
                                 ORDER BY MaterialName";

                SqlCommand cmd = new SqlCommand(query, con);

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
            List<StockAdjustment> list = new List<StockAdjustment>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT SA.*,
                                        RM.MaterialName
                                 FROM StockAdjustments SA
                                 INNER JOIN RawMaterials RM
                                    ON SA.MaterialId = RM.MaterialId
                                 ORDER BY SA.AdjustmentId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    StockAdjustment adjustment = new StockAdjustment();

                    adjustment.AdjustmentId =
                        Convert.ToInt32(dr["AdjustmentId"]);

                    adjustment.MaterialId =
                        Convert.ToInt32(dr["MaterialId"]);

                    adjustment.CurrentStock =
                        Convert.ToInt32(dr["CurrentStock"]);

                    adjustment.AdjustmentQuantity =
                        Convert.ToInt32(dr["AdjustmentQuantity"]);

                    adjustment.AdjustmentType =
                        dr["AdjustmentType"].ToString();

                    adjustment.AdjustmentDate =
                        Convert.ToDateTime(dr["AdjustmentDate"]);

                    adjustment.AdjustedBy =
                        dr["AdjustedBy"].ToString();

                    adjustment.Reason =
                        dr["Reason"].ToString();

                    adjustment.Status =
                        dr["Status"].ToString();

                    adjustment.Remarks =
                        dr["Remarks"].ToString();

                    adjustment.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    adjustment.MaterialName =
                        dr["MaterialName"].ToString();

                    list.Add(adjustment);
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
        public IActionResult Create(StockAdjustment adjustment)
        {
            if (!ModelState.IsValid)
            {
                LoadMaterials();
                return View(adjustment);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO StockAdjustments
                (
                    MaterialId,
                    CurrentStock,
                    AdjustmentQuantity,
                    AdjustmentType,
                    AdjustmentDate,
                    AdjustedBy,
                    Reason,
                    Status,
                    Remarks
                )
                VALUES
                (
                    @MaterialId,
                    @CurrentStock,
                    @AdjustmentQuantity,
                    @AdjustmentType,
                    @AdjustmentDate,
                    @AdjustedBy,
                    @Reason,
                    @Status,
                    @Remarks
                )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@MaterialId", adjustment.MaterialId);

                cmd.Parameters.AddWithValue(
                    "@CurrentStock", adjustment.CurrentStock);

                cmd.Parameters.AddWithValue(
                    "@AdjustmentQuantity", adjustment.AdjustmentQuantity);

                cmd.Parameters.AddWithValue(
                    "@AdjustmentType", adjustment.AdjustmentType ?? "");

                cmd.Parameters.AddWithValue(
                    "@AdjustmentDate", adjustment.AdjustmentDate);

                cmd.Parameters.AddWithValue(
                    "@AdjustedBy", adjustment.AdjustedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@Reason", adjustment.Reason ?? "");

                cmd.Parameters.AddWithValue(
                    "@Status", adjustment.Status ?? "Adjusted");

                cmd.Parameters.AddWithValue(
                    "@Remarks", adjustment.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Stock Adjustment Added Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            LoadMaterials();

            StockAdjustment adjustment = new StockAdjustment();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "SELECT * FROM StockAdjustments WHERE AdjustmentId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    adjustment.AdjustmentId =
                        Convert.ToInt32(dr["AdjustmentId"]);

                    adjustment.MaterialId =
                        Convert.ToInt32(dr["MaterialId"]);

                    adjustment.CurrentStock =
                        Convert.ToInt32(dr["CurrentStock"]);

                    adjustment.AdjustmentQuantity =
                        Convert.ToInt32(dr["AdjustmentQuantity"]);

                    adjustment.AdjustmentType =
                        dr["AdjustmentType"].ToString();

                    adjustment.AdjustmentDate =
                        Convert.ToDateTime(dr["AdjustmentDate"]);

                    adjustment.AdjustedBy =
                        dr["AdjustedBy"].ToString();

                    adjustment.Reason =
                        dr["Reason"].ToString();

                    adjustment.Status =
                        dr["Status"].ToString();

                    adjustment.Remarks =
                        dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(adjustment);
        }

        [HttpPost]
        public IActionResult Edit(StockAdjustment adjustment)
        {
            if (!ModelState.IsValid)
            {
                LoadMaterials();
                return View(adjustment);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE StockAdjustments SET
                                    MaterialId=@MaterialId,
                                    CurrentStock=@CurrentStock,
                                    AdjustmentQuantity=@AdjustmentQuantity,
                                    AdjustmentType=@AdjustmentType,
                                    AdjustmentDate=@AdjustmentDate,
                                    AdjustedBy=@AdjustedBy,
                                    Reason=@Reason,
                                    Status=@Status,
                                    Remarks=@Remarks
                                 WHERE AdjustmentId=@AdjustmentId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@AdjustmentId", adjustment.AdjustmentId);

                cmd.Parameters.AddWithValue(
                    "@MaterialId", adjustment.MaterialId);

                cmd.Parameters.AddWithValue(
                    "@CurrentStock", adjustment.CurrentStock);

                cmd.Parameters.AddWithValue(
                    "@AdjustmentQuantity", adjustment.AdjustmentQuantity);

                cmd.Parameters.AddWithValue(
                    "@AdjustmentType", adjustment.AdjustmentType ?? "");

                cmd.Parameters.AddWithValue(
                    "@AdjustmentDate", adjustment.AdjustmentDate);

                cmd.Parameters.AddWithValue(
                    "@AdjustedBy", adjustment.AdjustedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@Reason", adjustment.Reason ?? "");

                cmd.Parameters.AddWithValue(
                    "@Status", adjustment.Status ?? "Adjusted");

                cmd.Parameters.AddWithValue(
                    "@Remarks", adjustment.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Stock Adjustment Updated Successfully.";

            return RedirectToAction("Index");
        }


        public IActionResult Details(int id)
        {
            StockAdjustment adjustment = new StockAdjustment();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT SA.*,
                                        RM.MaterialName
                                 FROM StockAdjustments SA
                                 INNER JOIN RawMaterials RM
                                    ON SA.MaterialId = RM.MaterialId
                                 WHERE SA.AdjustmentId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    adjustment.AdjustmentId =
                        Convert.ToInt32(dr["AdjustmentId"]);

                    adjustment.MaterialId =
                        Convert.ToInt32(dr["MaterialId"]);

                    adjustment.CurrentStock =
                        Convert.ToInt32(dr["CurrentStock"]);

                    adjustment.AdjustmentQuantity =
                        Convert.ToInt32(dr["AdjustmentQuantity"]);

                    adjustment.AdjustmentType =
                        dr["AdjustmentType"].ToString();

                    adjustment.AdjustmentDate =
                        Convert.ToDateTime(dr["AdjustmentDate"]);

                    adjustment.AdjustedBy =
                        dr["AdjustedBy"].ToString();

                    adjustment.Reason =
                        dr["Reason"].ToString();

                    adjustment.Status =
                        dr["Status"].ToString();

                    adjustment.Remarks =
                        dr["Remarks"].ToString();

                    adjustment.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    adjustment.MaterialName =
                        dr["MaterialName"].ToString();
                }

                con.Close();
            }

            return View(adjustment);
        }

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "DELETE FROM StockAdjustments WHERE AdjustmentId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Stock Adjustment Deleted Successfully.";

            return RedirectToAction("Index");
        }
    }
}
