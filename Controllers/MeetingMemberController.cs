using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingMemberController : Controller
    {
        public ActionResult<List<MeetingMemberModel>> MeetingMemberList()
        {
            List<MeetingMemberModel> list = new List<MeetingMemberModel>();

            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingMember_SELECTALL";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                MeetingMemberModel mm = new MeetingMemberModel();
                mm.MeetingMemberID = Convert.ToInt32(reader["MeetingMemberID"]);
                mm.IsPresent = Convert.ToBoolean(reader["IsPresent"]);
                mm.Remarks = reader["Remarks"].ToString();

                list.Add(mm);
            }

            reader.Close();
            con.Close();

            return View("MeetingMemberList", list);
        }
        public IActionResult Delete(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_MeetingMember_DELETEBYPK";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@MeetingMemberID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("MeetingMemberList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("MeetingMemberList");
            }
        }
        public IActionResult MeetingMemberAddEdit()
        {
            return View();
        }
        public IActionResult Save(MeetingMemberModel model)
        {
          
            try
            {
                model.Created = DateTime.UtcNow;
                model.Modified = DateTime.UtcNow;
                if (!ModelState.IsValid)
                {
                    return View("MeetingMemberAddEdit", model);
                }

                SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_MeetingMember_INSERT";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter MID = new SqlParameter();
                MID.ParameterName = "@MeetingID";
                MID.SqlDbType = SqlDbType.Int;
                MID.Value = model.MeetingID;

                SqlParameter SID = new SqlParameter();
                SID.ParameterName = "@StaffID";
                SID.SqlDbType = SqlDbType.Int;
                SID.Value = model.StaffID;

                SqlParameter Isp = new SqlParameter();
                Isp.ParameterName = "@IsPresent";
                Isp.SqlDbType = SqlDbType.Bit;
                Isp.Value = model.IsPresent;

                SqlParameter remarks = new SqlParameter();
                remarks.ParameterName = "@Remarks";
                remarks.SqlDbType = SqlDbType.NVarChar;
                //remarks.Value = model.Remarks;
                remarks.Value = string.IsNullOrEmpty(model.Remarks)
                               ? DBNull.Value
                               : model.Remarks;

                SqlParameter created = new SqlParameter();
                created.ParameterName = "@Created";
                created.SqlDbType = SqlDbType.DateTime;
                created.Value = DateTime.Now;

                SqlParameter modified = new SqlParameter();
                modified.ParameterName = "@Modified";
                modified.SqlDbType = SqlDbType.DateTime;
                modified.Value = DateTime.Now;

                cmd.Parameters.Add(MID);
                cmd.Parameters.Add(SID);
                cmd.Parameters.Add(Isp);
                cmd.Parameters.Add(remarks);
                cmd.Parameters.Add(created);
                cmd.Parameters.Add(modified);


                con.Open();
                int noOfRows = cmd.ExecuteNonQuery();
                con.Close();

                if (noOfRows > 0)
                {
                    TempData["Success"] = "Record Inserted";
                }

                TempData["Success"] = "Meeting venue added successfully";
                return RedirectToAction("MeetingMemberList");

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("MeetingMemberList");

            }
        }
    }
}
