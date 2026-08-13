using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class MachineAssignmentController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True";

        public IActionResult Index()
        {
            List<MachineAssignment> list = new List<MachineAssignment>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MA.*,
                                PO.OrderNo,
                                M.MachineName
                         FROM MachineAssignments MA
                         INNER JOIN ProductionOrders PO
                            ON MA.ProductionOrderId = PO.ProductionOrderId
                         INNER JOIN Machines M
                            ON MA.MachineId = M.MachineId
                         ORDER BY MA.AssignmentId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    MachineAssignment assignment = new MachineAssignment();

                    assignment.AssignmentId = Convert.ToInt32(dr["AssignmentId"]);
                    assignment.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                    assignment.MachineId = Convert.ToInt32(dr["MachineId"]);
                    assignment.AssignedDate = Convert.ToDateTime(dr["AssignedDate"]);
                    assignment.Status = dr["Status"].ToString();
                    assignment.Remarks = dr["Remarks"].ToString();
                    assignment.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);
                    assignment.OrderNo = dr["OrderNo"].ToString();
                    assignment.MachineName = dr["MachineName"].ToString();

                    list.Add(assignment);
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
        public IActionResult Create(MachineAssignment assignment)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                EnsureRemarksColumnExpanded(con);

                string query = @"INSERT INTO MachineAssignments
                (ProductionOrderId,MachineId,AssignedDate,Status,Remarks)

                VALUES
                (@ProductionOrderId,@MachineId,@AssignedDate,@Status,@Remarks)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ProductionOrderId", assignment.ProductionOrderId);
                cmd.Parameters.AddWithValue("@MachineId", assignment.MachineId);
                cmd.Parameters.AddWithValue("@AssignedDate", assignment.AssignedDate);
                cmd.Parameters.AddWithValue("@Status", assignment.Status ?? "Assigned");
                cmd.Parameters.AddWithValue("@Remarks", assignment.Remarks ?? "");

                cmd.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            LoadDropdown();

            MachineAssignment assignment = new MachineAssignment();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT * FROM MachineAssignments WHERE AssignmentId=@AssignmentId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@AssignmentId", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    assignment.AssignmentId = Convert.ToInt32(dr["AssignmentId"]);
                    assignment.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                    assignment.MachineId = Convert.ToInt32(dr["MachineId"]);
                    assignment.AssignedDate = Convert.ToDateTime(dr["AssignedDate"]);
                    assignment.Status = dr["Status"].ToString();
                    assignment.Remarks = dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(assignment);
        }
        [HttpPost]
        public IActionResult Edit(MachineAssignment assignment)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdown();
                return View(assignment);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                EnsureRemarksColumnExpanded(con);

                string query = @"UPDATE MachineAssignments
                         SET
                         ProductionOrderId=@ProductionOrderId,
                         MachineId=@MachineId,
                         AssignedDate=@AssignedDate,
                         Status=@Status,
                         Remarks=@Remarks
                         WHERE AssignmentId=@AssignmentId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@AssignmentId", assignment.AssignmentId);
                cmd.Parameters.AddWithValue("@ProductionOrderId", assignment.ProductionOrderId);
                cmd.Parameters.AddWithValue("@MachineId", assignment.MachineId);
                cmd.Parameters.AddWithValue("@AssignedDate", assignment.AssignedDate);
                cmd.Parameters.AddWithValue("@Status", assignment.Status ?? "Assigned");
                cmd.Parameters.AddWithValue("@Remarks", assignment.Remarks ?? "");

                cmd.ExecuteNonQuery();
                con.Close();
            }

            TempData["Success"] = "Machine Assignment Updated Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            MachineAssignment assignment = new MachineAssignment();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MA.*,
                                PO.OrderNo,
                                M.MachineName
                         FROM MachineAssignments MA
                         INNER JOIN ProductionOrders PO
                            ON MA.ProductionOrderId = PO.ProductionOrderId
                         INNER JOIN Machines M
                            ON MA.MachineId = M.MachineId
                         WHERE MA.AssignmentId = @AssignmentId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@AssignmentId", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    assignment.AssignmentId = Convert.ToInt32(dr["AssignmentId"]);
                    assignment.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                    assignment.MachineId = Convert.ToInt32(dr["MachineId"]);
                    assignment.AssignedDate = Convert.ToDateTime(dr["AssignedDate"]);
                    assignment.Status = dr["Status"].ToString();
                    assignment.Remarks = dr["Remarks"].ToString();
                    assignment.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

                    // Display Fields
                    assignment.OrderNo = dr["OrderNo"].ToString();
                    assignment.MachineName = dr["MachineName"].ToString();
                }

                con.Close();
            }

            return View(assignment);
        }

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM MachineAssignments WHERE AssignmentId=@Id", con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Index");
        }

        private void EnsureRemarksColumnExpanded(SqlConnection con)
        {
            try
            {
                using (SqlCommand alterCmd = new SqlCommand("ALTER TABLE MachineAssignments ALTER COLUMN Remarks NVARCHAR(MAX)", con))
                {
                    alterCmd.ExecuteNonQuery();
                }
            }
            catch
            {
            }
        }

        private void LoadDropdown()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                List<SelectListItem> orders = new List<SelectListItem>();

                SqlCommand cmd = new SqlCommand("SELECT ProductionOrderId,OrderNo FROM ProductionOrders", con);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    orders.Add(new SelectListItem
                    {
                        Value = dr["ProductionOrderId"].ToString(),
                        Text = dr["OrderNo"].ToString()
                    });
                }

                dr.Close();

                
                List<SelectListItem> machines = new List<SelectListItem>();

                SqlCommand cmd1 = new SqlCommand("SELECT MachineId,MachineName FROM Machines", con);

                SqlDataReader dr1 = cmd1.ExecuteReader();

                while (dr1.Read())
                {
                    machines.Add(new SelectListItem
                    {
                        Value = dr1["MachineId"].ToString(),
                        Text = dr1["MachineName"].ToString()
                    });
                }

                dr1.Close();

                ViewBag.OrderList = orders;
                ViewBag.MachineList = machines;
            }
        }
    }
}
