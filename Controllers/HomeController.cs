using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CadueusJQM.Models;
using Newtonsoft.Json;

namespace CadueusJQM.Controllers
{
    public class HomeController : Controller
    {
        private DataStore DS { get { return new DataStore() { Server = this.Server }; } set{} }

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Favorites(string data)
        {
            if (data != null && data!="null")
            {
                var ids = JsonConvert.DeserializeObject<int[]>(data);
                var results = DS.GetProvidersByIds(ids);
                ViewBag.Providers = results;
            }
            else
            {
                ViewBag.Providers=new MedicalProvider[]{};
            }
            return View();
        }

        public ActionResult Information()
        {
            return View();
        }

        public ActionResult ShowMap(int Id)
        {
            var coord = DS.GetLatLong(Id);
            ViewBag.LatLong = coord;
            return View();
        }

        [HttpPost]
        public ActionResult FindProviders(SearchViewModel criteria)
        {
            criteria.City = criteria.City!=null?criteria.City.Replace("_", " "):string.Empty;
            criteria.Specialty = criteria.Specialty != null ? criteria.Specialty.Replace("_", " ") : string.Empty;

            List<SearchCriteria> list = new List<SearchCriteria>();
            if (criteria.City != string.Empty && !criteria.City.StartsWith("No"))
                list.Add(new SearchCriteria() { Selector = "city", Parameter = criteria.City });
            if (criteria.Specialty != string.Empty && !criteria.Specialty.StartsWith("No"))
                list.Add(new SearchCriteria() { Selector = "specialty", Parameter = criteria.Specialty });
            if (criteria.LastName!=null && criteria.LastName != "-1" && criteria.LastName != string.Empty)
                list.Add(new SearchCriteria() { Selector = "lastName", Parameter = criteria.LastName});
            var results = DS.GetProvidersByDimensionCollection(list.ToArray());
            ViewBag.Providers = results;
            return View();
        }

        public ActionResult Search()
        {
            var specialties = DS.GetDimensions("specialty").ToList();
            specialties.Insert(0, "No Specialty Selected");
            ViewBag.Specialties = specialties;

            var cities = DS.GetDimensions("city").ToList();
            cities.Insert(0, "No City Selected");
            ViewBag.Cities = cities;

            return View();
        }
    }

    public class SearchViewModel
    {
        public string City { get; set; }
        public string Specialty { get; set; }
        public string LastName { get; set; }
    }

    public class MPIds
    {
        public int[] IdCollection { get; set; }
    }
}
