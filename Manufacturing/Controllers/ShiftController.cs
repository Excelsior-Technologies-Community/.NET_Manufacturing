using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class ShiftController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True;TrustServerCertificate=True";

        public IActionResult Index()
        {
            List<Shift> list = new List<Shift>();
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd = new SqlCommand("SELECT * FROM Shifts ORDER BY ShiftId DESC",con))
                {
                    con.Open();
                    using(SqlDataReader dr= cmd.ExecuteReader())
                    {
                        while(dr.Read())
                        {
                            list.Add(new Shift
                            {
                                ShiftId = Convert.ToInt32(dr["ShiftId"]),
                                ShiftCode = dr["ShiftCode"].ToString(),
                                ShiftName = dr["ShiftName"].ToString(),
                                StartTime = TimeSpan.Parse(dr["StartTime"].ToString()),
                                EndTime = TimeSpan.Parse(dr["EndTime"].ToString()),
                                TotalHours = Convert.ToDecimal(dr["TotalHours"]),
                                SupervisorName = dr["SupervisorName"].ToString(),
                                Status = dr["Status"].ToString(),
                                Description = dr["Description"].ToString(),
                                CreatedDate = Convert.ToDateTime(dr["CreatedDate"]),
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
        public IActionResult Create(Shift shift)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand(@"INSERT INTO Shifts
                (ShiftCode,ShiftName,StartTime,EndTime,
                TotalHours,SupervisorName,Status,Description)
                VALUES
                (@ShiftCode,@ShiftName,@StartTime,@EndTime,
                @TotalHours,@SupervisorName,@Status,@Description)",con))
                {
                    cmd.Parameters.AddWithValue("@ShiftCode", shift.ShiftCode);
                    cmd.Parameters.AddWithValue("@ShiftName", shift.ShiftName);
                    cmd.Parameters.AddWithValue("@StartTime", shift.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", shift.EndTime);
                    cmd.Parameters.AddWithValue("@TotalHours", shift.TotalHours);
                    cmd.Parameters.AddWithValue("@SupervisorName", shift.SupervisorName ?? "");
                    cmd.Parameters.AddWithValue("@Status", shift.Status ?? "Active");
                    cmd.Parameters.AddWithValue("@Description", shift.Description ?? "");

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

       
        public IActionResult Edit(int id)
        {
            Shift shift = new Shift();
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand("SELECT * FROM Shifts WHERE ShiftId=@ShiftId",con))
                {
                    cmd.Parameters.AddWithValue("@ShiftId", id);
                    con.Open();
                    using(SqlDataReader dr= cmd.ExecuteReader())
                    {
                        if(dr.Read())
                        {
                            shift.ShiftId = Convert.ToInt32(dr["ShiftId"]);
                            shift.ShiftCode = dr["ShiftCode"].ToString();
                            shift.ShiftName = dr["ShiftName"].ToString();
                            shift.StartTime = TimeSpan.Parse(dr["StartTime"].ToString());
                            shift.EndTime = TimeSpan.Parse(dr["EndTime"].ToString());
                            shift.TotalHours = Convert.ToDecimal(dr["TotalHours"]);
                            shift.SupervisorName = dr["SupervisorName"].ToString();
                            shift.Status = dr["Status"].ToString();
                            shift.Description = dr["Description"].ToString();
                        }
                    }
                }
            }
            return View(shift);
        }

        [HttpPost]
        public IActionResult Edit(Shift shift)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand(@"UPDATE Shifts SET
                        ShiftCode=@ShiftCode,
                        ShiftName=@ShiftName,
                        StartTime=@StartTime,
                        EndTime=@EndTime,
                        TotalHours=@TotalHours,
                        SupervisorName=@SupervisorName,
                        Status=@Status,
                        Description=@Description
                        WHERE ShiftId=@ShiftId", con))
                {
                    cmd.Parameters.AddWithValue("@ShiftId", shift.ShiftId);
                    cmd.Parameters.AddWithValue("@ShiftCode", shift.ShiftCode);
                    cmd.Parameters.AddWithValue("@ShiftName", shift.ShiftName);
                    cmd.Parameters.AddWithValue("@StartTime", shift.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", shift.EndTime);
                    cmd.Parameters.AddWithValue("@TotalHours", shift.TotalHours);
                    cmd.Parameters.AddWithValue("@SupervisorName", shift.SupervisorName ?? "");
                    cmd.Parameters.AddWithValue("@Status", shift.Status ?? "Active");
                    cmd.Parameters.AddWithValue("@Description", shift.Description ?? "");

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
                using(SqlCommand cmd= new SqlCommand("DELETE FROM Shifts WHERE ShiftId=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult ChangeStatus(int id)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand(@"UPDATE Shifts SET Status = CASE WHEN Status='Active' THEN 'Inactive' ELSE 'Active'
                    END
                    WHERE ShiftId=@Id", con))
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
