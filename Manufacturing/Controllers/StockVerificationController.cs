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

                cmd.Parameters.AddWithValue(
                    "@MaterialId",
                    verification.MaterialId);

                cmd.Parameters.AddWithValue(
                    "@SystemStock",
                    verification.SystemStock);

                cmd.Parameters.AddWithValue(
                    "@PhysicalStock",
                    verification.PhysicalStock);

                cmd.Parameters.AddWithValue(
                    "@DifferenceStock",
                    verification.DifferenceStock);

                cmd.Parameters.AddWithValue(
                    "@VerificationDate",
                    verification.VerificationDate);

                cmd.Parameters.AddWithValue(
                    "@VerifiedBy",
                    verification.VerifiedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@Status",
                    verification.Status ?? "Pending");

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    verification.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Stock Verification Added Successfully.";

            return RedirectToAction("Index");
        }


    }
}
