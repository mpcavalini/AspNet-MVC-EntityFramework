using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PetStore.Core.UnitOfWork;
using PagedList;
using PetStore.Core.Domain;
using System.Net;
using Serilog;
using System.IO;
using System.Threading;

namespace PetStore.Web.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IPetUnitOfWork service;
        private readonly ILogger _logger = Log.ForContext<DefaultController>();
     

        public DefaultController(IPetUnitOfWork uok)
        {
            service = uok;
        }


        public ActionResult Index(string sort, string name, int? page, string filterName, byte? type, byte? filterType)
        {
            ViewBag.Sort = sort;
            ViewBag.NameSort = String.IsNullOrEmpty(sort) ? "name_desc" : "";
            ViewBag.DateSort = sort == "date" ? "date_desc" : "date";
            ViewBag.WeightSort = sort == "weight" ? "weight_desc" : "weight";

            if (name != null)
            {
                page = 1;
            }
            else
            {
                name = filterName;
            }

            ViewBag.FilterName = name;

            if (type >= 0)
            {
                page = 1;
            }
            else
            {
                type = filterType;
            }

            ViewBag.FilterType = type;

            var pets = Filter(name, type);
            Sort(sort,ref pets);

            int pageSize = 30;
            int pageNumber = (page ?? 1);

            return View(pets.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "Name,DateOfBirth,Weight,Type")] Pet pet)
        {
            if (ModelState.IsValid && pet.IsValid())
            {
                if (service.Pets.SingleOrDefault(s => s.Name == pet.Name && s.DateOfBirth == pet.DateOfBirth) == null)
                {
                    service.Pets.Add(pet);
                    var status = service.Complete();

                    if (status == 1)
                    {
                        _logger.Information("Pet added ID:{id}", pet.Id);
                        _logger.Information("Pet csv: {@Pet}", pet.ToString());
                        _logger.Information("Pet object: {@Pet}", pet);

                        TempData["Message"] = new Message("New pet created with success", MessageType.Success);
                        return RedirectToAction("Index");
                    }
                }

                TempData["Message"] = new Message("The pet name and date of birth already exists.", MessageType.Warning);
                return View();

            }

            TempData["Message"] = new Message("Your pet data is invalid, please, review it.", MessageType.Error);
            return View();
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var pet = service.Pets.Get((int)id);

            if (pet ==null)
            {
                return HttpNotFound();
            }

            return View(pet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,DateOfBirth,Weight,Type")] Pet pet)
        {
            if (ModelState.IsValid && pet.IsValid())
            {
                if (service.Pets.SingleOrDefault(s => s.Name == pet.Name && s.DateOfBirth == pet.DateOfBirth) == null)
                {
                    service.Pets.Update(pet);
                    var status = service.Complete();

                    if (status == 1)
                    {
                        _logger.Information("Pet saved ID: {id}", pet.Id);
                        _logger.Information("Pet csv: {@Pet}", pet.ToString());
                        _logger.Information("Pet object: {@Pet}", pet);

                        TempData["Message"] = new Message("New pet created with success", MessageType.Success);
                        return RedirectToAction("Index");
                    }
                }

                TempData["Message"] = new Message("The pet name and date of birth already exists.", MessageType.Warning);
                return View();

            }

            TempData["Message"] = new Message("Your pet data is invalid, please, review it.", MessageType.Error);
            return View();
        }

        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var pet = service.Pets.Get((int)id);
            if (pet == null)
            {
                return HttpNotFound();
            }

            service.Pets.Remove(pet);
            var status = service.Complete();

            if (status == 1)
            {
                _logger.Information("Pet deleted ID:{id}",pet.Id);
                _logger.Information("Pet csv: {@Pet}", pet.ToString());
                _logger.Information("Pet object: {@Pet}", pet);

                TempData["Message"] = new Message("Pet deleted with success", MessageType.Success);
                return RedirectToAction("Index");
            }

            TempData["Message"] = new Message("Something went wrong", MessageType.Warning);
            return RedirectToAction("Index");
        }

        public ActionResult Logs()
        {
            string[] filePaths = Directory.GetFiles(LogPath());

            List<string> files = new List<string>();
            foreach (string filePath in filePaths)
            {
                files.Add(Path.GetFileName(filePath));
            }

            return View(files);
        }

        public FileResult DownloadFile(string fileName)
        {
            string path = LogPath() + fileName;
            byte[] bytes = null;
            int maxRetryAttempts = 5;
            int retryDelayMilliseconds = 100;

            for (int attempt = 0; attempt < maxRetryAttempts; attempt++)
            {
                try
                {
                    bytes = System.IO.File.ReadAllBytes(path);

                    return File(bytes, "application/octet-stream", fileName);
                }
                catch (IOException ex) when (attempt < maxRetryAttempts - 1)
                {
                    Thread.Sleep(retryDelayMilliseconds);
                }
            }

            // Fake file in case system is holding above file
            path = LogPath() + ".gitkeep";
            bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", fileName);
        }

        private IEnumerable<Pet> Filter(string name, byte? type)
        {
            if (!String.IsNullOrEmpty(name) && type.HasValue && type >= 0)
            {
                return service.Pets.Find(s => s.Name.Contains(name) && s.Type == (PetType)type);
            }

            if (!string.IsNullOrEmpty(name))
            {
                return service.Pets.Find(s => s.Name.Contains(name));
            }

            if (type.HasValue && type >= 0)
            {
                return service.Pets.Find(s => s.Type == (PetType)type);
            }

            return service.Pets.GetAll();
        }

        private IEnumerable<Pet> Sort(string sort, ref IEnumerable<Pet> pets)
        {
            switch (sort)
            {
                case "name_desc":
                    pets = pets.OrderByDescending(s => s.Name);
                    break;
                case "date":
                    pets = pets.OrderBy(s => s.DateOfBirth);
                    break;
                case "date_desc":
                    pets = pets.OrderByDescending(s => s.DateOfBirth);
                    break;
                case "weight":
                    pets = pets.OrderBy(s => s.Weight);
                    break;
                case "weight_desc":
                    pets = pets.OrderByDescending(s => s.Weight);
                    break;
                default:  // Name ascending 
                    pets = pets.OrderBy(s => s.Name);
                    break;
            }

            return pets;
        }

        private string LogPath() => Server.MapPath("~/Logs/");
    }
}