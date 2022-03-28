using Emarketing.Models;
using Emarketing.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Emarketing.Controllers
{
    public class UserController : Controller
    {

        EmarketingDBEntities objemarketingDBEntities = new EmarketingDBEntities();
        // GET: User
        public ActionResult Index(int ?page)
        {
            int pagesize = 7, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = objemarketingDBEntities.Categories.Where(x => x.Status == 1).OrderByDescending(x => x.CategoryID).ToList();
            IPagedList<Category> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);
        }


        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(tblUser tblUser, HttpPostedFileBase CategoryImage)
        {
            string path = UploadFile(CategoryImage);
            if (path.Equals("-1"))
            {
                ViewBag.error = "image could not uploaded";

            }
            else
            {
                tblUser user = new tblUser();
                user.UserName = tblUser.UserName;
                user.UserEmail = tblUser.UserEmail;
                user.UserContact = tblUser.UserContact;
                user.UserPassword = tblUser.UserPassword;
                user.UserImage = path;

                objemarketingDBEntities.tblUsers.Add(user);
                objemarketingDBEntities.SaveChanges();
                return RedirectToAction("Login");



            }


                return View();
        }



        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(tblUser objtblUser)
        {

            tblUser user = objemarketingDBEntities.tblUsers.Where(x => x.UserName == objtblUser.UserName && x.UserPassword == objtblUser.UserPassword).SingleOrDefault();
            if (user != null)
            {

                Session["UserID"] = user.UserID.ToString();

                //FormsAuthentication.SetAuthCookie(objadmin.AdminUsername, false);
                //var authticket = new FormsAuthenticationTicket(1, admin.AdminUsername, DateTime.Now, DateTime.Now.AddMinutes(20), false, "Admin");
                //string encryptTicket = FormsAuthentication.Encrypt(authticket);
                //var authcookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptTicket);
                //HttpContext.Response.Cookies.Add(authcookie);


                return RedirectToAction("CreatADD");
            }
            else
            {
                ViewBag.error = "invalid username or pssword";
            }

            return View();
        }


        public ActionResult CreatADD()
        {
            List<Category> li = objemarketingDBEntities.Categories.ToList();
            ViewBag.categorylist = new SelectList(li, "CategoryID", "CategoryName");


            return View();
        }

        [HttpPost]
        public ActionResult CreatADD(Product objproduct, HttpPostedFileBase CategoryImage)
        {
            string path = UploadFile(CategoryImage);
            if (path.Equals("-1"))
            {
                ViewBag.error = "image could not uploaded";

            }
            else
            {
                Product product = new Product();

                product.ProductName = objproduct.ProductName;
                product.ProductPrice = objproduct.ProductPrice;
                product.ProductImage = path;
                product.ProductDiscription = objproduct.ProductDiscription;
                product.FK_Category = objproduct.FK_Category;
                product.FK_User =Convert.ToInt32(Session["UserID"].ToString());
                objemarketingDBEntities.Products.Add(product);
                objemarketingDBEntities.SaveChanges();
                Response.Redirect("Index");





            }



                return View();
        }


        public ActionResult DisplayAdd(int ?id, int ? page)
        {

            int pagesize = 7, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = objemarketingDBEntities.Products.Where(x => x.FK_Category == id).OrderByDescending(x => x.ProductID).ToList();
            IPagedList<Product> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);


        }

        [HttpPost]
        public ActionResult DisplayAdd(int? id, int? page,string search)
        {

            int pagesize = 7, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = objemarketingDBEntities.Products.Where(x => x.ProductName.Contains(search)).OrderByDescending(x => x.ProductID).ToList();
            IPagedList<Product> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);


        }


        public ActionResult ViewAdds(int? id)
        {
            AddViewModel addViewModel = new AddViewModel();

            Product p = objemarketingDBEntities.Products.Where(x => x.ProductID == id).SingleOrDefault();
            addViewModel.ProductID = p.ProductID;
            addViewModel.ProductName = p.ProductName;
            addViewModel.ProductImage = p.ProductImage;
            addViewModel.ProductPrice = p.ProductPrice;
            addViewModel.ProductDiscription = p.ProductDiscription;
            Category cat = objemarketingDBEntities.Categories.Where(x => x.CategoryID == p.FK_Category).SingleOrDefault();
            addViewModel.CategoryName = cat.CategoryName;
            tblUser u = objemarketingDBEntities.tblUsers.Where(x => x.UserID == p.FK_User).SingleOrDefault();
            addViewModel.UserName = u.UserName;
            addViewModel.UserImage = u.UserImage;
            addViewModel.UserContact = u.UserContact;
            addViewModel.FK_User = u.UserID;

            return View(addViewModel);        
        
        
        
        
        }
        
        public ActionResult DeleteAd(int? id)
        {
            Product product = objemarketingDBEntities.Products.Where(x => x.ProductID == id).SingleOrDefault();
            objemarketingDBEntities.Products.Remove(product);
            objemarketingDBEntities.SaveChanges();
            return View("Index");
        }


        public ActionResult Signout()
        {
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index");
        
        }   

            public string UploadFile(HttpPostedFileBase file)
        {

            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
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
                    catch (Exception ex)
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