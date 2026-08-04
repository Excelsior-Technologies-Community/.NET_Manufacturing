using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Manufacturing.Models
{
    public class RawMaterialController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True";

        public IActionResult Index()
        {
            List<RawMaterial> materials = new List<RawMaterial>();
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand("SELECT * FROM RawMaterials ORDER BY MaterialId DESC", con))
                {
                    con.Open();
                    using(SqlDataReader dr= cmd.ExecuteReader())
                    {
                        while(dr.Read())
                        {
                            materials.Add(new RawMaterial
                            {
                                MaterialId=Convert.ToInt32(dr["MaterialId"]),
                                MaterialCode = dr["MaterialCode"].ToString(),
                                MaterialName = dr["MaterialName"].ToString(),
                                Category = dr["Category"].ToString(),
                                Unit = dr["Unit"].ToString(),
                                CurrentStock = Convert.ToInt32(dr["CurrentStock"]),
                                MinimumStock = Convert.ToInt32(dr["MinimumStock"]),
                                PurchasePrice= Convert.ToDecimal(dr["PurchasePrice"]),
                                SupplierName = dr["SupplierName"].ToString(),
                                PurchaseDate = dr["PurchaseDate"] == DBNull.Value? null:Convert.ToDateTime(dr["PurchaseDate"]),
                                IssueQuantity = Convert.ToInt32(dr["IssueQuantity"]),
                                ApprovedBy = dr["ApprovedBy"].ToString(),
                                ApprovalStatus = dr["ApprovalStatus"].ToString(),
                                CreatedDate = dr["CreatedDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["CreatedDate"])
                            });
                        }
                    }
                }
            }
            return View(materials);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(RawMaterial material)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO RawMaterials
            (MaterialCode,MaterialName,Category,Unit,
             CurrentStock,MinimumStock,PurchasePrice,
             SupplierName,PurchaseDate,IssueQuantity,
             ApprovedBy,ApprovalStatus,Remarks)

            VALUES
            (@MaterialCode,@MaterialName,@Category,@Unit,
             @CurrentStock,@MinimumStock,@PurchasePrice,
             @SupplierName,@PurchaseDate,@IssueQuantity,
             @ApprovedBy,'Pending',@Remarks)", con))
                {
                    cmd.Parameters.AddWithValue("@MaterialCode", material.MaterialCode ?? "");
                    cmd.Parameters.AddWithValue("@MaterialName", material.MaterialName ?? "");
                    cmd.Parameters.AddWithValue("@Category", material.Category ?? "");
                    cmd.Parameters.AddWithValue("@Unit", material.Unit ?? "");
                    cmd.Parameters.AddWithValue("@CurrentStock", material.CurrentStock);
                    cmd.Parameters.AddWithValue("@MinimumStock", material.MinimumStock);
                    cmd.Parameters.AddWithValue("@PurchasePrice", material.PurchasePrice);
                    cmd.Parameters.AddWithValue("@SupplierName", material.SupplierName ?? "");
                    cmd.Parameters.AddWithValue("@PurchaseDate", (object?)material.PurchaseDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IssueQuantity", material.IssueQuantity);
                    cmd.Parameters.AddWithValue("@ApprovedBy", material.ApprovedBy ?? "");
                    cmd.Parameters.AddWithValue("@Remarks", material.Remarks ?? "");
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            RawMaterial material = new RawMaterial();
            using (SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM RawMaterials WHERE MaterialId=@MaterialId", con))
                {
                    cmd.Parameters.AddWithValue("@MaterialId", id);

                    con.Open();
                    using(SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if(dr.Read())
                        {
                            material.MaterialId = Convert.ToInt32(dr["MaterialId"]);
                            material.MaterialCode = dr["MaterialCode"].ToString();
                            material.MaterialName = dr["MaterialName"].ToString();
                            material.Category = dr["Category"].ToString();
                            material.Unit = dr["Unit"].ToString();
                            material.CurrentStock = Convert.ToInt32(dr["CurrentStock"]);
                            material.MinimumStock = Convert.ToInt32(dr["MinimumStock"]);
                            material.PurchasePrice = Convert.ToDecimal(dr["PurchasePrice"]);
                            material.SupplierName = dr["SupplierName"].ToString();

                            material.PurchaseDate = dr["PurchaseDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(dr["PurchaseDate"]);

                            material.IssueQuantity = Convert.ToInt32(dr["IssueQuantity"]);
                            material.ApprovedBy = dr["ApprovedBy"].ToString();
                            material.ApprovalStatus = dr["ApprovalStatus"].ToString();
                            material.Remarks = dr["Remarks"].ToString();
                        }
                    }
                }
            }
            return View(material);
        }

        [HttpPost]
        public IActionResult Edit(RawMaterial material)
        {
            using(SqlConnection con =new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand(@"UPDATE RawMaterials SET
                            MaterialCode=@MaterialCode,
                            MaterialName=@MaterialName,
                            Category=@Category,
                            Unit=@Unit,
                            CurrentStock=@CurrentStock,
                            MinimumStock=@MinimumStock,
                            PurchasePrice=@PurchasePrice,
                            SupplierName=@SupplierName,
                            PurchaseDate=@PurchaseDate,
                            IssueQuantity=@IssueQuantity,
                            ApprovedBy=@ApprovedBy,
                            ApprovalStatus=@ApprovalStatus,
                            Remarks=@Remarks
                         WHERE MaterialId=@MaterialId", con))
                {
                    cmd.Parameters.AddWithValue("@MaterialId", material.MaterialId);
                    cmd.Parameters.AddWithValue("@MaterialCode", material.MaterialCode ?? "");
                    cmd.Parameters.AddWithValue("@MaterialName", material.MaterialName ?? "");
                    cmd.Parameters.AddWithValue("@Category", material.Category ?? "");
                    cmd.Parameters.AddWithValue("@Unit", material.Unit ?? "");
                    cmd.Parameters.AddWithValue("@CurrentStock", material.CurrentStock);
                    cmd.Parameters.AddWithValue("@MinimumStock", material.MinimumStock);
                    cmd.Parameters.AddWithValue("@PurchasePrice", material.PurchasePrice);
                    cmd.Parameters.AddWithValue("@SupplierName", material.SupplierName ?? "");
                    cmd.Parameters.AddWithValue("@PurchaseDate", (object?)material.PurchaseDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IssueQuantity", material.IssueQuantity);
                    cmd.Parameters.AddWithValue("@ApprovedBy", material.ApprovedBy ?? "");
                    cmd.Parameters.AddWithValue("@ApprovalStatus", material.ApprovalStatus ?? "Pending");
                    cmd.Parameters.AddWithValue("@Remarks", material.Remarks ?? "");

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                TempData["Success"] = "Raw Material Updated Successfully.";

            }
            return RedirectToAction("Index");
        }

        public IActionResult Apporve(int id)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand("UPDATE RawMaterials SET ApprovalStatus='Approved' WHERE MaterialId=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand("DELETE FROM RawMaterials WHERE MaterialId=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult LowStock()
        {
            List<RawMaterial> materials = new List<RawMaterial>();
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand(@"SELECT * FROM RawMaterials
                         WHERE CurrentStock <= MinimumStock
                         ORDER BY CurrentStock ASC", con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while(dr.Read())
                        {
                            materials.Add(new RawMaterial()
                            {
                                MaterialId = Convert.ToInt32(dr["MaterialId"]),
                                MaterialCode = dr["MaterialCode"].ToString(),
                                MaterialName = dr["MaterialName"].ToString(),
                                Category = dr["Category"].ToString(),
                                Unit = dr["Unit"].ToString(),
                                CurrentStock = Convert.ToInt32(dr["CurrentStock"]),
                                MinimumStock = Convert.ToInt32(dr["MinimumStock"]),
                                PurchasePrice = Convert.ToDecimal(dr["PurchasePrice"]),
                                SupplierName = dr["SupplierName"].ToString(),
                                PurchaseDate = dr["PurchaseDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["PurchaseDate"]),
                                IssueQuantity = Convert.ToInt32(dr["IssueQuantity"]),
                                ApprovedBy = dr["ApprovedBy"].ToString(),
                                ApprovalStatus = dr["ApprovalStatus"].ToString(),
                                CreatedDate = dr["CreatedDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["CreatedDate"])
                            });
                        }
                    }
                }
            }
            return View(materials);
        }
    }
}
