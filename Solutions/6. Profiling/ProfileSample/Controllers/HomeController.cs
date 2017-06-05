using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProfileSample.DAL;
using ProfileSample.Models;

namespace ProfileSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var context = new ProfileSampleEntities();

            var sources = context.ImgSources.Take(20).Select(x => new { x.Name, x.Data }).ToList();
            var model = new List<ImageModel>(sources.Count);

            foreach (var item in sources)
            {
                var obj = new ImageModel()
                {
                    Name = item.Name,
                    Data = item.Data
                };

                model.Add(obj);
            } 

            return View(model);
        }

        public ActionResult Convert()
        {
            var files = Directory.GetFiles(Server.MapPath("~/Content/Img"), "*.jpg").ToList();

            using (var context = new ProfileSampleEntities())
            {
                var entities = new List<ImgSource>(files.Count);
                foreach (var file in files)
                {
                    using (var stream = new FileStream(file, FileMode.Open))
                    {
                        byte[] buff = new byte[stream.Length];

                        stream.Read(buff, 0, (int) stream.Length);

                        var entity = new ImgSource()
                        {
                            Name = Path.GetFileName(file),
                            Data = buff,
                        };

                        entities.Add(entity);
                    }
                }

                context.ImgSources.AddRange(entities);
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}