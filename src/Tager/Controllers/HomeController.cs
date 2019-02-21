using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TagManage.Models;

namespace TagManage.Controllers
{
#if DEBUG

#else
    [RequireHttps]
#endif
    public class HomeController : Controller
    {
        TagManageContext db = new TagManageContext();

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Tags(string searchStr, int page = 1)
        {
            var model = from x in db.Tags
                        where x.User.Email == User.Identity.Name
                        select x;

            if (searchStr != null)
            {
                ViewBag.SearchStr = searchStr;
                ViewBag.TagsList = model.Where(p => (p.Name.Contains(searchStr) || p.Url.Contains(searchStr))).OrderByDescending(p => p.ClickNum).ToPagedList(page, 20);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_PartialTags");
                }
                else
                {
                    return View("SIndex");
                }
            }

            ViewBag.TagsList = model.OrderByDescending(p => p.ClickNum).ToPagedList(page, 20);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_PartialTags");
            }
            
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddTag(Tag model)
        {
            if (ModelState.IsValid)
            {
                Tag entity = new Tag();
                entity.Name = model.Name;
                entity.Url = model.Url;
                entity.ClickNum = 0;
                entity.User = db.Users.Where(p => p.Email == User.Identity.Name).FirstOrDefault();

                db.Tags.Add(entity);
                db.SaveChanges();

                return Content("<Script>alert('添加成功!');location.href='/Home/Tags';</Script>");
            }

            return Content("<Script>alert('添加失败!请重试.');location.href='/Home/Tags';</Script>");
        }

        [HttpPost]
        [Authorize]
        public void AddClick(int id)
        {
            var model = db.Tags.Find(id);
            if (model.User.Email == User.Identity.Name)
            {
                model.ClickNum++;
                model.User = model.User;
                db.Entry<Tag>(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult CheckTag(string name)
        {
            if (db.Tags.Where(p => p.Name == name).Count() > 0)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteTag(int id, string searchStr)
        {
            var model = db.Tags.Find(id);
            if (model != null)
            {
                db.Tags.Remove(model);
                db.SaveChanges();
                if (searchStr == null)
                {
                    ViewBag.TagsList = (from x in db.Tags where x.User.Email == User.Identity.Name orderby x.ClickNum descending, x.Id descending select x).ToList();
                }
                else
                {
                    ViewBag.TagsList = (from x in db.Tags where x.User.Email == User.Identity.Name && (x.Name.Contains(searchStr) || x.Url.Contains(searchStr)) orderby x.ClickNum descending, x.Id descending select x).ToList();
                }
                ViewBag.SearchStr = searchStr;
                return PartialView("_PartialTagList");
            }

            return Json(false);
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