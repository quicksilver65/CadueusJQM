using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CadueusJQM.Models
{
    public class MedicalProvider
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int ZipCode { get; set; }
        public string Suite { get; set; }
        public string Specialty { get; set; }
        public string Facility { get; set; }
        public long PhoneNumber { get; set; }
    }
}