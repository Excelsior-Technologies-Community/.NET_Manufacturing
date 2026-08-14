using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class MachineAvailabilityController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True;TrustServerCertificate=True";

        private void LoadMachines()
        {
            List<SelectListItem> machineList = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MachineId, MachineName
                                 FROM Machines
                                 ORDER BY MachineName";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    machineList.Add(new SelectListItem
                    {
                        Value = dr["MachineId"].ToString(),
                        Text = dr["MachineName"].ToString()
                    });
                }

                dr.Close();
            }

            ViewBag.MachineList = machineList;
        }
        public IActionResult Index()
        {
            List<MachineAvailability> list =
                new List<MachineAvailability>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MA.*,
                                        M.MachineName
                                 FROM MachineAvailability MA
                                 INNER JOIN Machines M
                                    ON MA.MachineId = M.MachineId
                                 ORDER BY MA.AvailabilityId DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    MachineAvailability availability =
                        new MachineAvailability();

                    availability.AvailabilityId =
                        Convert.ToInt32(dr["AvailabilityId"]);

                    availability.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    availability.AvailabilityStatus =
                        dr["AvailabilityStatus"].ToString();

                    availability.AvailableFrom =
                        Convert.ToDateTime(dr["AvailableFrom"]);

                    availability.AvailableTo =
                        dr["AvailableTo"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["AvailableTo"]);

                    availability.CurrentStatus =
                        dr["CurrentStatus"].ToString();

                    availability.Remarks =
                        dr["Remarks"].ToString();

                    availability.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    availability.MachineName =
                        dr["MachineName"].ToString();

                    list.Add(availability);
                }

                con.Close();
            }

            return View(list);
        }

        public IActionResult Create()
        {
            LoadMachines();

            return View();
        }

        [HttpPost]
        public IActionResult Create(MachineAvailability availability)
        {
            if (!ModelState.IsValid)
            {
                LoadMachines();
                return View(availability);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO MachineAvailability
                (
                    MachineId,
                    AvailabilityStatus,
                    AvailableFrom,
                    AvailableTo,
                    CurrentStatus,
                    Remarks
                )
                VALUES
                (
                    @MachineId,
                    @AvailabilityStatus,
                    @AvailableFrom,
                    @AvailableTo,
                    @CurrentStatus,
                    @Remarks
                )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@MachineId",
                    availability.MachineId);

                cmd.Parameters.AddWithValue(
                    "@AvailabilityStatus",
                    availability.AvailabilityStatus ?? "");

                cmd.Parameters.AddWithValue(
                    "@AvailableFrom",
                    availability.AvailableFrom);

                cmd.Parameters.AddWithValue(
                    "@AvailableTo",
                    (object?)availability.AvailableTo ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@CurrentStatus",
                    availability.CurrentStatus ?? "");

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    availability.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Machine Availability Added Successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            LoadMachines();

            MachineAvailability availability =
                new MachineAvailability();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "SELECT * FROM MachineAvailability WHERE AvailabilityId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    availability.AvailabilityId =
                        Convert.ToInt32(dr["AvailabilityId"]);

                    availability.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    availability.AvailabilityStatus =
                        dr["AvailabilityStatus"].ToString();

                    availability.AvailableFrom =
                        Convert.ToDateTime(dr["AvailableFrom"]);

                    availability.AvailableTo =
                        dr["AvailableTo"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["AvailableTo"]);

                    availability.CurrentStatus =
                        dr["CurrentStatus"].ToString();

                    availability.Remarks =
                        dr["Remarks"].ToString();
                }

                con.Close();
            }

            return View(availability);
        }

        [HttpPost]
        public IActionResult Edit(MachineAvailability availability)
        {
            if (!ModelState.IsValid)
            {
                LoadMachines();
                return View(availability);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE MachineAvailability SET
                                    MachineId=@MachineId,
                                    AvailabilityStatus=@AvailabilityStatus,
                                    AvailableFrom=@AvailableFrom,
                                    AvailableTo=@AvailableTo,
                                    CurrentStatus=@CurrentStatus,
                                    Remarks=@Remarks
                                 WHERE AvailabilityId=@AvailabilityId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@AvailabilityId",
                    availability.AvailabilityId);

                cmd.Parameters.AddWithValue(
                    "@MachineId",
                    availability.MachineId);

                cmd.Parameters.AddWithValue(
                    "@AvailabilityStatus",
                    availability.AvailabilityStatus ?? "");

                cmd.Parameters.AddWithValue(
                    "@AvailableFrom",
                    availability.AvailableFrom);

                cmd.Parameters.AddWithValue(
                    "@AvailableTo",
                    (object?)availability.AvailableTo ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@CurrentStatus",
                    availability.CurrentStatus ?? "");

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    availability.Remarks ?? "");

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Machine Availability Updated Successfully.";

            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            MachineAvailability availability =
                new MachineAvailability();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT MA.*,
                                        M.MachineName
                                 FROM MachineAvailability MA
                                 INNER JOIN Machines M
                                    ON MA.MachineId = M.MachineId
                                 WHERE MA.AvailabilityId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    availability.AvailabilityId =
                        Convert.ToInt32(dr["AvailabilityId"]);

                    availability.MachineId =
                        Convert.ToInt32(dr["MachineId"]);

                    availability.AvailabilityStatus =
                        dr["AvailabilityStatus"].ToString();

                    availability.AvailableFrom =
                        Convert.ToDateTime(dr["AvailableFrom"]);

                    availability.AvailableTo =
                        dr["AvailableTo"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["AvailableTo"]);

                    availability.CurrentStatus =
                        dr["CurrentStatus"].ToString();

                    availability.Remarks =
                        dr["Remarks"].ToString();

                    availability.CreatedDate =
                        Convert.ToDateTime(dr["CreatedDate"]);

                    availability.MachineName =
                        dr["MachineName"].ToString();
                }

                con.Close();
            }

            return View(availability);
        }


        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query =
                    "DELETE FROM MachineAvailability WHERE AvailabilityId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            TempData["Success"] =
                "Machine Availability Deleted Successfully.";

            return RedirectToAction("Index");
        }
    }
}
