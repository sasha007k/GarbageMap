using System;
using System.Collections.Generic;
using System.Linq;
using GarbageMap.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using GarbageMap.GarbageDetection.ML.DataModels;
using GarbageMap.GarbageDetection.Services;
using GarbageMap.Models.DbModels;
using GarbageMap.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using GarbageMap.Models.ApiModels;
using Microsoft.EntityFrameworkCore;

namespace GarbageMap.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _client;
        private readonly IObjectDetectionService _objectDetectionService;

        public HomeController(ApplicationDbContext context, HttpClient client, IObjectDetectionService objectDetectionService)
        {
            _context = context;
            _client = client;
            _objectDetectionService = objectDetectionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAllCameraPlaces()
        {
            var cameraPlaces = _context?.CameraPlaces?.ToList();
            var garbageCans = _context?.GarbageCans?.ToList();
            var cameraPlaceViewModels = new List<CameraPlaceViewModel>();
            foreach (var place in cameraPlaces)
            {
                var cans = garbageCans.Where(x => x.CameraPlaceId == place.Id);
                var avarage = 0.0;
                if (cans.Any())
                {
                    avarage = cans.Average(x => x.FulfiledPercentage);
                }

                cameraPlaceViewModels.Add(
                    new CameraPlaceViewModel
                    {
                        Place = place,
                        AverageFullfill = Math.Round(avarage, 2)
                    });
            }

            return Json(cameraPlaceViewModels);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IEnumerable<RegionModel> GetRegionsList()
        {
            var list = _context.Regions.Select(s => new RegionModel() { Name = s.Name, Index = s.Index }).ToList();
            return list.OrderBy(l => l.Name);
        }

        public ActionResult AddCamera()
        {
            ViewData["RegionsList"] = new SelectList(GetRegionsList(), "Index", "Name");

            return PartialView();
        }

        public JsonResult GetDistrictByIndex(int regionIndex)
        {
            var districts = _context.Districts.Where(d => d.Region.Index == regionIndex).ToList();
            districts.Insert(0, new District() {Id = -1, Index = -1, Name = "Select District"});
            return Json(new SelectList(districts, "Index", "Name"));
        }

        public JsonResult GetCitiesByIndex(int regionIndex, int districtIndex)
        {
            var regionCities = _context.Cities.Where(d => d.Region.Index == regionIndex).ToList();
            var districtCities = _context.Cities.Where(d => d.District.Index == districtIndex).ToList();
            var allCities = new List<City>(regionCities);
            allCities.AddRange(districtCities);
            allCities.Insert(0, new City() { Id = -1, Index = -1, Name = "Select City" });
            return Json(new SelectList(allCities, "Id", "Name"));
        }

        [HttpPost]
        public async Task<ActionResult> AddCamera(AddCameraViewModel viewModel)
        {
            if (viewModel != null && !string.IsNullOrWhiteSpace(viewModel.Street))
            {
                var city = _context.Cities.FirstOrDefault(c => c.Id == viewModel.CityId);
                if (city != null)
                {
                    city.Region = _context.Regions.FirstOrDefault(c => c.Index == viewModel.RegionIndex);
                    if (city?.Region != null)
                    {
                        var address = $"{city.Region.Name}, {city.Name}, {viewModel.Street}";

                        if (viewModel.HouseNumber > 0)
                        {
                            address += $" {viewModel.HouseNumber}";
                        }

                        var result = await _client.GetAsync($"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key=AIzaSyC3amkviJ3ysQSwHyFWEZBq2hWrRjobabw");

                        if (result.IsSuccessStatusCode)
                        {
                            var content = await result.Content.ReadAsStringAsync();
                            var root = JsonConvert.DeserializeObject<RootObject>(content);

                            if (root.Status == "OK")
                            {
                                var latitude = root.Results[0].Geometry.Location.Latitude;
                                var longitude = root.Results[0].Geometry.Location.Longitude;

                                var cameraAddress = new Address() 
                                { 
                                    City = city, 
                                    CityId = city.Id, 
                                    HouseNumber = viewModel.HouseNumber, 
                                    Street = viewModel.Street 
                                };
                                var addedAddress = await _context.Addresses.AddAsync(cameraAddress);
                                await _context.SaveChangesAsync();

                                var cameraPlace = new CameraPlace() 
                                { 
                                    Address = addedAddress.Entity, 
                                    AddressId = addedAddress.Entity.Id, 
                                    Latitude = latitude, 
                                    Longitude = longitude 
                                };
                                await _context.CameraPlaces.AddAsync(cameraPlace);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult CameraPoints()
        {
            var cameraPlaces = _context.CameraPlaces.Include(p => p.Address)
                .ThenInclude(p => p.City)
                .ThenInclude(p => p.District)
                .ThenInclude(p => p.Region)
                .Include(p => p.Address)
                .ThenInclude(p => p.City)
                .ThenInclude(p => p.Region)
                .ToList();

            var cameraPoints = new List<CameraPointViewModel>();

            foreach (var place in cameraPlaces)
            {
                var garbageCans = _context.GarbageCans.Where(g => g.CameraPlaceId == place.Id)
                    .Include(g => g.CameraPlace)
                    .Include(g => g.GarbageCanType).ToList();

                var numberOfCans = garbageCans.Count();
                var totalCapacity = garbageCans.Select(g => g.GarbageCanType.Capacity).Sum();
                var averageFullFil = garbageCans.Any() ? garbageCans.Average(x => x.FulfiledPercentage) : 0;

                cameraPoints.Add(new CameraPointViewModel() 
                {
                    CameraPlace = place, 
                    NumberOfCans = numberOfCans, 
                    TotalCapacity = totalCapacity,
                    FulfiledPercentage = Math.Round(averageFullFil, 2)
                });
            }

            return View(cameraPoints);
        }

        public ActionResult AddGarbageCan(int cameraPlaceId)
        {
            var garbageCansType = _context.GarbageCanTypes.ToList();
            ViewData["GarbageCanTypesList"] = new SelectList(garbageCansType, "Id", "Model");

            return PartialView(new AddGarbageCanViewModel() { CameraPlaceId = cameraPlaceId });
        }

        [HttpPost]
        public async Task<ActionResult> AddGarbageCan(AddGarbageCanViewModel model)
        {
            var garbageCanType = _context.GarbageCanTypes.FirstOrDefault(g => g.Id == model.GarbageCanTypeId);
            var cameraPlace = _context.CameraPlaces.FirstOrDefault(c => c.Id == model.CameraPlaceId);
            if (garbageCanType != null && cameraPlace != null)
            {
                var random = new Random();
                var garbageCan = new GarbageCans()
                {
                    CameraPlace = cameraPlace,
                    CameraPlaceId = cameraPlace.Id,
                    GarbageCanType = garbageCanType,
                    GarbageCanTypeId = garbageCanType.Id,
                    FulfiledPercentage = random.Next(100)
                };

                await _context.GarbageCans.AddAsync(garbageCan);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("CameraPoints", "Home");
        }

        public ActionResult ViewCamera(int cameraPlaceId)
        {
            var camera = _context.CameraPlaces.FirstOrDefault(s => s.Id == cameraPlaceId);
            if (camera != null && !string.IsNullOrWhiteSpace(camera.PathToImage))
            {
                var image = new Bitmap(camera.PathToImage);
                var imageInputData = new ImageInputData { Image = image };

                var result = DetectAndPaintImage(
                    imageInputData,
                    60,
                    80);

                ViewData["Result"] = "data:image/jpeg;base64," + result;
            }
            else
            {
                var noSignalPath = @"D:\Programming\ThirdYear\GarbageDetection\Images\photos\no signal.jpg";
                var bitmap = new Bitmap(noSignalPath);
                var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Jpeg);
                var byteImage = ms.ToArray();

                ViewData["Result"] = "data:image/jpeg;base64," + Convert.ToBase64String(byteImage); ;
            }

            return View();
        }

        private string DetectAndPaintImage(ImageInputData imageInputData, float minProbabilityToShow, float minThresholdToShow)
        {
            //Predict the objects in the image
            _objectDetectionService.DetectObjectsUsingModel(imageInputData, minProbabilityToShow, minThresholdToShow);
            var img = _objectDetectionService.DrawBoundingBox(imageInputData.Image);

            using (MemoryStream m = new MemoryStream())
            {
                img.Save(m, img.RawFormat);
                byte[] imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                return Convert.ToBase64String(imageBytes);
            }
        }
    }
}
