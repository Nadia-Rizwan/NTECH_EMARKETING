using Emarketing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using PagedList;
using System.Web.Security;

namespace Emarketing.Controllers
{

    public class AdminController : Controller
    {

        EmarketingDBEntities objemarketingDBEntities = new EmarketingDBEntities();
        // GET: Admin
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Admin objadmin)
        {

            Admin admin = objemarketingDBEntities.Admins.Where(x => x.AdminUsername == objadmin.AdminUsername && x.AdminPassword == objadmin.AdminPassword).SingleOrDefault();
            if(admin != null)
            {

                Session["AdminID"] = admin.AdminID.ToString();

                FormsAuthentication.SetAuthCookie(objadmin.AdminUsername, false);
                var authticket = new FormsAuthenticationTicket(1, admin.AdminUsername, DateTime.Now, DateTime.Now.AddMinutes(20), false, "Admin");
                string encryptTicket = FormsAuthentication.Encrypt(authticket);
                var authcookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptTicket);
                HttpContext.Response.Cookies.Add(authcookie);


                return RedirectToAction("Create");
            }
            else
            {
                ViewBag.error = "invalid username or pssword";
            }
            
            return View();
        }

        public ActionResult Create()
        {
            if (Session["AdminID"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(Category bjcategory,HttpPostedFileBase CategoryImage)
        {
            string path = UploadFile(CategoryImage);
            if (path.Equals("-1"))
            {
                ViewBag.error = "image could not uploaded";
                
            }
            else
            {
                Category category = new Category();
                category.CategoryName = bjcategory.CategoryName;
                category.CategoryImage = path;
                category.Status =1;
                category.FK_Admin = Convert.ToInt32(Session["AdminID"].ToString());
                objemarketingDBEntities.Categories.Add(category);
                objemarketingDBEntities.SaveChanges();
                return RedirectToAction("ViewCategories");

            }
            return View();
        }

        public ActionResult ViewCategories(int?page)
        {
            int pagesize = 7, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = objemarketingDBEntities.Categories.Where(x => x.Status == 1).OrderByDescending(x => x.CategoryID).ToList();
            IPagedList<Category> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);
        }


        public string UploadFile(HttpPostedFileBase file) {

            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file !=null && file.ContentLength > 0)
            {
                string extention = Path.GetExtension(file.FileName);
                if (extention.ToLower().Equals(".jpg") || extention.ToLower().Equals(".jpeg") || extention.ToLower().Equals(".png"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);
                    }
                    catch(Exception ex)
                    {

                        path = "-1";

                    }
                }
                else
                {

                   
                    Response.Write("<script>alert('Only jpeg,jpg or png formates are acceptable...'); </script>");

                    path = "-1";

                }
               

            }
            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }
            return path;


        }



    }
}