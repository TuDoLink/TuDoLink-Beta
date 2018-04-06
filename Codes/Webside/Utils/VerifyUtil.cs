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
//using WHC.Framework.Commons;
//using DH.Mail.SendClound;

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
                    validCode.Code = GetVerifyCode(6);// new Random().Next(100000, 999999).ToString();

                    validCode.CreateAt = DateTime.Now;

                    validCode.Insert();
                }
                var enCode = SecurityHelper.DESEncrypt(validCode.Code, key);

                //发送邮件
                var validUrl = url + "/Account/ValidEmail?code=" + enCode;

                var subject = "Welcome to Join Tudolink";
                //var html = string.Format("<h2>欢迎加入Tudolink</h2><br /><a href='{0}'>点击激活账户</a>", validUrl);

                var path = HttpContext.Current.Server.MapPath("~/Config/validEmail.config");
                var html = System.IO.File.ReadAllText(path).Replace("{validUrl}", validUrl);

                TB_MailTask mailTask = new TB_MailTask();
                mailTask.CreateAt = DateTime.Now;
                mailTask.Subject = subject;
                mailTask.Detail = html;
                mailTask.Target = email;


                AppConfig config = new AppConfig();
                var api = config.AppConfigGet("Mail_Api");
                if (api == "SendCloudApi")
                {
                    if (SendCloudApi.Send(email, subject, html))
                    {
                        mailTask.SendAt = DateTime.Now;
                    }
                }
                else
                {
                    SendMail(subject, null, html, email);
                }

                mailTask.Insert();

                return true;
            }
        }
        public static string SendMail(string topic, string attachmentUrl, string body, string email)
        {
            AppConfig config = new AppConfig();

            var config_port = config.AppConfigGet("Mail_Port");// "info2018TD";;
            var config_user = config.AppConfigGet("Mail_User");// "info@tudolink.io";
            var config_password = config.AppConfigGet("Mail_Password");// "info2018TD";

            string sendAddress = config_user;// "info@tudolink.io";
            string sendPassword = config_password;
            string receiveAddress = email;
            string mailAttachment = attachmentUrl;//附件
            string mailBody = body;//内容

            SmtpClient client = new SmtpClient("smtp.zoho.com");   //设置邮件协议

            client.UseDefaultCredentials = false;//这一句得写前面
            client.EnableSsl = true;//服务器不支持SSL连接

            client.Port = 587;
            if (!string.IsNullOrEmpty(config_port))
            {
                client.Port = Convert.ToInt32(config_port);
            }
            client.DeliveryMethod = SmtpDeliveryMethod.Network; //通过网络发送到Smtp服务器
            //client.Credentials = new NetworkCredential(sendUsername[0].ToString(), sendPassword); //通过用户名和密码 认证
            client.Credentials = new NetworkCredential(sendAddress, sendPassword); //通过用户名和密码 认证 
            MailMessage mmsg = new MailMessage(new MailAddress(sendAddress), new MailAddress(receiveAddress)); //发件人和收件人的邮箱地址
            mmsg.Subject = topic;//邮件主题
            mmsg.SubjectEncoding = Encoding.UTF8;//主题编码
            mmsg.Body = mailBody;//邮件正文
            mmsg.IsBodyHtml = true;
            mmsg.BodyEncoding = Encoding.UTF8;//正文编码
            mmsg.IsBodyHtml = true;//设置为HTML格式           
            mmsg.Priority = MailPriority.High;//优先级
            if (!string.IsNullOrEmpty(mailAttachment))
            {
                mmsg.Attachments.Add(new Attachment(mailAttachment));//增加附件
            }
            try
            {
                client.Send(mmsg);
                return null;
            }
            catch (Exception ee)
            {
                return ee.Message;
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
        public static bool CheckEmailVerfyCode(string code, out TB_User user, out string message)
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




        /// <summary>
        /// 校验验证码是否正确
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="code">手机验证码</param>
        /// <param name="message">返回信息(失效，超时...)</param>
        /// <param name="message">返回错误信息</param>
        /// <returns>是否正确</returns>
        public static bool SendMobileRegisterSucces(string mobile, string pwd, out string message)
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