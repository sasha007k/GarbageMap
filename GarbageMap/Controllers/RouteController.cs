using GarbageMap.Models;
using GarbageMap.Models.ApiModels;
using GarbageMap.Models.DbModels;
using GarbageMap.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GarbageMap.Controllers
{
    public class RouteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string APIKEY = "AIzaSyC3amkviJ3ysQSwHyFWEZBq2hWrRjobabw";
        private const double CAR_CAPACITY = 15000.0;

        public RouteController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await BuildRoute());
        }

        public async Task<RouteModel> BuildRoute()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var startPoint = new PointModel // start point
            {
                Address = "Start point",
                Latitude = 49.812399,
                Longitude = 24.001771
            };

            var endPoint = new PointModel // end point
            {
                Address = "End point",
                Latitude = 49.900763,
                Longitude = 24.034930
            };

            var points = new List<PointModel>();
            var places = GetNotGreenCameraPlaces(userId);

            var approximateCapacity = 0.0;
            foreach (var place in places)
            {
                approximateCapacity += place.TotalCapacity;
            }


            points.Add(startPoint);
            foreach (var place in places)
            {
                points.Add(new PointModel // garbage points
                {
                    Address = $"{place.CameraPlace.Address.Street}, {place.CameraPlace.Address.HouseNumber}",
                    Latitude = place.CameraPlace.Latitude,
                    Longitude = place.CameraPlace.Longitude,
                    CameraPlaceId = place.CameraPlace.Id
                });
            }
            points.Add(endPoint);

            //var matrix = await GetMatrixWithGoogleApi(points);
            var matrix = GetMatrixWithCoordinates(points); 
            var pointsOrder = GetPointsOrder(ref matrix, 0);

            var orderedPoints = new List<PointModel>();
            for (int i = 0; i < pointsOrder.Count; i++)
            {
                var index = pointsOrder[i];
                orderedPoints.Add(points[index]);
            }

            var routeModel = new RouteModel()
            {
                StartPoint = startPoint,
                EndPoint = endPoint,
                Points = orderedPoints
            };

            return routeModel;
        }

        private List<CameraPlaceAndTotalCapacity> GetNotGreenCameraPlaces(string userId)
        {
            var notGreenPlaces = new List<CameraPlaceAndTotalCapacity>();
            var cameraPlaces = _context.CameraPlaces.Where(x => x.OrganizationId == userId).Include(x => x.Address).ToList();
            var garbageCans = _context?.GarbageCans?.Include(g => g.CameraPlace)
                .Include(g => g.GarbageCanType).ToList();
            foreach (var place in cameraPlaces)
            {
                var cansOnSpecificPlace = garbageCans.Where(x => x.CameraPlaceId == place.Id);
                var avarage = 0.0;
                if (cansOnSpecificPlace.Any())
                {
                    avarage = cansOnSpecificPlace.Average(x => x.FulfiledPercentage);
                }

                if (avarage >= 80)
                {
                    var totalCapacity = cansOnSpecificPlace.Select(g => g.GarbageCanType.Capacity).Sum();
                    notGreenPlaces.Add(new CameraPlaceAndTotalCapacity 
                    { CameraPlace = place, AverageFullFil = avarage, TotalCapacity = totalCapacity });
                }
            }

            return notGreenPlaces;
        }

        private int GetDistance(double latitude, double longitude, double otherLatitude, double otherLongitude)
        {
            var d1 = latitude * (Math.PI / 180.0);
            var num1 = longitude * (Math.PI / 180.0);
            var d2 = otherLatitude * (Math.PI / 180.0);
            var num2 = otherLongitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return (int)(6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3))));
        }

        private async Task<int[,]> GetMatrixWithGoogleApi(List<PointModel> points)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://maps.googleapis.com/maps/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var nfi = new NumberFormatInfo
                {
                    NumberDecimalSeparator = "."
                };

                var pointsAmount = points.Count;
                var matrix = new int[pointsAmount, pointsAmount];
                for (int i = 0; i < pointsAmount; i++)
                {
                    var startLatitude = points[i].Latitude.ToString(nfi);
                    var startLongitude = points[i].Longitude.ToString(nfi);
                    for (int j = i + 1; j < pointsAmount; j++)
                    {
                        var endLatitude = points[j].Latitude.ToString(nfi);
                        var endLongitude = points[j].Longitude.ToString(nfi);

                        var originPoint = $"{startLatitude},{startLongitude}";
                        var destPoint = $"{endLatitude},{endLongitude}";

                        var apiCall = $"api/directions/json?origin={originPoint}&destination={destPoint}&key={APIKEY}";
                        var response = await client.GetAsync(apiCall);
                        var jsonStr = await response.Content.ReadAsStringAsync();
                        var dist = GetDistanceJson(jsonStr);
                        matrix[i, j] = dist;
                        matrix[j, i] = dist;
                    }
                }

                return matrix;
            }
        }

        private int[,] GetMatrixWithCoordinates(List<PointModel> points)
        {
            var pointsAmount = points.Count;
            var matrix = new int[pointsAmount, pointsAmount];
            for (int i = 0; i < pointsAmount; i++)
            {
                for (int j = i + 1; j < pointsAmount; j++)
                {
                    var dist = GetDistance(points[i].Latitude, points[i].Longitude, points[j].Latitude, points[j].Longitude);

                    matrix[i, j] = dist;
                    matrix[j, i] = dist;
                }
            }

            return matrix;
        }

        private int GetDistanceJson(string jsonStr)
        {
            JObject json = JObject.Parse(jsonStr);

            var routesKey = json["routes"];
            var legsKey = routesKey[0]["legs"];
            var distanceKey = legsKey[0]["distance"];
            return distanceKey.Value<int>("value");
        }

        private List<int>GetPointsOrder(ref int[,] matrix, int startRow)
        {
            var pointsOrder = new List<int>();

            do
            {
                var resultJ = SearchMinimumInRow(ref matrix, startRow);
                startRow = resultJ;                
                if (resultJ != -1)
                {
                    pointsOrder.Add(resultJ);
                    SetMaxInt(ref matrix, resultJ);
                }
            }
            while (pointsOrder.Count != matrix.GetLength(0) - 2);

            return pointsOrder;
        }

        private int SearchMinimumInRow(ref int[,] matrix, int row)
        {
            var length = matrix.GetLength(0);
            var nearestNaighbor = int.MaxValue;
            var resultJ = -1;
            for (int j = 0; j < length; j++)
            {
                if (matrix[row, j] < nearestNaighbor && row != j && j != 0 && j != length-1)
                {
                    nearestNaighbor = matrix[row, j];
                    resultJ = j;
                }
            }

            return resultJ;
        }

        private void SetMaxInt(ref int[,] matrix, int column)
        {
            var length = matrix.GetLength(0);
            for (int i = 0; i < length; i++)
            {
                matrix[i, column] = int.MaxValue;
            }
        }
    }
}
