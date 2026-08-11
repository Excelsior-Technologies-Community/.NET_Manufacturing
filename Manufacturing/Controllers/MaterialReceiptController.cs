using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class MaterialReceiptController : Controller
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
            List<MaterialReceipt> list = new List<MaterialReceipt>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MR.*,
                                        RM.MaterialName
                                 FROM MaterialReceipts MR
                                 INNER JOIN RawMaterials RM
                                    ON MR.MaterialId = RM.MaterialId
                                 ORDER BY MR.ReceiptId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    MaterialReceipt receipt = new MaterialReceipt();

                    receipt.ReceiptId =
                        Convert.ToInt32(dr["ReceiptId"]);

                    receipt.MaterialId =
                        Convert.ToInt32(dr["MaterialId"]);

                    receipt.SupplierName =
                        dr["SupplierName"].ToString();

                    receipt.ReferenceNo =
                        dr["ReferenceNo"].ToString();

                    receipt.ReceivedQuantity =
                        Convert.ToInt32(dr["ReceivedQuantity"]);

                    receipt.ReceiveDate =
                        Convert.ToDateTime(dr["ReceiveDate"]);

                    receipt.ReceivedBy =
                        dr["ReceivedBy"].ToString();

                    receipt.Status =
                        dr["Status"].ToString();

                    receipt.Remarks =
                        dr["Remarks"].ToString();

                    receipt.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    receipt.MaterialName =
                        dr["MaterialName"].ToString();

                    list.Add(receipt);
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
        public IActionResult Create(MaterialReceipt receipt)
        {
            ModelState.Remove("ReceiptNumber");
            ModelState.Remove("ReceiptDate");
            ModelState.Remove("MaterialName");
            ModelState.Remove("MaterialCode");
            ModelState.Remove("Unit");

            if (!ModelState.IsValid)
            {
                LoadMaterials();

                return View(receipt);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO MaterialReceipts
                (
                    MaterialId,
                    SupplierName,
                    ReferenceNo,
                    ReceivedQuantity,
                    ReceiveDate,
                    ReceivedBy,
                    Status,
                    Remarks
                )
                VALUES
                (
                    @MaterialId,
                    @SupplierName,
                    @ReferenceNo,
                    @ReceivedQuantity,
                    @ReceiveDate,
                    @ReceivedBy,
                    @Status,
                    @Remarks
                )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@MaterialId",
                    receipt.MaterialId);

                cmd.Parameters.AddWithValue(
                    "@SupplierName",
                    receipt.SupplierName ?? "");

                cmd.Parameters.AddWithValue(
                    "@ReferenceNo",
                    receipt.ReferenceNo ?? "");

                cmd.Parameters.AddWithValue(
                    "@ReceivedQuantity",
                    receipt.ReceivedQuantity);

                cmd.Parameters.AddWithValue(
                    "@ReceiveDate",
                    receipt.ReceiveDate == DateTime.MinValue ? DateTime.Now : receipt.ReceiveDate);

                cmd.Parameters.AddWithValue(
                    "@ReceivedBy",
                    receipt.ReceivedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@Status",
                    receipt.Status ?? "Received");

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    receipt.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Material Received Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            LoadMaterials();

            MaterialReceipt receipt = new MaterialReceipt();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "SELECT * FROM MaterialReceipts WHERE ReceiptId=@ReceiptId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ReceiptId", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    receipt.ReceiptId =
                        Convert.ToInt32(dr["ReceiptId"]);

                    receipt.MaterialId =
                        Convert.ToInt32(dr["MaterialId"]);

                    receipt.SupplierName =
                        dr["SupplierName"].ToString();

                    receipt.ReferenceNo =
                        dr["ReferenceNo"].ToString();

                    receipt.ReceivedQuantity =
                        Convert.ToInt32(dr["ReceivedQuantity"]);

                    receipt.ReceiveDate =
                        Convert.ToDateTime(dr["ReceiveDate"]);

                    receipt.ReceivedBy =
                        dr["ReceivedBy"].ToString();

                    receipt.Status =
                        dr["Status"].ToString();

                    receipt.Remarks =
                        dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(receipt);
        }

        [HttpPost]
        public IActionResult Edit(MaterialReceipt receipt)
        {
            ModelState.Remove("ReceiptNumber");
            ModelState.Remove("ReceiptDate");
            ModelState.Remove("MaterialName");
            ModelState.Remove("MaterialCode");
            ModelState.Remove("Unit");

            if (!ModelState.IsValid)
            {
                LoadMaterials();

                return View(receipt);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE MaterialReceipts SET
                                    MaterialId=@MaterialId,
                                    SupplierName=@SupplierName,
                                    ReferenceNo=@ReferenceNo,
                                    ReceivedQuantity=@ReceivedQuantity,
                                    ReceiveDate=@ReceiveDate,
                                    ReceivedBy=@ReceivedBy,
                                    Status=@Status,
                                    Remarks=@Remarks
                                 WHERE ReceiptId=@ReceiptId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@ReceiptId",
                    receipt.ReceiptId);

                cmd.Parameters.AddWithValue(
                    "@MaterialId",
                    receipt.MaterialId);

                cmd.Parameters.AddWithValue(
                    "@SupplierName",
                    receipt.SupplierName ?? "");

                cmd.Parameters.AddWithValue(
                    "@ReferenceNo",
                    receipt.ReferenceNo ?? "");

                cmd.Parameters.AddWithValue(
                    "@ReceivedQuantity",
                    receipt.ReceivedQuantity);

                cmd.Parameters.AddWithValue(
                    "@ReceiveDate",
                    receipt.ReceiveDate == DateTime.MinValue ? DateTime.Now : receipt.ReceiveDate);

                cmd.Parameters.AddWithValue(
                    "@ReceivedBy",
                    receipt.ReceivedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@Status",
                    receipt.Status ?? "Received");

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    receipt.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Material Receipt Updated Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            MaterialReceipt receipt = new MaterialReceipt();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MR.*,
                                        RM.MaterialName
                                 FROM MaterialReceipts MR
                                 INNER JOIN RawMaterials RM
                                    ON MR.MaterialId = RM.MaterialId
                                 WHERE MR.ReceiptId=@ReceiptId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ReceiptId", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    receipt.ReceiptId =
                        Convert.ToInt32(dr["ReceiptId"]);

                    receipt.MaterialId =
                        Convert.ToInt32(dr["MaterialId"]);

                    receipt.SupplierName =
                        dr["SupplierName"].ToString();

                    receipt.ReferenceNo =
                        dr["ReferenceNo"].ToString();

                    receipt.ReceivedQuantity =
                        Convert.ToInt32(dr["ReceivedQuantity"]);

                    receipt.ReceiveDate =
                        Convert.ToDateTime(dr["ReceiveDate"]);

                    receipt.ReceivedBy =
                        dr["ReceivedBy"].ToString();

                    receipt.Status =
                        dr["Status"].ToString();

                    receipt.Remarks =
                        dr["Remarks"].ToString();

                    receipt.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    receipt.MaterialName =
                        dr["MaterialName"].ToString();
                }

                con.Close();
            }

            return View(receipt);
        }

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "DELETE FROM MaterialReceipts WHERE ReceiptId=@ReceiptId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@ReceiptId",
                    id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Material Receipt Deleted Successfully.";

            return RedirectToAction("Index");
        }
    }
}
