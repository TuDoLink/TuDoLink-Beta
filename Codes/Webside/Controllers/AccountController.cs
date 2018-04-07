using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TudolinkWeb.Service;
using TudolinkWeb.Models;
using NPoco;
using System.Security.Cryptography;
using System.Text;
using TudolinkWeb.Utils;

namespace TudolinkWeb.Controllers
{
    public class AccountController : Controller
    {
        private SQLiteDababase db = new SQLiteDababase();

        private TB_User CurrentUserInfo
        {
            get
            {
                return Session["UserInfo"] as TB_User;
            }
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string accountEmail, string accountPassword)
        {
            using (var db = new Database("DefaultConnection"))
            {
                Result result = new Result();
                result.success = false;
                var user = db.FirstOrDefault<TB_User>("where Email=@0", accountEmail);
                if (user == null)
                {
                    result.msg = ("User name does not exist.");
                    result.success = false;
                    return Json(result);
                }
                if (user.Password != accountPassword)
                {
                    result.msg = ("The username or password is incorrect.");
                    result.success = false;
                    return Json(result);
                }
                if (!user.EmailValid)
                {
                    result.msg = ("The Email unverified, Please Verify your email.");
                    result.success = false;
                    return Json(result);
                }
                Session["UserInfo"] = user;

                result.success = true;
                result.msg = ("/Account/Index");
                return Json(result);
            }
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>    
        [HttpPost]
        public ActionResult Create(string accountEmail, string accountPassword)
        {
            using (var db = new Database("DefaultConnection"))
            {
                var user = db.FirstOrDefault<TB_User>("where Email=@0", accountEmail);
                if (user != null)
                {
                    return Content("The email is registered!");
                }
                user = new TB_User();
                user.UserName = accountEmail;
                user.Email = accountEmail;
                user.Password = accountPassword;

                if (string.IsNullOrEmpty(recommendBy))
                {
                    if (Session["recommendBy"] != null)
                    {
                        recommendBy = Session["recommendBy"].ToString();
                    }
                }
                if (!string.IsNullOrEmpty(recommendBy))
                {
                    var recommend = db.FirstOrDefault<TB_User>("where Email=@0 Or Id=@0", recommendBy);
                    if (recommend != null)
                    {
                        user.RecommendBy = recommend.Id;
                    }
                }

                user.CreateAt = DateTime.Now;
                user.Insert();

                Session["UserInfo"] = user;
                var url = Request.Url.ToString().Replace(Request.Url.PathAndQuery, "");

                VerifyUtil.SendEmailValidCode(user.Email, url);

                return RedirectToAction("ValidEmail");
            }
        }


        /// <summary>
        /// 验证邮箱是否存在
        /// </summary>
        /// <returns></returns>    
        [HttpPost]
        public ActionResult ValidName(string email)
        {
            using (var db = new Database("DefaultConnection"))
            {
                var user = db.FirstOrDefault<TB_User>("where Email=@0", email);
                if (user != null)
                {
                    return Json(new { valid = false });
                }
                return Json(new { valid = true });
            }
        }

        public ActionResult ValidEmail(string code = "")
        {

            if (string.IsNullOrEmpty(code))
            {
                if (CurrentUserInfo == null)//无用户信息
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    Session["UserInfo"] = db.FirstOrDefault<TB_User>("where Id=@0", CurrentUserInfo.Id);

                    if (CurrentUserInfo.EmailValid)//刷新页面，重新获取用户信息，已验证邮箱则跳回首页。
                    {
                        return Redirect("/Home/Index");
                    }
                }
                return View();
            }
            else
            {

                var message = "";
                TB_User user = null;
                if (!VerifyUtil.CheckEmailVerfyCode(code, out user, out message))
                {
                    return Content(message);
                }

                Session["UserInfo"] = user;

                return RedirectToAction("Recharge");
            }
        }


        [HttpPost]
        public ActionResult ValidEmail()
        {
            if (CurrentUserInfo == null)
            {
                return RedirectToAction("Login");
            }
            Result data = new Result();
            try
            {
                var url = Request.Url.ToString().TrimEnd(Request.Url.PathAndQuery.ToArray());
                data.success = VerifyUtil.SendEmailValidCode(CurrentUserInfo.Email, url);
            }
            catch (Exception ex)
            {
                data.success = false;
                data.msg = "Error sending mail!Please contact the administrator.";
            }
            return Json(data);
        }


        public ActionResult Valided()
        {
            return View();
        }

        /// <summary>
        /// 充值
        /// </summary>
        /// <returns></returns>
        public ActionResult Recharge()
        {
            if (CurrentUserInfo == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        public ActionResult RechargeResult()
        {
            if (CurrentUserInfo == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Rechange(string telegramHandle, string ethAddress)
        {
            if (CurrentUserInfo == null)
            {
                return RedirectToAction("Login");
            }
            using (var db = new SQLiteDababase())
            {
                TB_Recharge tB_Recharge = new TB_Recharge();
                tB_Recharge.UserId = CurrentUserInfo.Id;
                tB_Recharge.TelegramHandle = telegramHandle;
                tB_Recharge.DesiredContribution = desiredContribution;
                tB_Recharge.ETHaddress = ethAddress;
                tB_Recharge.CreateAt = DateTime.Now;
                tB_Recharge.Insert();
                return Redirect("/Account/RechargeResult");
            }
        }



    }
}
