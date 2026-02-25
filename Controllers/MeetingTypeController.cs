using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Data;
using System.Reflection;

namespace MOM.Controllers
{
    public class MeetingTypeController : Controller
    {
        #region MeetingType List
        public ActionResult<List<MeetingTypeModel>> MeetingTypeList()
        {
            List<MeetingTypeModel> list = new List<MeetingTypeModel>();
            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingType_SELECTALL";
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            { 
                MeetingTypeModel m = new MeetingTypeModel();
                m.MeetingTypeID = Convert.ToInt32(reader["MeetingTypeID"]);
                m.MeetingTypeName = reader["MeetingTypeName"].ToString();
                m.Remarks = reader["Remarks"].ToString();
                //m.Created = Convert.ToDateTime(reader["Created"]);
                list.Add(m);
            }
            reader.Close();
            con.Close();

            return View("MeetingTypeList",list);
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
                cmd.CommandText = "PR_MOM_MeetingType_DELETEBYPK";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@MeetingTypeID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                TempData["Success"] = "Delete Successfully";

                return RedirectToAction("MeetingTypeList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("MeetingTypeList");
            }

        }
        #endregion

        #region MeetingTypeAddEdit
        [HttpGet]
        public IActionResult MeetingTypeAddEdit(int? id)
        {
            if (id > 0)
            {
                MeetingTypeModel meetingTypeModel = GetMeetingTypeById(id.Value);
            
                return View(meetingTypeModel);

            }
            else
            {
                return View(new MeetingTypeModel());
            }
        }
        #endregion

        #region Save
        [HttpPost]
        public IActionResult Save(MeetingTypeModel model)
        {
            ModelState.Remove("MeetingTypeName");
            if (string.IsNullOrEmpty(model.MeetingTypeName))
            {
                ModelState.AddModelError("MeetingTypeName", "Meeting Type name can not be null or empty.");
            }
            try
            {
                model.Created = DateTime.UtcNow;
                model.Modified = DateTime.UtcNow;
                if (!ModelState.IsValid)
                {
                    return View("MeetingTypeAddEdit", model);
                }

                SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                
                cmd.CommandType = CommandType.StoredProcedure;

                 if (model.MeetingTypeID == 0)
        {
            
            cmd.CommandText = "PR_MOM_MeetingType_INSERT";

            cmd.Parameters.AddWithValue("@MeetingTypeName", model.MeetingTypeName);
            cmd.Parameters.AddWithValue("@Remarks", model.Remarks);
            cmd.Parameters.AddWithValue("@Created", DateTime.Now);
            cmd.Parameters.AddWithValue("@Modified", DateTime.Now); 
        }
        else
        {
            
            cmd.CommandText = "PR_MOM_MeetingType_UPDATEBYPK";

            cmd.Parameters.AddWithValue("@MeetingTypeID", model.MeetingTypeID);
            cmd.Parameters.AddWithValue("@MeetingTypeName", model.MeetingTypeName);
            cmd.Parameters.AddWithValue("@Remarks", model.Remarks);


        }
                con.Open();
                int noOfRows = cmd.ExecuteNonQuery();
                con.Close();

                if (noOfRows > 0)
                {
                    if (model.MeetingTypeID == 0)
                        TempData["Success"] = "Record Inserted Successfully.";
                    else
                        TempData["Success"] = "Record Updated Successfully.";
                }
                return RedirectToAction("MeetingTypeList");

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("MeetingTypeList");

            }
        }
        #endregion

        #region GetMeetingTypeById
        public MeetingTypeModel GetMeetingTypeById(int id)
        {
            MeetingTypeModel meetingTypeModel = new MeetingTypeModel();

            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand("PR_MOM_MeetingType_SELECTBYPK", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MeetingTypeID", id);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                meetingTypeModel.MeetingTypeID = Convert.ToInt32(reader["MeetingTypeID"]);
                meetingTypeModel.MeetingTypeName = reader["MeetingTypeName"].ToString();
                meetingTypeModel.Remarks = reader["Remarks"].ToString();
                meetingTypeModel.Created = Convert.ToDateTime(reader["Created"]);

            }

            reader.Close();
            con.Close();

            return meetingTypeModel;
        }
        #endregion
    }
}
