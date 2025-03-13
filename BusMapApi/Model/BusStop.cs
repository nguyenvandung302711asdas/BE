using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusMapApi.Model
{
    public class BusStop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
    }
}