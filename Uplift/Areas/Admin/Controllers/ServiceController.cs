using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Uplift.DataAccess.Data.Repository.IRepository;
using Uplift.Models.ViewModels;
using System.IO;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace Uplift.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        [BindProperty] 
        public ServiceViewModel ServiceViewModel { get; set; }

        public ServiceController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? Id)
        {
            ServiceViewModel = new ServiceViewModel()
            {
                Service = new Models.Service(),
                CategoryList = _unitOfWork.Category.GetCategoryListForDropdown(),
                FrequencyList = _unitOfWork.Frequency.GetFrequencyListForDropdown()
            };

            if (Id != null)
            {
                ServiceViewModel.Service = _unitOfWork.Service.Get(Id.GetValueOrDefault());
            }

            return View(ServiceViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (ServiceViewModel.Service.Id == 0)
                {
                    // create a new service
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\services");
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }
                    ServiceViewModel.Service.ImageUrl = @"\images\services\" + fileName + extension;

                    _unitOfWork.Service.Add(ServiceViewModel.Service);
                }
                else
                {
                    // editing an existing service
                    var serviceFromDb = _unitOfWork.Service.Get(ServiceViewModel.Service.Id);
                    if (files.Count > 0)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        var uploads = Path.Combine(webRootPath, @"images\services");
                        var extension_new = Path.GetExtension(files[0].FileName);

                        // delete the old image file
                        var imagePath = Path.Combine(webRootPath, serviceFromDb.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }

                        using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension_new), FileMode.Create))
                        {
                            files[0].CopyTo(fileStreams);
                        }
                        ServiceViewModel.Service.ImageUrl = @"\images\services\" + fileName + extension_new;
                    }
                    else
                    {
                        // no image was uploaded
                        ServiceViewModel.Service.ImageUrl = serviceFromDb.ImageUrl;
                    }

                    _unitOfWork.Service.Update(ServiceViewModel.Service);
                }
                // save the changes to db
                _unitOfWork.Save();

                // refresh the page
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ServiceViewModel.CategoryList = _unitOfWork.Category.GetCategoryListForDropdown();
                ServiceViewModel.FrequencyList = _unitOfWork.Frequency.GetFrequencyListForDropdown();
                return View(ServiceViewModel);
            }
        }

        #region API CALLS

        public IActionResult GetAll()
        {
            return Json(new {data = _unitOfWork.Service.GetAll(includeProperties: "Category,Frequency") });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objectFromDb = _unitOfWork.Service.Get(id);

            string webRootPath = _hostEnvironment.WebRootPath;

            var imagePath = Path.Combine(webRootPath, objectFromDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            if (objectFromDb == null)
            {
                return  Json(new {success = false, Message="Error while deleting." });
            }

            _unitOfWork.Service.Remove(objectFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, Message = "Delete worked" });
        }
        #endregion
    }
}