using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingVenueController : Controller
    {
        #region MeetingVenue List
        public ActionResult<List<MeetingVenueModel>> MeetingVenueList()
        {
            List<MeetingVenueModel> list = new List<MeetingVenueModel>();

            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingVenue_SELECTALL";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                MeetingVenueModel mv = new MeetingVenueModel();
                mv.MeetingVenueID = Convert.ToInt32(reader["MeetingVenueID"]);
                mv.MeetingVenueName = reader["MeetingVenueName"].ToString();

                list.Add(mv);
            }

            reader.Close();

            con.Close();

            return View("MeetingVenueList", list);
        }
        #endregion

        #region MeetingVenueAddEdit
        [HttpGet]
        public IActionResult MeetingVenueAddEdit(int? id)
        {
            if (id > 0)
            {
                MeetingVenueModel meetingVenueModel = GetMeetingVenueById(id.Value);
                return View(meetingVenueModel);
            }
            else
            {
                return View(new MeetingVenueModel());
            }
        }
        #endregion

        #region Save
        public IActionResult Save(MeetingVenueModel model)
        {
            ModelState.Remove("MeetingVenueName");
            if (string.IsNullOrEmpty(model.MeetingVenueName))
            {
                ModelState.AddModelError("MeetingVenueName", "Meeting Venue name can not be null or empty.");
            }
            try
            {
                model.Created = DateTime.UtcNow;
                model.Modified = DateTime.UtcNow;
                if (!ModelState.IsValid)
                {
                    return View("MeetingVenueAddEdit", model);
                }

                SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                if (model.MeetingVenueID == 0)
                {
                    cmd.CommandText = "PR_MOM_MeetingVenue_INSERT";
                    cmd.Parameters.AddWithValue("MeetingVenueName", model.MeetingVenueName);
                    cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Modified", DateTime.Now);
                }
                else
                {
                    cmd.CommandText = "PR_MOM_MeetingVenue_UPDATEBYPK";
                    cmd.Parameters.AddWithValue("MeetingVenueID", model.MeetingVenueID);
                    cmd.Parameters.AddWithValue("MeetingVenueName", model.MeetingVenueName);
                    
                    cmd.Parameters.AddWithValue("@Modified", DateTime.Now);
                }
                cmd.CommandType = CommandType.StoredProcedure;

              

                con.Open();
                int noOfRows = cmd.ExecuteNonQuery();
                con.Close();

                if (noOfRows > 0)
                {
                    if (model.MeetingVenueID == 0)
                        TempData["Success"] = "Record Inserted Successfully.";
                    else
                        TempData["Success"] = "Record Updated Successfully.";
                }

              
                return RedirectToAction("MeetingVenueList");

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("MeetingVenueList");

            }
        }

        #endregion

        #region GetMeetingVenueById
        public MeetingVenueModel GetMeetingVenueById(int id)
        {
            MeetingVenueModel meetingVenueModel = new MeetingVenueModel();

            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand("PR_MOM_MeetingVenue_SELECTBYPK", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MeetingVenueID", id);

          
            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                meetingVenueModel.MeetingVenueID = Convert.ToInt32(reader["MeetingVenueID"]);
                meetingVenueModel.MeetingVenueName = reader["MeetingVenueName"].ToString();
               
            }

            reader.Close();
            con.Close();

            return meetingVenueModel;
        }
        #endregion

        #region Delete
        public IActionResult Delete(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_MeetingVenue_DELETEBYPK";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@MeetingVenueID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                TempData["Success"] = "Delete Successfully";

                return RedirectToAction("MeetingVenueList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("MeetingVenueList");
            }
        }
        #endregion
    }
}