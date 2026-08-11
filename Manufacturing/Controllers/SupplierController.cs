using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class SupplierController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True;TrustServerCertificate=True";

        public IActionResult Index()
        {
            List<Supplier> list = new List<Supplier>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT * FROM Suppliers
                                 ORDER BY SupplierId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Supplier supplier = new Supplier();

                    supplier.SupplierId = Convert.ToInt32(dr["SupplierId"]);

                    supplier.SupplierCode = dr["SupplierCode"].ToString();

                    supplier.SupplierName = dr["SupplierName"].ToString();

                    supplier.ContactPerson = dr["ContactPerson"].ToString();

                    supplier.Mobile = dr["Mobile"].ToString();

                    supplier.Email = dr["Email"].ToString();

                    supplier.Address = dr["Address"].ToString();

                    supplier.City = dr["City"].ToString();

                    supplier.GSTNumber = dr["GSTNumber"].ToString();

                    supplier.Status = dr["Status"].ToString();

                    supplier.Remarks = dr["Remarks"].ToString();

                    supplier.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

                    list.Add(supplier);
                }

                con.Close();
            }

            return View(list);
        }

        public IActionResult Create()
        {
            return View();

        }

        [HttpPost]
        public IActionResult Create(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return View(supplier);
            }
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO Suppliers
                (
                    SupplierCode,
                    SupplierName,
                    ContactPerson,
                    Mobile,
                    Email,
                    Address,
                    City,
                    GSTNumber,
                    Status,
                    Remarks
                )
                VALUES
                (
                    @SupplierCode,
                    @SupplierName,
                    @ContactPerson,
                    @Mobile,
                    @Email,
                    @Address,
                    @City,
                    @GSTNumber,
                    @Status,
                    @Remarks
                )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@SupplierCode", supplier.SupplierCode);
                cmd.Parameters.AddWithValue("@SupplierName", supplier.SupplierName);
                cmd.Parameters.AddWithValue("@ContactPerson", supplier.ContactPerson ?? "");
                cmd.Parameters.AddWithValue("@Mobile", supplier.Mobile ?? "");
                cmd.Parameters.AddWithValue("@Email", supplier.Email ?? "");
                cmd.Parameters.AddWithValue("@Address", supplier.Address ?? "");
                cmd.Parameters.AddWithValue("@City", supplier.City ?? "");
                cmd.Parameters.AddWithValue("@GSTNumber", supplier.GSTNumber ?? "");
                cmd.Parameters.AddWithValue("@Status", supplier.Status ?? "Active");
                cmd.Parameters.AddWithValue("@Remarks", supplier.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Supplier Added Successfully.";

            return RedirectToAction("Index");
        }


        public IActionResult Edit(int id)
        {
            Supplier supplier = new Supplier();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT * FROM Suppliers WHERE SupplierId=@SupplierId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@SupplierId", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    supplier.SupplierId = Convert.ToInt32(dr["SupplierId"]);

                    supplier.SupplierCode = dr["SupplierCode"].ToString();

                    supplier.SupplierName = dr["SupplierName"].ToString();

                    supplier.ContactPerson = dr["ContactPerson"].ToString();

                    supplier.Mobile = dr["Mobile"].ToString();

                    supplier.Email = dr["Email"].ToString();

                    supplier.Address = dr["Address"].ToString();

                    supplier.City = dr["City"].ToString();

                    supplier.GSTNumber = dr["GSTNumber"].ToString();

                    supplier.Status = dr["Status"].ToString();

                    supplier.Remarks = dr["Remarks"].ToString();
                }
                con.Close();
            }

            return View(supplier);
        }


        [HttpPost]
        public IActionResult Edit(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return View(supplier);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE Suppliers SET
                    SupplierCode=@SupplierCode,
                    SupplierName=@SupplierName,
                    ContactPerson=@ContactPerson,
                    Mobile=@Mobile,
                    Email=@Email,
                    Address=@Address,
                    City=@City,
                    GSTNumber=@GSTNumber,
                    Status=@Status,
                    Remarks=@Remarks
                    WHERE SupplierId=@SupplierId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@SupplierId", supplier.SupplierId);
                cmd.Parameters.AddWithValue("@SupplierCode", supplier.SupplierCode);
                cmd.Parameters.AddWithValue("@SupplierName", supplier.SupplierName);
                cmd.Parameters.AddWithValue("@ContactPerson", supplier.ContactPerson ?? "");
                cmd.Parameters.AddWithValue("@Mobile", supplier.Mobile ?? "");
                cmd.Parameters.AddWithValue("@Email", supplier.Email ?? "");
                cmd.Parameters.AddWithValue("@Address", supplier.Address ?? "");
                cmd.Parameters.AddWithValue("@City", supplier.City ?? "");
                cmd.Parameters.AddWithValue("@GSTNumber", supplier.GSTNumber ?? "");
                cmd.Parameters.AddWithValue("@Status", supplier.Status ?? "Active");
                cmd.Parameters.AddWithValue("@Remarks", supplier.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] = "Supplier Updated Successfully.";

            return RedirectToAction("Index");
        }


        public IActionResult Details(int id)
        {
            Supplier supplier = new Supplier();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT * FROM Suppliers WHERE SupplierId=@SupplierId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@SupplierId", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    supplier.SupplierId =Convert.ToInt32(dr["SupplierId"]);

                    supplier.SupplierCode =dr["SupplierCode"].ToString();

                    supplier.SupplierName = dr["SupplierName"].ToString();

                    supplier.ContactPerson = dr["ContactPerson"].ToString();

                    supplier.Mobile = dr["Mobile"].ToString();

                    supplier.Email = dr["Email"].ToString();

                    supplier.Address = dr["Address"].ToString();

                    supplier.City = dr["City"].ToString();

                    supplier.GSTNumber = dr["GSTNumber"].ToString();

                    supplier.Status = dr["Status"].ToString();

                    supplier.Remarks = dr["Remarks"].ToString();

                    supplier.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);
                }

                con.Close();
            }

            return View(supplier);
        }

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "DELETE FROM Suppliers WHERE SupplierId=@SupplierId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@SupplierId", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }
            TempData["Success"] = "Supplier Deleted Successfully.";
            return RedirectToAction("Index");
        }
    }
}
