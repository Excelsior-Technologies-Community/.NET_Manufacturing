using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class OperatorAssignmentController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True";
        public IActionResult Index()
        {
            List<OperatorAssignment> list = new List<OperatorAssignment>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT OA.*,
                                PO.OrderNo,
                                E.FullName,
                                S.ShiftName
                         FROM OperatorAssignments OA
                         INNER JOIN ProductionOrders PO
                            ON OA.ProductionOrderId = PO.ProductionOrderId
                         INNER JOIN Employees E
                            ON OA.EmployeeId = E.EmployeeId
                         INNER JOIN Shifts S
                            ON OA.ShiftId = S.ShiftId
                         ORDER BY OA.OperatorAssignmentId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    OperatorAssignment assignment = new OperatorAssignment();

                    assignment.OperatorAssignmentId = Convert.ToInt32(dr["OperatorAssignmentId"]);
                    assignment.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                    assignment.EmployeeId = Convert.ToInt32(dr["EmployeeId"]);
                    assignment.ShiftId = Convert.ToInt32(dr["ShiftId"]);
                    assignment.AssignedDate = Convert.ToDateTime(dr["AssignedDate"]);
                    assignment.Status = dr["Status"].ToString();
                    assignment.Remarks = dr["Remarks"].ToString();
                    assignment.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);
                    assignment.OrderNo = dr["OrderNo"].ToString();
                    assignment.EmployeeName = dr["FullName"].ToString();
                    assignment.ShiftName = dr["ShiftName"].ToString();

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
        public IActionResult Create(OperatorAssignment assignment)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                EnsureRemarksColumnExpanded(con);

                string query = @"INSERT INTO OperatorAssignments
                (ProductionOrderId,EmployeeId,ShiftId,
                AssignedDate,Status,Remarks)

                VALUES
                (@ProductionOrderId,@EmployeeId,@ShiftId,
                @AssignedDate,@Status,@Remarks)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ProductionOrderId", assignment.ProductionOrderId);
                cmd.Parameters.AddWithValue("@EmployeeId", assignment.EmployeeId);
                cmd.Parameters.AddWithValue("@ShiftId", assignment.ShiftId);
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

            OperatorAssignment assignment = new OperatorAssignment();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT * FROM OperatorAssignments WHERE OperatorAssignmentId=@OperatorAssignmentId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@OperatorAssignmentId", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    assignment.OperatorAssignmentId = Convert.ToInt32(dr["OperatorAssignmentId"]);
                    assignment.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                    assignment.EmployeeId = Convert.ToInt32(dr["EmployeeId"]);
                    assignment.ShiftId = Convert.ToInt32(dr["ShiftId"]);
                    assignment.AssignedDate = Convert.ToDateTime(dr["AssignedDate"]);
                    assignment.Status = dr["Status"].ToString();
                    assignment.Remarks = dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(assignment);
        }


        [HttpPost]
        public IActionResult Edit(OperatorAssignment assignment)
        {
            ModelState.Remove("OrderNo");
            ModelState.Remove("EmployeeName");
            ModelState.Remove("ShiftName");

            if (!ModelState.IsValid)
            {
                LoadDropdown();
                return View(assignment);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                EnsureRemarksColumnExpanded(con);

                string query = @"UPDATE OperatorAssignments SET
                        ProductionOrderId=@ProductionOrderId,
                        EmployeeId=@EmployeeId,
                        ShiftId=@ShiftId,
                        AssignedDate=@AssignedDate,
                        Status=@Status,
                        Remarks=@Remarks
                        WHERE OperatorAssignmentId=@OperatorAssignmentId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@OperatorAssignmentId", assignment.OperatorAssignmentId);
                cmd.Parameters.AddWithValue("@ProductionOrderId", assignment.ProductionOrderId);
                cmd.Parameters.AddWithValue("@EmployeeId", assignment.EmployeeId);
                cmd.Parameters.AddWithValue("@ShiftId", assignment.ShiftId);
                cmd.Parameters.AddWithValue("@AssignedDate", assignment.AssignedDate);
                cmd.Parameters.AddWithValue("@Status", assignment.Status ?? "Assigned");
                cmd.Parameters.AddWithValue("@Remarks", assignment.Remarks ?? "");

                cmd.ExecuteNonQuery();
                con.Close();
            }

            TempData["Success"] = "Operator Assignment Updated Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            OperatorAssignment assignment = new OperatorAssignment();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT OA.*,
                                PO.OrderNo,
                                E.FullName,
                                S.ShiftName
                         FROM OperatorAssignments OA
                         INNER JOIN ProductionOrders PO
                            ON OA.ProductionOrderId = PO.ProductionOrderId
                         INNER JOIN Employees E
                            ON OA.EmployeeId = E.EmployeeId
                         INNER JOIN Shifts S
                            ON OA.ShiftId = S.ShiftId
                         WHERE OA.OperatorAssignmentId=@OperatorAssignmentId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@OperatorAssignmentId", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    assignment.OperatorAssignmentId = Convert.ToInt32(dr["OperatorAssignmentId"]);
                    assignment.ProductionOrderId = Convert.ToInt32(dr["ProductionOrderId"]);
                    assignment.EmployeeId = Convert.ToInt32(dr["EmployeeId"]);
                    assignment.ShiftId = Convert.ToInt32(dr["ShiftId"]);
                    assignment.AssignedDate = Convert.ToDateTime(dr["AssignedDate"]);
                    assignment.Status = dr["Status"].ToString();
                    assignment.Remarks = dr["Remarks"].ToString();
                    assignment.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

                    assignment.OrderNo = dr["OrderNo"].ToString();
                    assignment.EmployeeName = dr["FullName"].ToString();
                    assignment.ShiftName = dr["ShiftName"].ToString();
                }

                con.Close();
            }

            return View(assignment);
        }

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "DELETE FROM OperatorAssignments WHERE OperatorAssignmentId=@OperatorAssignmentId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@OperatorAssignmentId", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            TempData["Success"] = "Operator Assignment Deleted Successfully.";

            return RedirectToAction("Index");
        }
        private void EnsureRemarksColumnExpanded(SqlConnection con)
        {
            try
            {
                using (SqlCommand alterCmd = new SqlCommand("ALTER TABLE OperatorAssignments ALTER COLUMN Remarks NVARCHAR(MAX)", con))
                {
                    alterCmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // Ignore if column is already expanded or permission restricted
            }
        }

        public void LoadDropdown()
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

                List<SelectListItem> employeeList = new List<SelectListItem>();

                SqlCommand cmd1 = new SqlCommand("SELECT EmployeeId,FullName FROM Employees", con);

                SqlDataReader dr1 = cmd1.ExecuteReader();

                while (dr1.Read())
                {
                    employeeList.Add(new SelectListItem
                    {
                        Value = dr1["EmployeeId"].ToString(),
                        Text = dr1["FullName"].ToString()
                    });
                }

                dr1.Close();

                List<SelectListItem> shiftList = new List<SelectListItem>();

                SqlCommand cmd2 = new SqlCommand("SELECT ShiftId,ShiftName FROM Shifts", con);

                SqlDataReader dr2 = cmd2.ExecuteReader();

                while (dr2.Read())
                {
                    shiftList.Add(new SelectListItem
                    {
                        Value = dr2["ShiftId"].ToString(),
                        Text = dr2["ShiftName"].ToString()
                    });
                }

                dr2.Close();

                ViewBag.OrderList = orderList;
                ViewBag.EmployeeList = employeeList;
                ViewBag.ShiftList = shiftList;
            }
        }
    }
}
