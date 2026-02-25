using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingsController : Controller
    {
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
                p.ParameterName = "@MeetingsID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("MeetingsList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("MeetingsList");
            }
        }
        public IActionResult MeetingsAddEdit()
        {
            ViewBag.DepartmentList = FillDepartmentDropDown();
            ViewBag.MeetingTypeList = FillMeetingTypeDropDown();
            ViewBag.Venuelist = FillMeetingVenueDropDown();
            return View();
        }
        public IActionResult Save(MeetingsModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.DepartmentList = FillDepartmentDropDown();
                    ViewBag.MeetingTypeList = FillMeetingTypeDropDown();
                    ViewBag.MeetingVenueList = FillMeetingVenueDropDown();
                    return View("MeetingsAddEdit", model);
                }


                model.Created = DateTime.UtcNow;
                model.Modified = DateTime.UtcNow;

                SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                if (model.MeetingsID == 0)
                {
                    cmd.CommandText = "PR_MOM_Meetings_Insert";
                }
                else
                {
                    cmd.CommandText = "PR_MOM_Meetings_UpdateByPk";
                    cmd.Parameters.AddWithValue("@MeetingID", model.MeetingsID);
                }

                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter dattime = new SqlParameter();
                dattime.ParameterName = "@MeetingDate";
                dattime.SqlDbType = SqlDbType.DateTime;
                dattime.Value = model.MeetingDate;

                SqlParameter MVId = new SqlParameter();
                MVId.ParameterName = "@MeetingVenueID";
                MVId.SqlDbType = SqlDbType.Int;
                MVId.Value = model.MeetingVenueID;

                SqlParameter MTId = new SqlParameter();
                MTId.ParameterName = "@MeetingTypeID";
                MTId.SqlDbType = SqlDbType.Int;
                MTId.Value = model.MeetingTypeID;

                SqlParameter deptId = new SqlParameter();
                deptId.ParameterName = "@DepartmentID";
                deptId.SqlDbType = SqlDbType.Int;
                deptId.Value = model.DepartmentID;

                SqlParameter MTDes = new SqlParameter();
                MTDes.ParameterName = "@MeetingDescription";
                MTDes.SqlDbType = SqlDbType.NVarChar;
                MTDes.Value = model.MeetingDescription;

                SqlParameter DocPath = new SqlParameter();
                DocPath.ParameterName = "@DocumentPath";
                DocPath.SqlDbType = SqlDbType.NVarChar;
                DocPath.Value = model.DocumentPath;

                SqlParameter created = new SqlParameter();
                created.ParameterName = "@Created";
                created.SqlDbType = SqlDbType.DateTime;
                created.Value = model.Created;

                SqlParameter modified = new SqlParameter();
                modified.ParameterName = "@Modified";
                modified.SqlDbType = SqlDbType.DateTime;
                modified.Value = model.Modified;

                SqlParameter IsCancel = new SqlParameter();
                IsCancel.ParameterName = "@IsCancelled";
                IsCancel.SqlDbType = SqlDbType.Bit;
                IsCancel.Value = model.IsCancelled;

                SqlParameter CancelTime = new SqlParameter();
                CancelTime.ParameterName = "@CancellationDateTime";
                CancelTime.SqlDbType = SqlDbType.DateTime;
                CancelTime.Value = model.CancellationDateTime;

                SqlParameter CancelReason = new SqlParameter();
                CancelReason.ParameterName = "@CancellationReason";
                CancelReason.SqlDbType = SqlDbType.NVarChar;
                CancelReason.Value = model.CancellationReason;

                cmd.Parameters.Add(dattime);
                cmd.Parameters.Add(MVId);
                cmd.Parameters.Add(MTId);
                cmd.Parameters.Add(deptId);
                cmd.Parameters.Add(MTDes);
                cmd.Parameters.Add(DocPath);
                cmd.Parameters.Add(created);
                cmd.Parameters.Add(modified);
                cmd.Parameters.Add(IsCancel);
                cmd.Parameters.Add(CancelTime);
                cmd.Parameters.Add(CancelReason);

                con.Open();

                int noOfRows = cmd.ExecuteNonQuery();

                if (noOfRows > 0)
                {
                    TempData["Success"] = "Record Inserted";
                }
                con.Close();


                return RedirectToAction("MeetingsList");

            }

            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("MeetingsList");

            }

        }
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

    }
}

