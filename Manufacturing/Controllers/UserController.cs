using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class UserController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True";

        public IActionResult Index()
        {
            List<User> list= new List<User>();
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand("SELECT * FROM Users", con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while(dr.Read())
                        {
                            list.Add(new User { 
                                UserId = Convert.ToInt32(dr["UserId"]),
                                FullName = dr["FullName"].ToString(),
                                Email = dr["Email"].ToString(),
                                Mobile = dr["Mobile"].ToString(),
                                Username = dr["Username"].ToString(),
                                Password = dr["Password"].ToString(),
                                Role = dr["Role"].ToString(),
                                IsActive = Convert.ToBoolean(dr["IsActive"])
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
        public IActionResult Create(User user)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand(@"INSERT INTO Users
                            (FullName,Email,Mobile,Username,Password,Role,IsActive)
                            VALUES
                            (@FullName,@Email,@Mobile,@Username,@Password,@Role,@IsActive)", con))
                {
                    cmd.Parameters.AddWithValue("@FullName", user.FullName ?? "");
                    cmd.Parameters.AddWithValue("@Email", user.Email ?? "");
                    cmd.Parameters.AddWithValue("@Mobile", user.Mobile ?? "");
                    cmd.Parameters.AddWithValue("@Username", user.Username ?? "");
                    cmd.Parameters.AddWithValue("@Password", user.Password ?? "");
                    cmd.Parameters.AddWithValue("@Role", user.Role ?? "Machine Operator");
                    cmd.Parameters.AddWithValue("@IsActive", user.IsActive);

                    con.Open();
                    cmd.ExecuteNonQuery();

                }
            }
            return RedirectToAction("Index");   
        }

        public IActionResult Edit(int id)
        {
            User user = new User();
            using (SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand("SELECT * FROM Users WHERE UserId=@UserId", con))
                {
                    cmd.Parameters.AddWithValue("@UserId", id);
                    con.Open();
                    using(SqlDataReader dr= cmd.ExecuteReader())
                    {
                        if(dr.Read())
                        {
                            user.UserId = Convert.ToInt32(dr["UserId"]);
                            user.FullName = dr["FullName"].ToString();
                            user.Email = dr["Email"].ToString();
                            user.Mobile = dr["Mobile"].ToString();
                            user.Username = dr["Username"].ToString();
                            user.Password = dr["Password"].ToString();
                            user.Role = dr["Role"].ToString();
                            user.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        }
                    }
                }
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User user)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand(@"UPDATE Users SET
                            FullName=@FullName,
                            Email=@Email,
                            Mobile=@Mobile,
                            Username=@Username,
                            Password=@Password,
                            Role=@Role,
                            IsActive=@IsActive
                            WHERE UserId=@UserId", con))
                {
                    cmd.Parameters.AddWithValue("@UserId", user.UserId);
                    cmd.Parameters.AddWithValue("@FullName", user.FullName ?? "");
                    cmd.Parameters.AddWithValue("@Email", user.Email ?? "");
                    cmd.Parameters.AddWithValue("@Mobile", user.Mobile ?? "");
                    cmd.Parameters.AddWithValue("@Username", user.Username ?? "");
                    cmd.Parameters.AddWithValue("@Password", user.Password ?? "");
                    cmd.Parameters.AddWithValue("@Role", user.Role ?? "Machine Operator");
                    cmd.Parameters.AddWithValue("@IsActive", user.IsActive);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE UserId=@UserId", con))
                {
                    cmd.Parameters.AddWithValue("@UserId", id);

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
                using(SqlCommand cmd= new SqlCommand(@"UPDATE Users
                            SET IsActive =
                            CASE
                            WHEN IsActive = 1 THEN 0
                            ELSE 1
                            END
                            WHERE UserId=@UserId", con))
                {
                    cmd.Parameters.AddWithValue("@UserId", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
