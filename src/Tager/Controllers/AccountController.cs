using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TagManage.Models;

namespace TagManage.Controllers
{
#if DEBUG

#else
    [RequireHttps]
#endif
    public class AccountController : Controller
    {
        TagManageContext db = new TagManageContext();

        // GET: Account
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                string password = GetMD5(model.Password, model.Email);
                if (db.Users.Where(p => p.Email == model.Email && p.Password == password && p.RCode == "").Count() > 0)
                {
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                           1,
                           model.Email,
                           DateTime.Now,
                           DateTime.Now.AddMonths(2),
                           false,
                           "User"
                          );
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    System.Web.HttpCookie authCookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);

                    return Redirect("/Home/Tags");
                }

                ModelState.AddModelError("", "账号密码或密码错误或账号为激活!");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Register model)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Where(p => p.Email == model.Email).Count() > 0)
                {
                    ModelState.AddModelError("Email", "该邮箱已注册!");
                    return View(model);
                }

                User entity = new Models.User();
                entity.Email = model.Email;
                entity.Password = GetMD5(model.Password, model.Email);
                entity.RCode = Guid.NewGuid().ToString();

                var url = new UriBuilder(Request.Url)
                {
                    Path = Url.Action("Activation", "Account", new { id = entity.RCode }),
                    Query = ""
                }.ToString();

                string content = "欢迎注册标签管理器;<br/> 请点击 <a href ='" + url + "'> 此处 </a> 来确保您的邮箱地址为有效地址; 若不能跳转，请将以下地址复制到浏览器地址栏进行跳转:<br/>" + url +"<br/>若非本人操作请忽略此邮件.";
                if (!SendEmail(model.Email, "激活邮件", content))
                {
                    ModelState.AddModelError("", "激活邮件发送失败!请重试.");
                    return View(model);
                }

                db.Users.Add(entity);
                db.SaveChanges();

                return Content("<Script>alert('注册成功!请前往该邮箱查收激活邮件激活账号!');location.href='/';</Script>");
            }
            return View(model);
        }

        public ActionResult Activation(string id)
        {
            if (id != null)
            {
                var model = db.Users.Where(p => p.RCode == id).FirstOrDefault();
                if (model != null)
                {
                    model.RCode = "";
                    db.Entry<User>(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return Content("<Script>alert('激活成功!请登录.');location.href='/Account/Login';</Script>");
                }
                return Content("<Script>alert('该账号可能已经激活!请尝试登录.');location.href='/Account/Login';</Script>");
            }
            return Content("<Script>alert('激活失败!请重新注册.');location.href='/Account/Register';</Script>");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.Users.Where(p => p.Email == model.Email && p.RCode == "").FirstOrDefault();
                if (entity != null)
                {
                    entity.FCode = Guid.NewGuid().ToString();

                    var url = new UriBuilder(Request.Url)
                    {
                        Path = Url.Action("ResetPassword", "Account", new { id = entity.FCode }),
                        Query = ""
                    }.ToString();

                    string content = "请点击 <a href ='" + url + "'> 此处 </a> 来重置密码; 若不能跳转，请将以下地址复制到浏览器地址栏进行跳转:<br/>" + url + "<br/>若非本人操作请忽略此邮件.";
                    if (!SendEmail(model.Email, "找回密码", content))
                    {
                        ModelState.AddModelError("", "邮件发送失败!请重试.");
                        return View(model);
                    }

                    db.Users.Add(entity);
                    db.SaveChanges();

                    return Content("<Script>alert('邮件已发送!请前往邮箱重置密码.');location.href='/';</Script>");
                }
                ModelState.AddModelError("Email", "该账号未激活或不存在!");
                return View();
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            if (id != null)
            {
                if (db.Users.Where(p => p.FCode == id). Count() > 0)
                {
                    ViewBag.code = id;
                    return View();
                }
                return Content("<Script>alert('链接已失效!请重新申请.');location.href='/Account/ForgotPassword';</Script>");
            }
            return Content("<Script>alert('链接已失效!');Location.href='/';</Script>");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPassword model, string code)
        {
            ViewBag.code = code;

            if (ModelState.IsValid)
            {
                var entity = db.Users.Where(p => p.FCode == code).FirstOrDefault();

                if (entity != null)
                {
                    entity.FCode = "";
                    entity.Password = GetMD5(model.Password, entity.Email);

                    db.Entry<User>(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return Content("<Script>alert('密码设置成功!请登录.');location.href='/Account/Login';</Script>");
                }
                return View("Error");
            }
            return View();
        }

        public ActionResult CheckEmail(string email)
        {
            if (db.Users.Where(p => p.Email == email).Count() > 0)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }
        
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return Redirect("/");
        }

        private bool SendEmail(string to, string sub, string content)
        {
            MailMessage msg = new MailMessage();

            msg.To.Add(to);
            msg.Subject = sub;
            msg.From = new MailAddress("admin.tags@ydath.cn");
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码
            msg.Body = content;
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码
            msg.IsBodyHtml = true;
            msg.Priority = MailPriority.High;
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("admin.tags@ydath.cn", "Dudeping.2016sicau");
            client.Host = "smtp.exmail.qq.com";
            object userState = msg;

            try
            {
                client.Send(msg);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetMD5(string sDataIn, string code)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytValue, bytHash;
            bytValue = System.Text.Encoding.UTF8.GetBytes(sDataIn + code);
            bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            string sTemp = "";
            for (int i = 0; i < bytHash.Length; i++)
            {
                sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
            }
            return sTemp.ToLower();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}