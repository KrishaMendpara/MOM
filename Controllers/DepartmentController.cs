using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class DepartmentController : Controller
    {
        #region Department List
        public ActionResult<List<DepartmentModel>> Index()
        {
            List<DepartmentModel> list = new List<DepartmentModel>();

            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Department_SELECTALL";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                DepartmentModel d = new DepartmentModel();
                d.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                d.DepartmentName = reader["DepartmentName"].ToString();

                list.Add(d);
            }

            reader.Close();
            con.Close();

            return View("DepartmentList", list);
        }
        #endregion

        #region DepartmentAddEdit
        [HttpGet]

        public IActionResult DepartmentAddEdit(int? id)
        {
            if (id > 0)
            {
                DepartmentModel department = GetDepartmentById(id.Value);
                return View(department);
            }
            else
            {
                return View(new DepartmentModel());
            }
        }
        #endregion

        #region GetDepartmentById
        public DepartmentModel GetDepartmentById(int id)
        {
            DepartmentModel department = new DepartmentModel();

            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand("PR_MOM_Department_SELECTBYPK", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DepartmentID", id);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                department.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                department.DepartmentName = reader["DepartmentName"].ToString();
            }

            reader.Close();
            con.Close();

            return department;
        }
        #endregion

        #region Save

        [HttpPost]
        public IActionResult Save(DepartmentModel model)
        {
            ModelState.Remove("DepartmentName");
            if (string.IsNullOrEmpty(model.DepartmentName))
            {
                ModelState.AddModelError("DepartmentName", "Department name can not be null or empty.");
            }

            try
            {
                model.Created = DateTime.UtcNow;
                model.Modified = DateTime.UtcNow;
                if (!ModelState.IsValid)
                {
                    return View("DepartmentAddEdit", model);
                }

                SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                if (model.DepartmentID==0)
                {
                    cmd.CommandText = "PR_MOM_Department_INSERT";
                    cmd.Parameters.AddWithValue("DepartmentName", model.DepartmentName);
                    //cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                    //cmd.Parameters.AddWithValue("@Modified", DateTime.Now);
                }
                else
                {
                    cmd.CommandText = "PR_MOM_Department_UPDATEBYPK";
                    cmd.Parameters.AddWithValue("DepartmentID", model.DepartmentID);
                    cmd.Parameters.AddWithValue("DepartmentName", model.DepartmentName);
                  
                    //cmd.Parameters.AddWithValue("@Modified", DateTime.Now);
                }
               
                cmd.CommandType = CommandType.StoredProcedure;

               

                con.Open();
                int noOfRows = cmd.ExecuteNonQuery();
                //con.Close();

                if (noOfRows > 0)
                {
                    if (model.DepartmentID == 0)
                        TempData["Success"] = "Record Inserted Successfully.";
                    else
                        TempData["Success"] = "Record Updated Successfully.";
                }
                con.Close() ;
              
                return RedirectToAction("Index");

            }

            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");

            }
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
                cmd.CommandText = "PR_MOM_Department_DELETEBYPK";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@DepartmentID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                TempData["Success"] = "Delete Successfully";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Error"] = "Forign key constraint violated";
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Search

        [HttpPost]
        public IActionResult Index(IFormCollection formData)
        {
            string searchText = formData["SearchText"].ToString();

            if (string.IsNullOrWhiteSpace(searchText))
                searchText = null;

            ViewBag.SearchText = searchText;

            List<DepartmentModel> list = GetDepartments(searchText);
            return View("DepartmentList", list);
        }

        private List<DepartmentModel> GetDepartments(string searchText)
        {
            List<DepartmentModel> list = new List<DepartmentModel>();

            SqlConnection con = new SqlConnection("Server=DESKTOP-HUDL387\\SQLEXPRESS;Database=DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Department_SELECTALL";
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(searchText))
            {
                cmd.Parameters.AddWithValue("@SearchText", searchText);
            }

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                DepartmentModel d = new DepartmentModel();
                d.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                d.DepartmentName = reader["DepartmentName"].ToString();

                list.Add(d);
            }

            reader.Close();
            con.Close();

            return list;
        }

        #endregion
    }
}