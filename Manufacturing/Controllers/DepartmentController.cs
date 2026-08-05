using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class DepartmentController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True";

        public IActionResult Index()
        {
            List<Department> list = new List<Department>();
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand("SELECT * FROM Department ORDER BY DepartmentId DESC", con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while(dr.Read())
                        {
                            list.Add(new Department
                            {
                                DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                                DepartmentName = dr["DepartmentName"].ToString() ?? "",
                                DepartmentCode = dr["DepartmentCode"].ToString() ?? "",
                                DepartmentHead = dr["DepartmentHead"].ToString() ?? "",
                                Description = dr["Description"].ToString() ?? "",
                                Status = dr["Status"].ToString() ?? "",
                                CreatedDate = dr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(dr["CreatedDate"]) : DateTime.Now,
                            });
                        }
                    }
                }
            }
            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Department department)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd = new SqlCommand(@"INSERT INTO Department
                                (DepartmentCode,DepartmentName,DepartmentHead,Description,Status)
                                VALUES
                                (@DepartmentCode,@DepartmentName,@DepartmentHead,@Description,@Status)", con))
                {
                    cmd.Parameters.AddWithValue("@DepartmentCode", department.DepartmentCode ?? "");
                    cmd.Parameters.AddWithValue("@DepartmentName", department.DepartmentName ?? "");
                    cmd.Parameters.AddWithValue("@DepartmentHead", department.DepartmentHead ?? "");
                    cmd.Parameters.AddWithValue("@Description", department.Description ?? "");
                    cmd.Parameters.AddWithValue("@Status", department.Status ?? "Active");
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Department department = new Department();
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand("SELECT * FROM Department WHERE DepartmentId=@DepartmentId", con))
                {
                    cmd.Parameters.AddWithValue("@DepartmentId", id);
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if(dr.Read())
                        {
                            department.DepartmentId = Convert.ToInt32(dr["DepartmentId"]);
                            department.DepartmentName = dr["DepartmentName"].ToString() ?? "";
                            department.DepartmentCode = dr["DepartmentCode"].ToString() ?? "";
                            department.DepartmentHead = dr["DepartmentHead"].ToString() ?? "";
                            department.Description = dr["Description"].ToString() ?? "";
                            department.Status = dr["Status"].ToString() ?? "";
                        }
                    }
                }
            }
            return View(department);
        }

        [HttpPost]
        public IActionResult Edit(Department department)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand(@"UPDATE Department SET
                                DepartmentCode=@DepartmentCode,
                                DepartmentName=@DepartmentName,
                                DepartmentHead=@DepartmentHead,
                                Description=@Description,
                                Status=@Status
                                WHERE DepartmentId=@DepartmentId", con))
                {
                    cmd.Parameters.AddWithValue("@DepartmentId", department.DepartmentId);
                    cmd.Parameters.AddWithValue("@DepartmentCode", department.DepartmentCode ?? "");
                    cmd.Parameters.AddWithValue("@DepartmentName", department.DepartmentName ?? "");
                    cmd.Parameters.AddWithValue("@DepartmentHead", department.DepartmentHead ?? "");
                    cmd.Parameters.AddWithValue("@Description", department.Description ?? "");
                    cmd.Parameters.AddWithValue("@Status", department.Status ?? "Active");

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand("DELETE FROM Department WHERE DepartmentId=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Status(int id)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand("UPDATE Department SET Status = CASE WHEN Status='Active' THEN 'Inactive' ELSE 'Active' END WHERE DepartmentId=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
