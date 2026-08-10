using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Models
{
    public class MaterialReturnController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True;TrustServerCertificate=True";

        private void LoadDropdown()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                List<SelectListItem> materialList = new List<SelectListItem>();

                SqlCommand materialCmd = new SqlCommand(
                    "SELECT MaterialId, MaterialName FROM RawMaterials",
                    con);

                SqlDataReader dr = materialCmd.ExecuteReader();

                while (dr.Read())
                {
                    materialList.Add(new SelectListItem
                    {
                        Value = dr["MaterialId"].ToString(),
                        Text = dr["MaterialName"].ToString()
                    });
                }

                dr.Close();

                List<SelectListItem> orderList = new List<SelectListItem>();

                SqlCommand orderCmd = new SqlCommand(
                    "SELECT ProductionOrderId, OrderNo FROM ProductionOrders",
                    con);

                SqlDataReader dr1 = orderCmd.ExecuteReader();

                while (dr1.Read())
                {
                    orderList.Add(new SelectListItem
                    {
                        Value = dr1["ProductionOrderId"].ToString(),
                        Text = dr1["OrderNo"].ToString()
                    });
                }

                dr1.Close();

                ViewBag.MaterialList = materialList;
                ViewBag.OrderList = orderList;
            }
        }

        public IActionResult Index()
        {
            List<MaterialReturn> list = new List<MaterialReturn>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MR.*,
                                        RM.MaterialName,
                                        PO.OrderNo
                                 FROM MaterialReturns MR
                                 INNER JOIN RawMaterials RM
                                    ON MR.MaterialId = RM.MaterialId
                                 INNER JOIN ProductionOrders PO
                                    ON MR.ProductionOrderId = PO.ProductionOrderId
                                 ORDER BY MR.ReturnId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    MaterialReturn materialReturn = new MaterialReturn();

                    materialReturn.ReturnId =
                        Convert.ToInt32(dr["ReturnId"]);

                    materialReturn.MaterialId =
                        Convert.ToInt32(dr["MaterialId"]);

                    materialReturn.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    materialReturn.ReturnQuantity =
                        Convert.ToInt32(dr["ReturnQuantity"]);

                    materialReturn.ReturnDate =
                        Convert.ToDateTime(dr["ReturnDate"]);

                    materialReturn.ReturnedBy =
                        dr["ReturnedBy"].ToString();

                    materialReturn.Status =
                        dr["Status"].ToString();

                    materialReturn.Remarks =
                        dr["Remarks"].ToString();

                    materialReturn.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    materialReturn.MaterialName =
                        dr["MaterialName"].ToString();

                    materialReturn.OrderNo =
                        dr["OrderNo"].ToString();

                    list.Add(materialReturn);
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
        public IActionResult Create(MaterialReturn materialReturn)
        {
            ModelState.Remove("MaterialName");
            ModelState.Remove("OrderNo");

            if (!ModelState.IsValid)
            {
                LoadDropdown();

                return View(materialReturn);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO MaterialReturns
                                (
                                    MaterialId,
                                    ProductionOrderId,
                                    ReturnQuantity,
                                    ReturnDate,
                                    ReturnedBy,
                                    Status,
                                    Remarks
                                )
                                VALUES
                                (
                                    @MaterialId,
                                    @ProductionOrderId,
                                    @ReturnQuantity,
                                    @ReturnDate,
                                    @ReturnedBy,
                                    @Status,
                                    @Remarks
                                )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@MaterialId",
                    materialReturn.MaterialId);

                cmd.Parameters.AddWithValue(
                    "@ProductionOrderId",
                    materialReturn.ProductionOrderId);

                cmd.Parameters.AddWithValue(
                    "@ReturnQuantity",
                    materialReturn.ReturnQuantity);

                cmd.Parameters.AddWithValue(
                    "@ReturnDate",
                    materialReturn.ReturnDate);

                cmd.Parameters.AddWithValue(
                    "@ReturnedBy",
                    materialReturn.ReturnedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@Status",
                    materialReturn.Status ?? "Returned");

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    materialReturn.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Material Returned Successfully.";

            return RedirectToAction("Index");
        }


        public IActionResult Edit(int id)
        {
            LoadDropdown();

            MaterialReturn materialReturn = new MaterialReturn();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "SELECT * FROM MaterialReturns WHERE ReturnId=@ReturnId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ReturnId", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    materialReturn.ReturnId =
                        Convert.ToInt32(dr["ReturnId"]);

                    materialReturn.MaterialId =
                        Convert.ToInt32(dr["MaterialId"]);

                    materialReturn.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    materialReturn.ReturnQuantity =
                        Convert.ToInt32(dr["ReturnQuantity"]);

                    materialReturn.ReturnDate =
                        Convert.ToDateTime(dr["ReturnDate"]);

                    materialReturn.ReturnedBy =
                        dr["ReturnedBy"].ToString();

                    materialReturn.Status =
                        dr["Status"].ToString();

                    materialReturn.Remarks =
                        dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(materialReturn);
        }

        [HttpPost]
        public IActionResult Edit(MaterialReturn materialReturn)
        {
            ModelState.Remove("MaterialName");
            ModelState.Remove("OrderNo");

            if (!ModelState.IsValid)
            {
                LoadDropdown();

                return View(materialReturn);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE MaterialReturns SET
                                    MaterialId=@MaterialId,
                                    ProductionOrderId=@ProductionOrderId,
                                    ReturnQuantity=@ReturnQuantity,
                                    ReturnDate=@ReturnDate,
                                    ReturnedBy=@ReturnedBy,
                                    Status=@Status,
                                    Remarks=@Remarks
                                 WHERE ReturnId=@ReturnId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@ReturnId",
                    materialReturn.ReturnId);

                cmd.Parameters.AddWithValue(
                    "@MaterialId",
                    materialReturn.MaterialId);

                cmd.Parameters.AddWithValue(
                    "@ProductionOrderId",
                    materialReturn.ProductionOrderId);

                cmd.Parameters.AddWithValue(
                    "@ReturnQuantity",
                    materialReturn.ReturnQuantity);

                cmd.Parameters.AddWithValue(
                    "@ReturnDate",
                    materialReturn.ReturnDate);

                cmd.Parameters.AddWithValue(
                    "@ReturnedBy",
                    materialReturn.ReturnedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@Status",
                    materialReturn.Status ?? "Returned");

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    materialReturn.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Material Return Updated Successfully.";

            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            MaterialReturn materialReturn = new MaterialReturn();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MR.*,
                                        RM.MaterialName,
                                        PO.OrderNo
                                 FROM MaterialReturns MR
                                 INNER JOIN RawMaterials RM
                                    ON MR.MaterialId = RM.MaterialId
                                 INNER JOIN ProductionOrders PO
                                    ON MR.ProductionOrderId = PO.ProductionOrderId
                                 WHERE MR.ReturnId=@ReturnId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ReturnId", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    materialReturn.ReturnId =
                        Convert.ToInt32(dr["ReturnId"]);

                    materialReturn.MaterialId =
                        Convert.ToInt32(dr["MaterialId"]);

                    materialReturn.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    materialReturn.ReturnQuantity =
                        Convert.ToInt32(dr["ReturnQuantity"]);

                    materialReturn.ReturnDate =
                        Convert.ToDateTime(dr["ReturnDate"]);

                    materialReturn.ReturnedBy =
                        dr["ReturnedBy"].ToString();

                    materialReturn.Status =
                        dr["Status"].ToString();

                    materialReturn.Remarks =
                        dr["Remarks"].ToString();

                    materialReturn.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    materialReturn.MaterialName =
                        dr["MaterialName"].ToString();

                    materialReturn.OrderNo =
                        dr["OrderNo"].ToString();
                }

                con.Close();
            }

            return View(materialReturn);
        }

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "DELETE FROM MaterialReturns WHERE ReturnId=@ReturnId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ReturnId", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Material Return Deleted Successfully.";

            return RedirectToAction("Index");
        }


    }
}
