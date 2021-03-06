﻿using DPTS.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPTS.Domain.Core.Address;
using DPTS.Domain.Core.Doctors;
using DPTS.Domain.Core.Speciality;
using PagedList;
using DPTS.Domain.Entities;
using DPTS.Domain.Core.Country;
using DPTS.Domain.Core.StateProvince;
using DPTS.Data.Context;
using System;
using DPTS.Domain.Common;
using Microsoft.AspNet.Identity;
using DPTS.Domain.Core.GenericAttributes;

namespace DPTS.Web.Controllers
{
    public class HomeController : Controller
    {
        #region Field

        private readonly ISpecialityService _specialityService;
        private readonly IDoctorService _doctorService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateService;
        private readonly IAddressService _addressService;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext context;
        private readonly DPTSDbContext _context;
        private readonly IPictureService _pictureService;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Constructor

        public HomeController(ISpecialityService specialityService,
            IDoctorService doctorService,
            IAddressService addressService,
            ICountryService countryService,
            IStateProvinceService stateService,
            IPictureService pictureService,
            IGenericAttributeService genericAttributeService)
        {
            _specialityService = specialityService;
            _doctorService = doctorService;
            UserManager = _userManager;
            context = new ApplicationDbContext();
            _addressService = addressService;
            _countryService = countryService;
            _stateService = stateService;
            _context = new DPTSDbContext();
            _pictureService = pictureService;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Utilities
        public SearchModel PrepareSearchModel(SearchModel model)
        {
            var searchModel = new SearchModel
            {
                Specialitie = model.Specialitie,
                minfee = model.minfee,
                maxfee = model.maxfee,
                geo_location = model.geo_location,
                q = model.q
            };
            return searchModel;
        }
        public PictureModel GetProfilePicture(string doctorId)
        {
            var pictures = _pictureService.GetPicturesByUserId(doctorId);
            var defaultPicture = pictures.FirstOrDefault();
            var defaultPictureModel = new PictureModel
            {
                ImageUrl = (defaultPicture == null) ? "/Content/wp-content/themes/docdirect/images/doctor-male.png" :
                _pictureService.GetPictureUrl(defaultPicture, 365, false),
                FullSizeImageUrl = (defaultPicture == null) ? "/Content/wp-content/themes/docdirect/images/doctor-male.png" :
                _pictureService.GetPictureUrl(defaultPicture, 0, false),
                Title = "",
                AlternateText = "",
            };
            return defaultPictureModel;
        }
        public List<PictureModel> GetAllPictures(string doctorId)
        {
            var pictures = _pictureService.GetPicturesByUserId(doctorId);
            var pictureModels = new List<PictureModel>();
            if (pictures != null)
            {
                foreach (var picture in pictures.Skip(1))
                {
                    var pictureModel = new PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture, 150),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        Title = "",
                        AlternateText = "",
                    };
                    pictureModels.Add(pictureModel);
                }
            }
            return pictureModels;
        }

        [NonAction]
        public string GetAddressline(Address model)
        {
            try
            {
                string addrLine = string.Empty;
                if (model != null)
                {
                    addrLine += model.Address1 + ", ";
                    addrLine += model.City;
                }
                return addrLine;
            }
            catch { return null; }
        }
        [NonAction]
        public string GetShortAddressline(Address model)
        {
            try
            {
                string addrLine = string.Empty;
                if (model != null)
                {
                    addrLine += model.City + ", ";
                    addrLine += model.StateProvince.Name + ", ";
                    addrLine += model.Country.Name;
                }
                return addrLine;
            }
            catch { return null; }
        }
        [NonAction]
        public string GetTotalExperience(ICollection<Experience> model)
        {
            decimal totexpr = 0;
            try
            {
                if (model != null)
                {
                    var startDate = model.FirstOrDefault().StartDate;
                    var endDate = model.LastOrDefault().EndDate;
                    if (startDate != null && endDate != null)
                    {
                        var totday = (endDate - startDate).TotalDays;
                        if (totday > 0)
                        {
                            totexpr = decimal.Parse(totday.ToString()) / 365.25m;
                            totexpr = Math.Round(totexpr, 0);
                        }
                    }
                }

            }
            catch { }
            return totexpr.ToString();
        }
        [NonAction]
        public string GetQualification(ICollection<Education> model)
        {
            string eduList = string.Empty;
            try
            {
                if(model != null)
                {
                    foreach (var edu in model)
                    {
                        eduList += edu.Title + " ,";
                    }
                }
            }
            catch { }
            return eduList.TrimEnd(',');
        }
        [NonAction]
        public string GetSpecialities(ICollection<Speciality> model)
        {
            string specList = string.Empty;
            try
            {
                if (model != null)
                {
                    foreach (var spc in model)
                    {
                        specList += spc.Title + " ,";
                    }
                }
            }
            catch { }
            return specList.TrimEnd(',');
        }

        public IList<SelectListItem> GetSpecialityList()
        {
            var specialitys = _specialityService.GetAllSpeciality(false);
            if (specialitys == null)
                return null;

            List<SelectListItem> typelst = new List<SelectListItem>();
            typelst.Add(
                new SelectListItem
                {
                    Text = "Select Speciality",
                    Value = "0"
                });
            foreach (var _type in specialitys.ToList())
            {
                typelst.Add(
                    new SelectListItem
                    {
                        Text = _type.Title,
                        Value = _type.Id.ToString()
                    });
            }
            return typelst;
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }


        public ApplicationUser GetUserById(string userId)
        {
            return context.Users.SingleOrDefault(u => u.Id == userId);
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult SearchBox()
        {
            var model = new SearchModel
            {
               // AvailableSpeciality = GetSpecialityList()
            };
            return PartialView(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page is here.";

            return View();
        }
        
        public PartialViewResult _DoctorBox(SearchModel model, int? page)
        {
            var searchViewModel = new List<TempDoctorViewModel>();
            var pageNumber = (page ?? 1) - 1;
            var pageSize = 5;
            int totalCount;
            int specilityId = 0;
            string searchByName = string.Empty;

            if (model == null)
                model = new SearchModel();


            var searchTerms = model.q ?? "";
            if (!string.IsNullOrWhiteSpace(model.q))
                searchTerms = model.q.Trim();


            if (!string.IsNullOrWhiteSpace(searchTerms))
            {
                try
                {
                    specilityId = _specialityService.GetAllSpeciality(true).Where(s => s.Title == searchTerms).FirstOrDefault().Id;
                }
                catch
                {
                    specilityId = 0;
                }
                if (specilityId == 0)
                {
                    searchByName = searchTerms;
                }
            }

            var data = _doctorService.SearchDoctor(pageNumber, pageSize, out totalCount,
                model.geo_location,
                specialityId: specilityId,
                searchByName: searchByName,
                maxFee: model.maxfee,
                minFee: model.minfee,
                SortBy:model.SortBy,
                searchCriteria:model.searchCriteria
                );

            if (data != null)
            {
                searchViewModel = data.Select(doc => new TempDoctorViewModel
                {
                    Doctors = doc,
                    Address = _addressService.GetAllAddressByUser(doc.DoctorId).FirstOrDefault(),
                    AddressLine = GetAddressline(_addressService.GetAllAddressByUser(doc.DoctorId).FirstOrDefault()),
                    YearOfExperience = GetTotalExperience(doc.Experience),
                    Qualification = GetQualification(doc.Education),
                    ListSpecialities = GetSpecialities(_specialityService.GetDoctorSpecilities(doc.DoctorId)),
                    ReviewOverviewModel = PrepareDoctorReviewOverviewModel(doc),
                    AddPictureModel = GetProfilePicture(doc.DoctorId),
                    DoctorPictureModels = GetAllPictures(doc.DoctorId)
                }).ToList();
            }

            IPagedList<TempDoctorViewModel> pageDoctors = new StaticPagedList<TempDoctorViewModel>(searchViewModel,
                pageNumber + 1, pageSize, totalCount);
            ViewBag.SearchModel = PrepareSearchModel(model);
            ViewBag.IsHomePageSearch = false;
            return PartialView(pageDoctors);
        }

        public static DoctorReviewOverviewModel PrepareDoctorReviewOverviewModel(Doctor doctor)
        {
            DoctorReviewOverviewModel doctorReview;
            doctorReview = new DoctorReviewOverviewModel()
                {
                    RatingSum = doctor.ApprovedRatingSum,
                    TotalReviews = doctor.ApprovedTotalReviews
                };
            if (doctorReview != null)
            {
                doctorReview.DoctorId = doctor.DoctorId;
                doctorReview.AllowPatientReviews = true;
            }
            return doctorReview;
        }

        public int GetRatingPercentage(Doctor doc)
        {
            int ratingPercent = 0;
            try
            {
                var model = PrepareDoctorReviewOverviewModel(doc);
                if (model.TotalReviews != 0)
                {
                    ratingPercent = ((model.RatingSum * 100) / model.TotalReviews) / 5;
                }
                return ratingPercent;
            }
            catch { return ratingPercent; }
        }

       // [ValidateInput(false)]
        //[OutputCache(Duration = 300, VaryByParam = "page")]
        public ActionResult Search(SearchModel model, int? page)
        {
            var searchViewModel = new List<TempDoctorViewModel>();
            var pageNumber = (page ?? 1) - 1;
            var pageSize = 10;
            int totalCount;
            int specilityId = 0;
            string searchByName = string.Empty;

            if (model == null)
                model = new SearchModel();

            var searchTerms = model.q ?? "";
            if(!string.IsNullOrWhiteSpace(model.q))
                searchTerms = model.q.Trim();

            if(!string.IsNullOrWhiteSpace(searchTerms))
            {
                try
                {
                    specilityId = _specialityService.GetAllSpeciality(true).Where(s => s.Title == searchTerms).FirstOrDefault().Id;
                }
                catch
                {
                    specilityId = 0;
                }
                if(specilityId == 0)
                {
                    searchByName = searchTerms;
                }
            }

            var data = _doctorService.SearchDoctor(pageNumber, pageSize, out totalCount,
                model.geo_location,
                specilityId,
                searchByName,
                searchCategory:null);

            if (data != null)
            {
                searchViewModel = data.Select(doc => new TempDoctorViewModel
                {
                    Doctors = doc,
                    Address = _addressService.GetAllAddressByUser(doc.DoctorId).FirstOrDefault(),
                    AddressLine = GetAddressline(_addressService.GetAllAddressByUser(doc.DoctorId).FirstOrDefault()),
                    YearOfExperience = GetTotalExperience(doc.Experience),
                    Qualification = GetQualification(doc.Education),
                    ListSpecialities = GetSpecialities(_specialityService.GetDoctorSpecilities(doc.DoctorId)),
                    ReviewOverviewModel = PrepareDoctorReviewOverviewModel(doc),
                    AddPictureModel =GetProfilePicture(doc.DoctorId),
                    DoctorPictureModels = GetAllPictures(doc.DoctorId)
                }).ToList();
            }

            IPagedList<TempDoctorViewModel> pageDoctors = new StaticPagedList<TempDoctorViewModel>(searchViewModel,
                pageNumber + 1, pageSize, totalCount);

            ViewBag.SearchModel = PrepareSearchModel(model);

            ViewBag.IsHomePageSearch = true;

            return View(pageDoctors);
        }

        [ValidateInput(false)]
        //[OutputCache(Duration = 300, VaryByParam = "page")]
        public ActionResult TopFilter(string searchCriteria, int? page)
        {
            var searchViewModel = new List<TempDoctorViewModel>();
            var pageNumber = (page ?? 1) - 1;
            var pageSize = 5;
            int totalCount;

            var model = new SearchModel();

            var searchTerms = model.q ?? "";
            if (!string.IsNullOrWhiteSpace(model.q))
                searchTerms = model.q.Trim();

            var data = _doctorService.SearchDoctor(pageNumber, pageSize, out totalCount,
                model.geo_location,
                0,null,
                searchCriteria);

            if (data != null)
            {
                searchViewModel = data.Select(doc => new TempDoctorViewModel
                {
                    Doctors = doc,
                    Address = _addressService.GetAllAddressByUser(doc.DoctorId).FirstOrDefault(),
                    AddressLine = GetAddressline(_addressService.GetAllAddressByUser(doc.DoctorId).FirstOrDefault()),
                    YearOfExperience = GetTotalExperience(doc.Experience),
                    Qualification = GetQualification(doc.Education),
                    ListSpecialities = GetSpecialities(_specialityService.GetDoctorSpecilities(doc.DoctorId)),
                    ReviewOverviewModel = PrepareDoctorReviewOverviewModel(doc),
                    AddPictureModel = GetProfilePicture(doc.DoctorId),
                    EmailConsultMessage = (doc.EmailConsultFee > 0) ? "Charge for per question" + doc.EmailConsultFee +" by e-mail consult" :
                    "You can conuslt to Dr."+doc.AspNetUser.FirstName +" "+ doc.AspNetUser.LastName+"  free of charge by e-mail",
                    DoctorPictureModels = GetAllPictures(doc.DoctorId)
                }).ToList();
            }

            IPagedList<TempDoctorViewModel> pageDoctors = new StaticPagedList<TempDoctorViewModel>(searchViewModel,
                pageNumber + 1, pageSize, totalCount);

            ViewBag.SearchModel = PrepareSearchModel(model);

            ViewBag.IsHomePageSearch = true;

            return View("Search",pageDoctors);
        }

        //[ValidateInput(false)]
        //[OutputCache(Duration = 300, VaryByParam = "categoryname")]
        public ActionResult SuggestedDoctors(string categoryname)
        {
            var viewModel = new List<SuggestedDoctorViewModel>();
            int specilityId = 0;
            if (!string.IsNullOrWhiteSpace(categoryname))
            {
                try
                {
                    specilityId = _specialityService.GetAllSpeciality(true).Where(s => s.Title == categoryname).FirstOrDefault().Id;
                }
                catch
                {
                    specilityId = 0;
                }
            }
            var data = _doctorService.GetAllDoctorBySpeciality(specialityId: specilityId);
            if (data != null)
            {
                viewModel = data.Select(doc => new SuggestedDoctorViewModel
                {
                    Picture = GetProfilePicture(doc.DoctorId),
                    DoctorId = doc.DoctorId,
                    Name = doc.AspNetUser.FirstName + " " + doc.AspNetUser.LastName,
                    Fees = doc.ConsultationFee,
                    AddressLine = GetShortAddressline(_addressService.GetAllAddressByUser(doc.DoctorId).FirstOrDefault()),
                    Specailities = GetSpecialities(_specialityService.GetDoctorSpecilities(doc.DoctorId)),
                    Qualification = GetQualification(doc.Education),
                    YearOfExperience = GetTotalExperience(doc.Experience),
                    RatingPercentag = GetRatingPercentage(doc)
                }).ToList();
            }
            return PartialView("SuggestedDoctors", viewModel);
        }

        /// <summary>
        /// Get all doctors
        /// </summary>
        /// <param name="model"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Doctors(SearchModel model, int? page)
        {
            if (Request.QueryString.Count > 0 &&
                !string.IsNullOrWhiteSpace(model.Specialitie) ||
                !string.IsNullOrWhiteSpace(model.geo_location))
              //  !string.IsNullOrWhiteSpace(model.keyword))
            {
                TempData["SearchModel"] = model;
                return RedirectToAction("Search");
            }

            var pageNumber = (page ?? 1) - 1;
            var pageSize = 5;
            int totalCount;

            var data = _doctorService.GetAllDoctors(pageNumber, pageSize, out totalCount);

            var doctorViews = data.Select(doc => new TempDoctorViewModel
            {
                Doctors = doc,
                Address = _addressService.GetAllAddressByUser(doc.DoctorId).FirstOrDefault()
            }).ToList();

            IPagedList<TempDoctorViewModel> pageDoctors = new StaticPagedList<TempDoctorViewModel>(doctorViews,
                pageNumber + 1, pageSize, totalCount);

            var searchModel = new SearchModel
            {
                //  AvailableSpeciality = GetSpecialityList(),
            };
           ViewBag.SearchModel = searchModel;

            return View(pageDoctors);
        }

        public string GetProfilePicture1(string doctorId)
        {
            try
            {
                var pictures = _pictureService.GetPicturesByUserId(doctorId);
                var defaultPicture = pictures.FirstOrDefault();
                var imageUrl = (defaultPicture == null) ? "/Content/wp-content/themes/docdirect/images/singin_icon.png" :
                    _pictureService.GetPictureUrl(defaultPicture, 365, false);
                return imageUrl;
            }
            catch
            {
                return "/Content/wp-content/themes/docdirect/images/singin_icon.png";
            }
        }
        
        public ActionResult _ProfilePicture()
        {
            if (!Request.IsAuthenticated && !User.IsInRole("Doctor"))
                return new HttpUnauthorizedResult();

            ViewBag.FirstName = UserManager.FindById(User.Identity.GetUserId()).FirstName;
            ViewBag.SmallProfilePictureUrl = GetProfilePicture1(User.Identity.GetUserId());
            return PartialView();
        }
        [OutputCache(Duration = 300)]
        public ActionResult LocationList()
        {
            var locations = _genericAttributeService.GetAllLocation();
            return PartialView(locations);
        }
        [OutputCache(Duration = 300)]
        public ActionResult SpecialityList()
        {
            var spec = _genericAttributeService.GetAllSpecialities();
            return PartialView(spec);
        }
        public ActionResult Location_subpage()
        {
            return View();
        }
        public ActionResult detail_page()
        {
            return View();
        }
    }
}