using Manufacturing.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Manufacturing.Controllers
{
    public class MachineController : Controller
    {
        string cs = "Data Source=DESKTOP-48DB0K2\\SQLEXPRESS;Initial Catalog=Manufacturing;Integrated Security=True";

        public IActionResult Index()
        {
            List<Machine> list= new List<Machine>();
            using(SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd= new SqlCommand("SELECT * FROM Machines ORDER BY MachineId DESC", con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while(dr.Read())
                        {
                            list.Add(new Machine()
                            {
                                MachineId = Convert.ToInt32(dr["MachineId"]),
                                MachineCode = dr["MachineCode"].ToString(),
                                MachineName = dr["MachineName"].ToString(),
                                MachineType = dr["MachineType"].ToString(),
                                Manufacturer = dr["Manufacturer"].ToString(),
                                PurchaseDate = dr["PurchaseDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["PurchaseDate"]),
                                LastMaintenanceDate = dr["LastMaintenanceDate"] == DBNull.Value? null : Convert.ToDateTime(dr["LastMaintenanceDate"]),
                                NextMaintenanceDate= dr["NextMaintenanceDate"] == DBNull.Value?null : Convert.ToDateTime(dr["NextMaintenanceDate"]),
                                Status = dr["Status"].ToString(),
                                AllocationStatus = dr["AllocationStatus"].ToString(),
                                AllocatedTo= dr["AllocatedTo"].ToString(),
                                ApprovedBy = dr["ApprovedBy"].ToString(),
                                Remarks = dr["Remarks"].ToString(),
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
        public IActionResult Create(Machine machine)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Machines
                (MachineCode,MachineName,MachineType,Manufacturer,
                PurchaseDate,LastMaintenanceDate,NextMaintenanceDate,
                Status,AllocationStatus,AllocatedTo,ApprovedBy,Remarks)

                VALUES
                (@MachineCode,@MachineName,@MachineType,@Manufacturer,
                @PurchaseDate,@LastMaintenanceDate,@NextMaintenanceDate,
                @Status,@AllocationStatus,@AllocatedTo,@ApprovedBy,@Remarks)", con))
                {
                    cmd.Parameters.AddWithValue("@MachineCode", machine.MachineCode);
                    cmd.Parameters.AddWithValue("@MachineName", machine.MachineName);
                    cmd.Parameters.AddWithValue("@MachineType", machine.MachineType);
                    cmd.Parameters.AddWithValue("@Manufacturer", machine.Manufacturer);
                    cmd.Parameters.AddWithValue("@PurchaseDate", (object?)machine.PurchaseDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastMaintenanceDate", (object?)machine.LastMaintenanceDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NextMaintenanceDate", (object?)machine.NextMaintenanceDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", machine.Status);
                    cmd.Parameters.AddWithValue("@AllocationStatus", machine.AllocationStatus);
                    cmd.Parameters.AddWithValue("@AllocatedTo", machine.AllocatedTo ?? "");
                    cmd.Parameters.AddWithValue("@ApprovedBy", machine.ApprovedBy ?? "");
                    cmd.Parameters.AddWithValue("@Remarks", machine.Remarks ?? "");

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Machine machine = new Machine();
            using (SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Machines WHERE MachineId=@MachineId", con))
                {
                    cmd.Parameters.AddWithValue("@MachineId", id);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader()) 
                    { 
                     if(dr.Read())
                        {
                            machine.MachineId = Convert.ToInt32(dr["MachineId"]);
                            machine.MachineCode = dr["MachineCode"].ToString();
                            machine.MachineName = dr["MachineName"].ToString();
                            machine.MachineType = dr["MachineType"].ToString();
                            machine.Manufacturer = dr["Manufacturer"].ToString();

                            machine.PurchaseDate = dr["PurchaseDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(dr["PurchaseDate"]);

                            machine.LastMaintenanceDate = dr["LastMaintenanceDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(dr["LastMaintenanceDate"]);

                            machine.NextMaintenanceDate = dr["NextMaintenanceDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(dr["NextMaintenanceDate"]);

                            machine.Status = dr["Status"].ToString();
                            machine.AllocationStatus = dr["AllocationStatus"].ToString();
                            machine.AllocatedTo = dr["AllocatedTo"].ToString();
                            machine.ApprovedBy = dr["ApprovedBy"].ToString();
                            machine.Remarks = dr["Remarks"].ToString();
                        }
                    }
                }
            }                return View(machine);
        }

        [HttpPost]
        public IActionResult Edit(Machine machine)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd = new SqlCommand(@"UPDATE Machines SET
                        MachineCode=@MachineCode,
                        MachineName=@MachineName,
                        MachineType=@MachineType,
                        Manufacturer=@Manufacturer,
                        PurchaseDate=@PurchaseDate,
                        LastMaintenanceDate=@LastMaintenanceDate,
                        NextMaintenanceDate=@NextMaintenanceDate,
                        Status=@Status,
                        AllocationStatus=@AllocationStatus,
                        AllocatedTo=@AllocatedTo,
                        ApprovedBy=@ApprovedBy,
                        Remarks=@Remarks
                        WHERE MachineId=@MachineId", con))
                {
                    cmd.Parameters.AddWithValue("@MachineId", machine.MachineId);
                    cmd.Parameters.AddWithValue("@MachineCode", machine.MachineCode);
                    cmd.Parameters.AddWithValue("@MachineName", machine.MachineName);
                    cmd.Parameters.AddWithValue("@MachineType", machine.MachineType ?? "");
                    cmd.Parameters.AddWithValue("@Manufacturer", machine.Manufacturer ?? "");
                    cmd.Parameters.AddWithValue("@PurchaseDate", (object?)machine.PurchaseDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastMaintenanceDate", (object?)machine.LastMaintenanceDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NextMaintenanceDate", (object?)machine.NextMaintenanceDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", machine.Status ?? "Available");
                    cmd.Parameters.AddWithValue("@AllocationStatus", machine.AllocationStatus ?? "Pending");
                    cmd.Parameters.AddWithValue("@AllocatedTo", machine.AllocatedTo ?? "");
                    cmd.Parameters.AddWithValue("@ApprovedBy", machine.ApprovedBy ?? "");
                    cmd.Parameters.AddWithValue("@Remarks", machine.Remarks ?? "");

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult ApproveAllocation(int id)
        {
            using(SqlConnection con = new SqlConnection(cs))
            {
                using(SqlCommand cmd= new SqlCommand("UPDATE Machines SET AllocationStatus='Approved' WHERE MachineId=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                      con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return View();
        }


        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Machines WHERE MachineId=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                    
            }

            return RedirectToAction("Index");
        }

        public IActionResult Status()
        {
            List<Machine> machines = new List<Machine>();
            using(SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Machines ORDER BY MachineName", con))
                {
                    con.Open();
                    using(SqlDataReader dr= cmd.ExecuteReader())
                    {
                        while(dr.Read())
                        {
                            machines.Add(new Machine()
                            {
                                MachineId = Convert.ToInt32(dr["MachineId"]),
                                MachineName = dr["MachineName"].ToString(),
                                MachineCode = dr["MachineCode"].ToString(),
                                MachineType = dr["MachineType"].ToString(),
                                Manufacturer = dr["Manufacturer"].ToString(),
                                PurchaseDate = dr["PurchaseDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["PurchaseDate"]),
                                LastMaintenanceDate = dr["LastMaintenanceDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["LastMaintenanceDate"]),
                                NextMaintenanceDate = dr["NextMaintenanceDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["NextMaintenanceDate"]),
                                Status = dr["Status"].ToString(),
                                AllocationStatus = dr["AllocationStatus"].ToString(),
                                AllocatedTo = dr["AllocatedTo"].ToString(),
                                ApprovedBy = dr["ApprovedBy"].ToString(),
                                Remarks = dr["Remarks"].ToString(),
                                CreatedDate = Convert.ToDateTime(dr["CreatedDate"]),
                            });
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}
