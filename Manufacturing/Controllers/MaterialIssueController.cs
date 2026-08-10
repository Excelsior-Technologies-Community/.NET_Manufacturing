using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class MaterialIssueController : Controller
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
            List<MaterialIssue> list = new List<MaterialIssue>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MI.*,
                                        RM.MaterialName,
                                        PO.OrderNo
                                 FROM MaterialIssues MI
                                 INNER JOIN RawMaterials RM
                                    ON MI.MaterialId = RM.MaterialId
                                 INNER JOIN ProductionOrders PO
                                    ON MI.ProductionOrderId = PO.ProductionOrderId
                                 ORDER BY MI.IssueId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    MaterialIssue issue = new MaterialIssue();

                    issue.IssueId = Convert.ToInt32(dr["IssueId"]);
                    issue.MaterialId = Convert.ToInt32(dr["MaterialId"]);
                    issue.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    issue.IssueQuantity =
                        Convert.ToInt32(dr["IssueQuantity"]);

                    issue.IssueDate =
                        Convert.ToDateTime(dr["IssueDate"]);

                    issue.IssuedBy =
                        dr["IssuedBy"].ToString();

                    issue.Status =
                        dr["Status"].ToString();

                    issue.Remarks =
                        dr["Remarks"].ToString();

                    issue.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    issue.MaterialName =
                        dr["MaterialName"].ToString();

                    issue.OrderNo =
                        dr["OrderNo"].ToString();

                    list.Add(issue);
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
        public IActionResult Create(MaterialIssue issue)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdown();

                return View(issue);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO MaterialIssues
                                (
                                    MaterialId,
                                    ProductionOrderId,
                                    IssueQuantity,
                                    IssueDate,
                                    IssuedBy,
                                    Status,
                                    Remarks
                                )
                                VALUES
                                (
                                    @MaterialId,
                                    @ProductionOrderId,
                                    @IssueQuantity,
                                    @IssueDate,
                                    @IssuedBy,
                                    @Status,
                                    @Remarks
                                )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@MaterialId",
                    issue.MaterialId);

                cmd.Parameters.AddWithValue(
                    "@ProductionOrderId",
                    issue.ProductionOrderId);

                cmd.Parameters.AddWithValue(
                    "@IssueQuantity",
                    issue.IssueQuantity);

                cmd.Parameters.AddWithValue(
                    "@IssueDate",
                    issue.IssueDate);

                cmd.Parameters.AddWithValue(
                    "@IssuedBy",
                    issue.IssuedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@Status",
                    issue.Status ?? "Issued");

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    issue.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Material Issued Successfully.";

            return RedirectToAction("Index");
        }


        public IActionResult Edit(int id)
        {
            LoadDropdown();

            MaterialIssue issue = new MaterialIssue();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "SELECT * FROM MaterialIssues WHERE IssueId=@IssueId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@IssueId", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    issue.IssueId =
                        Convert.ToInt32(dr["IssueId"]);

                    issue.MaterialId =
                        Convert.ToInt32(dr["MaterialId"]);

                    issue.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    issue.IssueQuantity =
                        Convert.ToInt32(dr["IssueQuantity"]);

                    issue.IssueDate =
                        Convert.ToDateTime(dr["IssueDate"]);

                    issue.IssuedBy =
                        dr["IssuedBy"].ToString();

                    issue.Status =
                        dr["Status"].ToString();

                    issue.Remarks =
                        dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(issue);
        }

        [HttpPost]
        public IActionResult Edit(MaterialIssue issue)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdown();

                return View(issue);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE MaterialIssues SET
                                    MaterialId=@MaterialId,
                                    ProductionOrderId=@ProductionOrderId,
                                    IssueQuantity=@IssueQuantity,
                                    IssueDate=@IssueDate,
                                    IssuedBy=@IssuedBy,
                                    Status=@Status,
                                    Remarks=@Remarks
                                 WHERE IssueId=@IssueId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@IssueId",
                    issue.IssueId);

                cmd.Parameters.AddWithValue(
                    "@MaterialId",
                    issue.MaterialId);

                cmd.Parameters.AddWithValue(
                    "@ProductionOrderId",
                    issue.ProductionOrderId);

                cmd.Parameters.AddWithValue(
                    "@IssueQuantity",
                    issue.IssueQuantity);

                cmd.Parameters.AddWithValue(
                    "@IssueDate",
                    issue.IssueDate);

                cmd.Parameters.AddWithValue(
                    "@IssuedBy",
                    issue.IssuedBy ?? "");

                cmd.Parameters.AddWithValue(
                    "@Status",
                    issue.Status ?? "Issued");

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    issue.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Material Issue Updated Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            MaterialIssue issue = new MaterialIssue();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MI.*,
                                        RM.MaterialName,
                                        PO.OrderNo
                                 FROM MaterialIssues MI
                                 INNER JOIN RawMaterials RM
                                    ON MI.MaterialId = RM.MaterialId
                                 INNER JOIN ProductionOrders PO
                                    ON MI.ProductionOrderId = PO.ProductionOrderId
                                 WHERE MI.IssueId=@IssueId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@IssueId", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    issue.IssueId =
                        Convert.ToInt32(dr["IssueId"]);

                    issue.MaterialId =
                        Convert.ToInt32(dr["MaterialId"]);

                    issue.ProductionOrderId =
                        Convert.ToInt32(dr["ProductionOrderId"]);

                    issue.IssueQuantity =
                        Convert.ToInt32(dr["IssueQuantity"]);

                    issue.IssueDate =
                        Convert.ToDateTime(dr["IssueDate"]);

                    issue.IssuedBy =
                        dr["IssuedBy"].ToString();

                    issue.Status =
                        dr["Status"].ToString();

                    issue.Remarks =
                        dr["Remarks"].ToString();

                    issue.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    issue.MaterialName =
                        dr["MaterialName"].ToString();

                    issue.OrderNo =
                        dr["OrderNo"].ToString();
                }

                con.Close();
            }

            return View(issue);
        }

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "DELETE FROM MaterialIssues WHERE IssueId=@IssueId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@IssueId", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Material Issue Deleted Successfully.";

            return RedirectToAction("Index");
        }
    }
}
