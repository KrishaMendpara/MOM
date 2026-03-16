using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingsController : Controller
    {
        #region Meetings List
        public ActionResult<List<MeetingsModel>> MeetingsList()
        {
            List<MeetingsModel> list = new List<MeetingsModel>();

            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Meetings_SELECTALL";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                MeetingsModel ms = new MeetingsModel();
                ms.MeetingID = Convert.ToInt32(reader["MeetingID"]);
                ms.MeetingTypeID = Convert.ToInt32(reader["MeetingTypeID"]);
                ms.MeetingDate = Convert.ToDateTime(reader["MeetingDate"]);
                ms.MeetingDescription = reader["MeetingDescription"].ToString();
                ms.DocumentPath = reader["DocumentPath"].ToString();
                ms.IsCancelled = Convert.ToBoolean(reader["IsCancelled"]);
                ms.CancellationReason = reader["CancellationReason"].ToString();
                list.Add(ms);
            }

            reader.Close();
            con.Close();

            return View("MeetingsList",list);

          
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
                cmd.CommandText = "PR_MOM_Meetings_DELETEBYPK";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@MeetingID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                TempData["Success"] = "Delete Successfully";
                return RedirectToAction("MeetingsList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("MeetingsList");
            }
        }
        #endregion

        #region Meetings Add Edit
        [HttpGet]
        public IActionResult MeetingsAddEdit(int? id)
        {
            ViewBag.DepartmentList = FillDepartmentDropDown();
            ViewBag.MeetingTypeList = FillMeetingTypeDropDown();
            ViewBag.Venuelist = FillMeetingVenueDropDown();

            if (id > 0)
            {
                MeetingsModel meetings = GetMeetingsById(id.Value);
                return View(meetings);
            }
            else
            {
                return View(new MeetingsModel());
            }
        }
        #endregion

        #region Get Meetings By Id
        public MeetingsModel GetMeetingsById(int id)
        {
            MeetingsModel meetings = new MeetingsModel();

            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand("PR_MOM_Meetings_SELECTBYPK", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MeetingID", id);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                meetings.MeetingID = Convert.ToInt32(reader["MeetingID"]);
                meetings.MeetingDate = Convert.ToDateTime(reader["MeetingDate"]);
                meetings.MeetingDescription = reader["MeetingDescription"].ToString();
                meetings.DocumentPath = reader["DocumentPath"].ToString();
                meetings.IsCancelled = Convert.ToBoolean(reader["IsCancelled"]);
                meetings.CancellationDateTime = reader["CancellationDateTime"] == DBNull.Value
                ? (DateTime?)null : Convert.ToDateTime(reader["CancellationDateTime"]);

                meetings.CancellationReason = reader["CancellationReason"].ToString();
                meetings.MeetingVenueID = Convert.ToInt32(reader["MeetingVenueID"]);
                meetings.MeetingTypeID = Convert.ToInt32(reader["MeetingTypeID"]);
                meetings.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);

            }

            reader.Close();
            con.Close();

            return meetings;
        }
        #endregion

        #region Save
        public IActionResult Save(MeetingsModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.DepartmentList = FillDepartmentDropDown();
                    ViewBag.MeetingTypeList = FillMeetingTypeDropDown();
                    ViewBag.Venuelist = FillMeetingVenueDropDown();
                    return View("MeetingsAddEdit", model);
                }

                using (SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;"))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (model.MeetingID == 0)
                        {
                            cmd.CommandText = "PR_MOM_Meetings_Insert";
                        }
                        else
                        {
                            cmd.CommandText = "PR_MOM_Meetings_UpdateByPk";
                            cmd.Parameters.AddWithValue("@MeetingID", model.MeetingID);
                        }

                        cmd.Parameters.AddWithValue("@MeetingDate", model.MeetingDate);
                        cmd.Parameters.AddWithValue("@MeetingVenueID", model.MeetingVenueID);
                        cmd.Parameters.AddWithValue("@MeetingTypeID", model.MeetingTypeID);
                        cmd.Parameters.AddWithValue("@DepartmentID", model.DepartmentID);
                        cmd.Parameters.AddWithValue("@MeetingDescription", model.MeetingDescription ?? "");
                        cmd.Parameters.AddWithValue("@DocumentPath", model.DocumentPath ?? "");
                        cmd.Parameters.AddWithValue("@IsCancelled", model.IsCancelled ?? false);
                        cmd.Parameters.AddWithValue("@CancellationDateTime",
                            model.CancellationDateTime == null ? DBNull.Value : model.CancellationDateTime);

                        cmd.Parameters.AddWithValue("@CancellationReason", model.CancellationReason ?? "");

                        con.Open();
                        int noOfRows = cmd.ExecuteNonQuery();
                        con.Close();

                        if (noOfRows > 0)
                        {
                            TempData["Success"] = model.MeetingID == 0 ? "Record Inserted" : "Record Updated";
                        }
                    }
                }

                return RedirectToAction("MeetingsList");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("MeetingsList");
            }
        }
        #endregion

        #region Search

        [HttpPost]
        public IActionResult MeetingsList(IFormCollection formData)
        {
            string searchText = formData["SearchText"].ToString();

            if (string.IsNullOrWhiteSpace(searchText))
                searchText = null;

            ViewBag.SearchText = searchText;

            List<MeetingsModel> list = GetMeetings(searchText);
            return View(list);
        }

        private List<MeetingsModel> GetMeetings(string searchText)
        {
            List<MeetingsModel> list = new List<MeetingsModel>();

            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Meetings_SELECTALL";
            cmd.CommandType = CommandType.StoredProcedure;

            if (searchText != null)
                cmd.Parameters.AddWithValue("@SearchText", searchText);
            else
                cmd.Parameters.AddWithValue("@SearchText", DBNull.Value);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                MeetingsModel meetings = new MeetingsModel();
                meetings.MeetingID = Convert.ToInt32(reader["MeetingID"]);
                meetings.MeetingDate = Convert.ToDateTime(reader["MeetingDate"]);
                meetings.MeetingDescription = reader["MeetingDescription"].ToString();
                meetings.DocumentPath = reader["DocumentPath"].ToString();
                meetings.IsCancelled = reader["IsCancelled"] != DBNull.Value
                       && Convert.ToBoolean(reader["IsCancelled"]);

                meetings.CancellationDateTime = reader["CancellationDateTime"] != DBNull.Value
                                                ? Convert.ToDateTime(reader["CancellationDateTime"])
                                                : null;

                meetings.CancellationReason = reader["CancellationReason"].ToString();


                list.Add(meetings);
            }

            reader.Close();
            con.Close();

            return list;
        }

        #endregion


        #region FillDepartmentDropDown
        public List<SelectListItem> FillDepartmentDropDown()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Department_SELECTALL";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new SelectListItem(reader["DepartmentName"].ToString(), reader["DepartmentID"].ToString()));

            }
            reader.Close();
            con.Close();
            return list;
        }
        #endregion

        #region FillMeetingTypeDropDown
        public List<SelectListItem> FillMeetingTypeDropDown()
        {
            List<SelectListItem> Typelist = new List<SelectListItem>();

            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingType_SELECTALL";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Typelist.Add(new SelectListItem(reader["MeetingTypeName"].ToString(), reader["MeetingTypeID"].ToString()));

            }
            reader.Close();
            con.Close();
            return Typelist;
        }
        #endregion

        #region FillMeetingVenueDropDown
        public List<SelectListItem> FillMeetingVenueDropDown()
        {
            List<SelectListItem> Venuelist = new List<SelectListItem>();

            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingVenue_SELECTALL";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Venuelist.Add(new SelectListItem(reader["MeetingVenueName"].ToString(), reader["MeetingVenueID"].ToString()));

            }
            reader.Close();
            con.Close();
            return Venuelist;
        }
        #endregion

    }
}

