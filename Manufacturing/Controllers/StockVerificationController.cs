using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class StockVerificationController : Controller
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
            List<StockVerification> list = new List<StockVerification>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT SV.*,
                                        RM.MaterialName
                                 FROM StockVerification SV
                                 INNER JOIN RawMaterials RM
                                    ON SV.MaterialId = RM.MaterialId
                                 ORDER BY SV.VerificationId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    StockVerification verification = new StockVerification();

                    verification.VerificationId =
                        Convert.ToInt32(dr["VerificationId"]);

                    verification.MaterialId =
                        Convert.ToInt32(dr["MaterialId"]);

                    verification.SystemStock =
                        Convert.ToInt32(dr["SystemStock"]);

                    verification.PhysicalStock =
                        Convert.ToInt32(dr["PhysicalStock"]);

                    verification.DifferenceStock =
                        Convert.ToInt32(dr["DifferenceStock"]);

                    verification.VerificationDate =
                        Convert.ToDateTime(dr["VerificationDate"]);

                    verification.VerifiedBy =
                        dr["VerifiedBy"].ToString();

                    verification.Status =
                        dr["Status"].ToString();

                    verification.Remarks =
                        dr["Remarks"].ToString();

                    verification.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    verification.MaterialName =
                        dr["MaterialName"].ToString();

                    list.Add(verification);
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
        public IActionResult Create(StockVerification verification)
        {
            if (!ModelState.IsValid)
            {
                LoadMaterials();

                return View(verification);
            }

            verification.DifferenceStock =
                verification.PhysicalStock -
                verification.SystemStock;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // 1. Insert into StockVerification table
                string query = @"INSERT INTO StockVerification
                                (
                                    MaterialId,
                                    SystemStock,
                                    PhysicalStock,
                                    DifferenceStock,
                                    VerificationDate,
                                    VerifiedBy,
                                    Status,
                                    Remarks
                                )
                                VALUES
                                (
                                    @MaterialId,
                                    @SystemStock,
                                    @PhysicalStock,
                                    @DifferenceStock,
                                    @VerificationDate,
                                    @VerifiedBy,
                                    @Status,
                                    @Remarks
                                )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@MaterialId", verification.MaterialId);
                cmd.Parameters.AddWithValue("@SystemStock", verification.SystemStock);
                cmd.Parameters.AddWithValue("@PhysicalStock", verification.PhysicalStock);
                cmd.Parameters.AddWithValue("@DifferenceStock", verification.DifferenceStock);
                cmd.Parameters.AddWithValue("@VerificationDate", verification.VerificationDate);
                cmd.Parameters.AddWithValue("@VerifiedBy", verification.VerifiedBy ?? "");
                cmd.Parameters.AddWithValue("@Status", verification.Status ?? "Verified");
                cmd.Parameters.AddWithValue("@Remarks", verification.Remarks ?? "");

                cmd.ExecuteNonQuery();

                // 2. Record new stock entry into MaterialStock table (CurrentStock = SystemStock, UpdatedStock = PhysicalStock)
                try
                {
                    string stockQuery = @"INSERT INTO MaterialStock
                                          (MaterialId, CurrentStock, AddedStock, UpdatedStock, UpdatedDate, UpdatedBy, Remarks)
                                          VALUES
                                          (@MaterialId, @CurrentStock, @AddedStock, @UpdatedStock, @UpdatedDate, @UpdatedBy, @Remarks)";

                    SqlCommand cmdStock = new SqlCommand(stockQuery, con);
                    cmdStock.Parameters.AddWithValue("@MaterialId", verification.MaterialId);
                    cmdStock.Parameters.AddWithValue("@CurrentStock", verification.SystemStock);
                    cmdStock.Parameters.AddWithValue("@AddedStock", verification.DifferenceStock);
                    cmdStock.Parameters.AddWithValue("@UpdatedStock", verification.PhysicalStock);
                    cmdStock.Parameters.AddWithValue("@UpdatedDate", verification.VerificationDate);
                    cmdStock.Parameters.AddWithValue("@UpdatedBy", verification.VerifiedBy ?? "Auditor");
                    cmdStock.Parameters.AddWithValue("@Remarks", "Stock Audit: " + (verification.Remarks ?? "Physical Audit"));
                    cmdStock.ExecuteNonQuery();

                    // 3. Update CurrentStock in RawMaterials table
                    string rawQuery = @"UPDATE RawMaterials SET CurrentStock = @CurrentStock WHERE MaterialId = @MaterialId";
                    SqlCommand cmdRaw = new SqlCommand(rawQuery, con);
                    cmdRaw.Parameters.AddWithValue("@CurrentStock", verification.PhysicalStock);
                    cmdRaw.Parameters.AddWithValue("@MaterialId", verification.MaterialId);
                    cmdRaw.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // Fallback log if table structure varies slightly
                    System.Diagnostics.Debug.WriteLine("MaterialStock update error: " + ex.Message);
                }

                con.Close();
            }

            TempData["Success"] = "Stock Verification Added Successfully. Material stock updated.";

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            StockVerification verification = new StockVerification();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT SV.*,
                                        RM.MaterialName
                                 FROM StockVerification SV
                                 LEFT JOIN RawMaterials RM
                                    ON SV.MaterialId = RM.MaterialId
                                 WHERE SV.VerificationId = @VerificationId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@VerificationId", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    verification.VerificationId = Convert.ToInt32(dr["VerificationId"]);
                    verification.MaterialId = Convert.ToInt32(dr["MaterialId"]);
                    verification.SystemStock = Convert.ToInt32(dr["SystemStock"]);
                    verification.PhysicalStock = Convert.ToInt32(dr["PhysicalStock"]);
                    verification.DifferenceStock = Convert.ToInt32(dr["DifferenceStock"]);
                    verification.VerificationDate = Convert.ToDateTime(dr["VerificationDate"]);
                    verification.VerifiedBy = dr["VerifiedBy"].ToString();
                    verification.Status = dr["Status"].ToString();
                    verification.Remarks = dr["Remarks"].ToString();
                    verification.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);
                    verification.MaterialName = dr["MaterialName"] != DBNull.Value ? dr["MaterialName"].ToString() : null;
                }

                con.Close();
            }

            return View(verification);
        }

        public IActionResult Edit(int id)
        {
            LoadMaterials();

            StockVerification verification = new StockVerification();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT * FROM StockVerification WHERE VerificationId = @VerificationId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@VerificationId", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    verification.VerificationId = Convert.ToInt32(dr["VerificationId"]);
                    verification.MaterialId = Convert.ToInt32(dr["MaterialId"]);
                    verification.SystemStock = Convert.ToInt32(dr["SystemStock"]);
                    verification.PhysicalStock = Convert.ToInt32(dr["PhysicalStock"]);
                    verification.DifferenceStock = Convert.ToInt32(dr["DifferenceStock"]);
                    verification.VerificationDate = Convert.ToDateTime(dr["VerificationDate"]);
                    verification.VerifiedBy = dr["VerifiedBy"].ToString();
                    verification.Status = dr["Status"].ToString();
                    verification.Remarks = dr["Remarks"].ToString();
                    verification.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);
                }

                con.Close();
            }

            return View(verification);
        }

        [HttpPost]
        public IActionResult Edit(StockVerification verification)
        {
            if (!ModelState.IsValid)
            {
                LoadMaterials();
                return View(verification);
            }

            verification.DifferenceStock =
                verification.PhysicalStock -
                verification.SystemStock;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                string query = @"UPDATE StockVerification
                                 SET
                                     MaterialId = @MaterialId,
                                     SystemStock = @SystemStock,
                                     PhysicalStock = @PhysicalStock,
                                     DifferenceStock = @DifferenceStock,
                                     VerificationDate = @VerificationDate,
                                     VerifiedBy = @VerifiedBy,
                                     Status = @Status,
                                     Remarks = @Remarks
                                 WHERE VerificationId = @VerificationId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@VerificationId", verification.VerificationId);
                cmd.Parameters.AddWithValue("@MaterialId", verification.MaterialId);
                cmd.Parameters.AddWithValue("@SystemStock", verification.SystemStock);
                cmd.Parameters.AddWithValue("@PhysicalStock", verification.PhysicalStock);
                cmd.Parameters.AddWithValue("@DifferenceStock", verification.DifferenceStock);
                cmd.Parameters.AddWithValue("@VerificationDate", verification.VerificationDate);
                cmd.Parameters.AddWithValue("@VerifiedBy", verification.VerifiedBy ?? "");
                cmd.Parameters.AddWithValue("@Status", verification.Status ?? "Verified");
                cmd.Parameters.AddWithValue("@Remarks", verification.Remarks ?? "");

                cmd.ExecuteNonQuery();

                // Also update MaterialStock and RawMaterials tables with UpdatedStock (PhysicalStock)
                try
                {
                    string stockQuery = @"INSERT INTO MaterialStock
                                          (MaterialId, CurrentStock, AddedStock, UpdatedStock, UpdatedDate, UpdatedBy, Remarks)
                                          VALUES
                                          (@MaterialId, @CurrentStock, @AddedStock, @UpdatedStock, @UpdatedDate, @UpdatedBy, @Remarks)";

                    SqlCommand cmdStock = new SqlCommand(stockQuery, con);
                    cmdStock.Parameters.AddWithValue("@MaterialId", verification.MaterialId);
                    cmdStock.Parameters.AddWithValue("@CurrentStock", verification.SystemStock);
                    cmdStock.Parameters.AddWithValue("@AddedStock", verification.DifferenceStock);
                    cmdStock.Parameters.AddWithValue("@UpdatedStock", verification.PhysicalStock);
                    cmdStock.Parameters.AddWithValue("@UpdatedDate", verification.VerificationDate);
                    cmdStock.Parameters.AddWithValue("@UpdatedBy", verification.VerifiedBy ?? "Auditor");
                    cmdStock.Parameters.AddWithValue("@Remarks", "Stock Audit Edit: " + (verification.Remarks ?? "Updated Audit"));
                    cmdStock.ExecuteNonQuery();

                    string rawQuery = @"UPDATE RawMaterials SET CurrentStock = @CurrentStock WHERE MaterialId = @MaterialId";
                    SqlCommand cmdRaw = new SqlCommand(rawQuery, con);
                    cmdRaw.Parameters.AddWithValue("@CurrentStock", verification.PhysicalStock);
                    cmdRaw.Parameters.AddWithValue("@MaterialId", verification.MaterialId);
                    cmdRaw.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("MaterialStock update error: " + ex.Message);
                }

                con.Close();
            }

            TempData["Success"] = "Stock Verification Updated Successfully. Material stock updated.";

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "DELETE FROM StockVerification WHERE VerificationId = @VerificationId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@VerificationId", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            TempData["Success"] = "Stock Verification Deleted Successfully.";

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
