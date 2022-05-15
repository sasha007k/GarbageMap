using System.Collections.Generic;
using System.Linq;
using GarbageMap.Models.DbModels;
using GarbageMap.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace GarbageMap.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult AllLocations()
        {
            var nodes = new List<TreeViewNode>();

            foreach (var region in _context.Regions)
            {
                nodes.Add(new TreeViewNode() { Id = region.Id.ToString(), ParentId = "#", Caption = region.Name });
            }

            nodes = nodes.OrderBy(n => n.Caption).ToList();

            foreach (var district in _context.Districts)
            {
                nodes.Add(new TreeViewNode() { Id = district.Region.Index + "-" + district.Index, ParentId = district.Region.Id.ToString(), Caption = district.Name });
            }

            foreach (var city in _context.Cities)
            {
                if (city.Region != null)
                {
                    nodes.Add(new TreeViewNode() { Id = city.Region.Index + "-" + city.Index, ParentId = city.Region.Id.ToString(), Caption = city.Name });
                }
                else if (city.District != null)
                {
                    nodes.Add(new TreeViewNode() { Id = city.District.Index + "-" + city.Index, ParentId = city.District.Region.Index + "-" + city.District.Index, Caption = city.Name });
                }
            }

            ViewBag.Json = JsonConvert.SerializeObject(nodes);
            return View();
        }

        private IEnumerable<RegionModel> GetRegionsList()
        {
            return _context.Regions.Select(s => new RegionModel() { Name = s.Name, Index = s.Index }).ToList();
        }

        public ActionResult AddDistrict()
        {
            ViewData["RegionsList"] = new SelectList(GetRegionsList(), "Index", "Name");

            return PartialView();
        }

        [HttpPost]
        public ActionResult AddDistrict(AddDistrictViewModel viewModel)
        {
            if (viewModel != null)
            {
                var region = _context.Regions.SingleOrDefault(r => r.Index == viewModel.RegionId);
                if (region != null)
                {
                    var district = new District()
                    {
                        Name = viewModel.Name,
                        Index = viewModel.Index,
                        Region = region,
                        RegionId = region.Id
                    };

                    _context.Districts.Add(district);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("AllLocations", "Admin");
        }

        private IEnumerable<DistrictModel> GetDistrictsList()
        {
            return _context.Districts.Select(s => new DistrictModel() { Name = s.Name, Index = s.Index }).ToList();
        }

        public ActionResult AddCity()
        {
            ViewData["RegionsList"] = new SelectList(GetRegionsList(), "Index", "Name");
            ViewData["DistrictsList"] = new SelectList(GetDistrictsList(), "Index", "Name");

            return PartialView();
        }

        [HttpPost]
        public ActionResult AddCity(AddCityViewModel viewModel)
        {
            if (viewModel != null)
            {
                var city = new City()
                {
                    Name = viewModel.Name,
                    Index = viewModel.Index,
                };

                if (viewModel.AddType == "Region")
                {
                    var region = _context.Regions.SingleOrDefault(r => r.Index == viewModel.RegionId);
                    city.Region = region;
                    city.RegionId = region.Id;
                }
                else
                {
                    var district = _context.Districts.SingleOrDefault(r => r.Index == viewModel.DistrictId);
                    city.District = district;
                    city.DistrictId = district.Id;
                }

                _context.Cities.Add(city);
                _context.SaveChanges();
            }

            return RedirectToAction("AllLocations", "Admin");
        }
    }
}
