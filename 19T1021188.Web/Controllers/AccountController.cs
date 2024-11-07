using _19T1021188.BusinessLayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace _19T1021188.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index1()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Trang đăng nhập vào hệ thống
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string userName = "", string password = "")
        {
            ViewBag.UserName = userName;
            
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Thông tin không đầy đủ!");
                return View();
            }

            var userAccount = UserAccountService.Authorize(AccountTypes.Employee, userName, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("", "Đăng nhập thất bại!Tài khoản hoặc mật khẩu sai!");
                return View();
            }

            //Ghi cookie cho phiên đăng nhập
            string cookieString = Newtonsoft.Json.JsonConvert.SerializeObject(userAccount);
            FormsAuthentication.SetAuthCookie(cookieString, false);// false: không lưu

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult RePassword(string userName = "")
        {
            ViewBag.userName = userName;
            return View();
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult RePassword(string oldPassword = "",string userName = "", string newPassword = "", string Renewpassword = "")
        {
            ViewBag.userName = userName;

            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(Renewpassword))
            {
                ModelState.AddModelError("", "Thông tin không đầy đủ!");
                return View();
            }

            if(newPassword != Renewpassword)
            {
                ModelState.AddModelError("", "Nhập lại mật khẩu sai!");
                return View();
            }

            var userAccount = UserAccountService.ChangPassword(AccountTypes.Employee, userName, oldPassword, newPassword);
            if (userAccount == false)
            {
                ModelState.AddModelError("", "Đăng nhập thất bại!Tài khoản hoặc mật khẩu sai!");
                return View();
            }

            
            //Ghi cookie cho phiên đăng nhập

            return RedirectToAction("Index", "Home");
        }
    }
}