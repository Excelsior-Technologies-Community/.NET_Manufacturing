using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class CapacityPlanningController : Controller
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

                List<SelectListItem> machineList = new List<SelectListItem>();

                SqlCommand cmd1 = new SqlCommand("SELECT MachineId,MachineName FROM Machines", con);

                SqlDataReader dr1 = cmd1.ExecuteReader();

                while (dr1.Read())
                {
                    machineList.Add(new SelectListItem
                    {
                        Value = dr1["MachineId"].ToString(),
                        Text = dr1["MachineName"].ToString()
                    });
                }

                dr1.Close();

                ViewBag.OrderList = orderList;
                ViewBag.MachineList = machineList;
            }
        }
    }
}
