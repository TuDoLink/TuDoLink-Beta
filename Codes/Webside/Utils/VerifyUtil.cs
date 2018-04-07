using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using TudolinkWeb.Models;
using TudolinkWeb.Service;

namespace TudolinkWeb.Utils
{
    /// <summary>
    /// 生成手机验证码
    /// </summary>
    public class VerifyUtil
    {

        static string key = "wwwroaderpcom";
        /// <summary>
        /// 获得随机数字字符
        /// </summary>
        /// <param name="len">字符长度</param>
        /// <returns></returns>
        public static string GetVerifyCode(int len = 6)
        {
            string str = string.Empty;
            Random r = new Random();
            for (int i = 0; i < 6; i++)
            {
                str += Convert.ToString(r.Next(0, 10));//6位随机数在0-9之间取，不合适自己改
            }
            return str;
        }

        public static bool SendEmailValidCode(string email, string url)
        {
            using (var db = new SQLiteDababase())
            {
                var validCode = db.FirstOrDefault<TB_ValidCode>("where Target=@0 order by CreateAt desc", email);
                if (validCode == null || validCode.CreateAt < DateTime.Now.AddMinutes(-15))
                {
                    validCode = new TB_ValidCode();
                    validCode.Target = email;
                    validCode.Code = GetVerifyCode(6);

                    validCode.CreateAt = DateTime.Now;

                    validCode.Insert();
                }
                var enCode = SecurityHelper.DESEncrypt(validCode.Code, key);

               
                
                return true;
            }
        }
  
        

        /// <summary>
        /// 校验邮箱验证码是否正确
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="code">手机验证码</param>
        /// <param name="message">返回信息(失效，超时...)</param>
        /// <param name="message">返回错误信息</param>
        /// <returns>是否正确</returns>
        public static bool CheckEmailVerfyCode(string code, out string message)
        {
            user = null;
            message = string.Empty;
            try
            {

                code = SecurityHelper.DESDecrypt(code, key);

                using (var db = new SQLiteDababase())
                {
                    var info = db.FirstOrDefault<TB_ValidCode>("where Code=@0", code);
                    if (info != null)
                    {

                        if (info.CreateAt.Subtract(DateTime.Now).Minutes > 20)
                        {
                            message = "Verification code failure，Please get it again";
                        }
                        else
                        {
                            user = db.FirstOrDefault<TB_User>("where Email=@0", info.Target);
                            if (!user.EmailValid)
                            {
                                user.EmailValid = true;
                                user.EmailValidAt = DateTime.Now;
                                user.Update();
                            }
                            return true;
                        }
                    }
                    else
                    {
                        message = "Verifying code is wrong";
                        return false;
                    }
                }


            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return false;
        }

*/


        /// <summary>
        /// 校验验证码是否正确
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="code">手机验证码</param>
        /// <param name="message">返回信息(失效，超时...)</param>
        /// <param name="message">返回错误信息</param>
        /// <returns>是否正确</returns>
        public static bool SendMobileRegisterSucces(string mobile, out string message)
        {
            message = string.Empty;
            try
            {
                //SendSM sms = new SendSM();
                //sms.SMS = string.Format(StrRegisterSucces, mobile, pwd);
                //sms.Mobile = mobile;
                //sms.CreateTime = DateTime.Now;
                //sms.Insert();
                //return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 注册用户信息 短信验证码内容
        /// </summary>
        public static string StrRegister = "仓库管理系统用户注册或既有用户手机核验专用验证码:{0}。如非本人直接访问仓库管理系统，请勿操作，切勿将验证码提供给第三方。";
        /// <summary>
        ///更改绑定手机 短信验证码内容
        /// </summary>
        public static string StrUpdate = "仓库管理系统用户更改绑定手机用户手机核验专用验证码:{0}。如非本人直接访问仓库管理系统，请勿操作，切勿将验证码提供给第三方。";
        /// <summary>
        /// 注册用户信息 邮箱激活
        /// </summary>
        public static string StrRegisteremail = "仓库管理系统用户注册链接:{0}。验证码：{1}";
         

        /// <summary>
        /// 注册用户信息 短信验证码内容
        /// </summary>
        public static string StrRegisterSucces = "感谢您注册仓库管理系统网,您的账户登录名为当前手机号码{0},初始登录密码为{1},请妥善保管。";



    }
}
