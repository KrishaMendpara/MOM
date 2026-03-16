using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class StaffController : Controller
    {


        #region Staff List
        public ActionResult<List<StaffModel>> StaffList()
        {
            List<StaffModel> list = new List<StaffModel>();

            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Staff_SELECTALL";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                StaffModel s = new StaffModel();
                s.StaffID = Convert.ToInt32(reader["StaffID"]);
                s.StaffName = reader["StaffName"].ToString();
                s.MobileNo = reader["MobileNo"].ToString();
                s.EmailAddress = reader["EmailAddress"].ToString();

                list.Add(s);
            }

            reader.Close();
            con.Close();

            return View(list);
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
                cmd.CommandText = "PR_MOM_Staff_DELETEBYPK";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@StaffID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                TempData["Success"] = "Delete Successfully";

                return RedirectToAction("StaffList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("StaffList");
            }
        }
        #endregion

        #region StaffAddEdit


        [HttpGet]
        public IActionResult StaffAddEdit(int? id)
        {
            
            ViewBag.DepartmentList = FillDepartmentDropDown();

            if (id > 0)
            {
                StaffModel staff = GetStaffById(id.Value);
                return View(staff);
            }
            else
            {
                return View(new StaffModel());
            }
        }
        #endregion

        #region GetStaffById
        public StaffModel GetStaffById(int id)
        {
            StaffModel staff = new StaffModel();

            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand("PR_MOM_Staff_SELECTBYPK", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StaffID", id);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                staff.StaffID = Convert.ToInt32(reader["StaffID"]);
                staff.StaffName = reader["StaffName"].ToString();
                staff.MobileNo = reader["MobileNo"].ToString();
                staff.EmailAddress = reader["EmailAddress"].ToString();
                staff.Remarks = reader["Remarks"].ToString();
                staff.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);

            }

            reader.Close();
            con.Close();
            
            return staff;
        }
        #endregion

        #region Save
        [HttpPost]
        public IActionResult Save(StaffModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.DepartmentList = FillDepartmentDropDown();
                    return View("StaffAddEdit", model);
                }

                model.Modified = DateTime.UtcNow;

                SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                if (model.StaffID == 0)
                {
                    cmd.CommandText = "PR_MOM_Staff_INSERT";
                    cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                    
                }
                else
                {
                    cmd.CommandText = "PR_MOM_Staff_UPDATEBYPK";
                    cmd.Parameters.AddWithValue("@StaffID", model.StaffID);
                   
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DepartmentID", model.DepartmentID);
                cmd.Parameters.AddWithValue("@StaffName", model.StaffName);
                cmd.Parameters.AddWithValue("@MobileNo", model.MobileNo);
                cmd.Parameters.AddWithValue("@EmailAddress", model.EmailAddress);
                cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Modified", model.Modified);   
                con.Open();

                int noOfRows = cmd.ExecuteNonQuery();

                con.Close();

                if (noOfRows > 0)
                {
                    if (model.StaffID == 0)
                        TempData["Success"] = "Record Inserted Successfully.";
                    else
                        TempData["Success"] = "Record Updated Successfully.";
                }

                return RedirectToAction("StaffList");

            }

            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("StaffList");

            }

        }
        #endregion

        #region Search

        [HttpPost]
        public IActionResult StaffList(IFormCollection formData)
        {
            string searchText = formData["SearchText"].ToString();

            if (string.IsNullOrWhiteSpace(searchText))
                searchText = null;

            ViewBag.SearchText = searchText;

            List<StaffModel> list = GetStaff(searchText);
            return View(list);
        }

        private List<StaffModel> GetStaff(string searchText)
        {
            List<StaffModel> list = new List<StaffModel>();

            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Staff_SELECTALL";
            cmd.CommandType = CommandType.StoredProcedure;

            if (searchText != null)
                cmd.Parameters.AddWithValue("@SearchText", searchText);
            else
                cmd.Parameters.AddWithValue("@SearchText", DBNull.Value);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                StaffModel s = new StaffModel();
                s.StaffID = Convert.ToInt32(reader["StaffID"]);
                s.StaffName = reader["StaffName"].ToString();
                s.MobileNo = reader["MobileNo"].ToString();
                s.EmailAddress = reader["EmailAddress"].ToString();
                s.Remarks = reader["Remarks"].ToString();

                list.Add(s);
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
    }
}
