using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace CadueusJQM.Models
{
    public class DataStore
    {
        public HttpServerUtilityBase Server { get; set; }
        public string[] GetDimensions(string field)
        {
            var results = new string[] { };
            switch (field)
            {
                case "city":
                    results = ProviderQuery().Select(p => p.City).Distinct().ToArray();
                    break;
                case "specialty":
                    results = ProviderQuery().Select(p => p.Specialty).Distinct().ToArray();
                    break;

            }
            return results.OrderBy(p => p).ToArray();
        }

        public LatLong GetLatLong(int id)
        {
            var mp = GetProviderById(id);
            var url = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0},+{1},+{2}&sensor=false", mp.Address, mp.City, mp.State);
            WebClient wc = new WebClient();
            var stream = wc.OpenRead(url);
            var doc = XDocument.Load(stream);
            var element = doc.Element("GeocodeResponse").Element("result").Element("geometry").Element("location");
            var x = element.Element("lat").Value;
            var latLong = new LatLong()
            {
                Latitude = element.Element("lat").Value,
                Longtitude = element.Element("lng").Value
            };

            stream.Close();
            return latLong;
        }


        public MedicalProvider[] GetProvidersByDimensionCollection(SearchCriteria[] criteria)
        {
            var query = BuildQuery(criteria);
            var temp = query.OrderBy(p => p.LastName).Take(25).ToArray();
            return temp;
        }

        public MedicalProvider[] GetProviderByLastName(string lastName)
        {
            var query = ProviderQuery().Where(p => p.LastName.ToUpper().StartsWith(lastName.ToUpper()));
            return query.ToArray();
        }
        public MedicalProvider[] GetProvidersByIds(int[] ids)
        {
            var query = ProviderQuery().Where(p => ids.Contains(p.Id));
            return query.ToArray();
        }
        public MedicalProvider GetProviderById(int Id)
        {
            var query = ProviderQuery().FirstOrDefault(p => p.Id == Id);
            return query;
        }

        private IQueryable<MedicalProvider> ProviderQuery()
        {
            return GetData().AsQueryable();
        }



        private IQueryable<MedicalProvider> BuildQuery(SearchCriteria[] criteria)
        {
            var query = ProviderQuery().ToList();

            foreach (var search in criteria)
            {
                switch (search.Selector)
                {
                    case "city":
                        query = query.Where(p => p.City == search.Parameter).ToList();
                        break;
                    case "specialty":
                        query = query.Where(p => p.Specialty == search.Parameter).ToList();
                        break;
                    case "zipcode":
                        query = query.Where(p => p.ZipCode.ToString() == search.Parameter).ToList();
                        break;
                    case "facility":
                        query = query.Where(p => p.Facility == search.Parameter).ToList();
                        break;
                    case "lastName":
                        query = query.Where(p => p.LastName.ToUpper().StartsWith(search.Parameter.ToUpper())).ToList();
                        break;

                }
            }
            return query.AsQueryable();
        }

        private MedicalProvider[] GetData()
        {
            if (System.Web.HttpContext.Current.Cache["providerData"] == null)
            {
                var jsonString = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/providerData.json"));
                var collection = JsonConvert.DeserializeObject<MedicalProvider[]>(jsonString);
                System.Web.HttpContext.Current.Cache["providerData"] = collection;
                return collection;
            }
            else
            {
                return (MedicalProvider[])System.Web.HttpContext.Current.Cache["providerData"];
            }


        }

    }
}