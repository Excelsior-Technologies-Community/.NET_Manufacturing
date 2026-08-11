using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class MaterialDispatchController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True;TrustServerCertificate=True";

        private void LoadDropdown()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                List<SelectListItem> materialList = new List<SelectListItem>();

                SqlCommand materialCmd = new SqlCommand(
                    "SELECT MaterialId, MaterialName FROM RawMaterials ORDER BY MaterialName",
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
                    "SELECT ProductionOrderId, OrderNo FROM ProductionOrders ORDER BY OrderNo",
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
            List<MaterialDispatch> list = new List<MaterialDispatch>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MD.*,
                                        RM.MaterialName,
                                        PO.OrderNo
                                 FROM MaterialDispatches MD
                                 INNER JOIN RawMaterials RM
                                    ON MD.MaterialId = RM.MaterialId
                                 LEFT JOIN ProductionOrders PO
                                    ON MD.ProductionOrderId = PO.ProductionOrderId
                                 ORDER BY MD.DispatchId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    MaterialDispatch dispatch = new MaterialDispatch();

                    dispatch.DispatchId =Convert.ToInt32(dr["DispatchId"]);

                    dispatch.MaterialId =  Convert.ToInt32(dr["MaterialId"]);

                    dispatch.ProductionOrderId = dr["ProductionOrderId"] == DBNull.Value ? null : Convert.ToInt32(dr["ProductionOrderId"]);

                    dispatch.DispatchQuantity = Convert.ToInt32(dr["DispatchQuantity"]);

                    dispatch.DispatchDate = Convert.ToDateTime(dr["DispatchDate"]);

                    dispatch.DispatchedBy = dr["DispatchedBy"].ToString();

                    dispatch.Destination = dr["Destination"].ToString();

                    dispatch.Status = dr["Status"].ToString();

                    dispatch.Remarks = dr["Remarks"].ToString();

                    dispatch.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

                    dispatch.MaterialName = dr["MaterialName"].ToString();


                    dispatch.OrderNo = dr["OrderNo"] == DBNull.Value ? "" : dr["OrderNo"].ToString();


                    list.Add(dispatch);
                }

                con.Close();
            }

            return View(list);
        }

    }
}
