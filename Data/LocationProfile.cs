using AutoMapper;
using CoreCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Data
{
    public class LocationProfile: Profile
    {
        public LocationProfile()
        {
            CreateMap<Location, LocationModel>();
            CreateMap<LocationModel, Location>();
        }
    }
}
