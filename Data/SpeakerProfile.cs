using AutoMapper;
using CoreCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Data
{
    public class SpeakerProfile: Profile
    {
        public SpeakerProfile()
        {
            CreateMap<Speaker, SpeakerModel>().ReverseMap(); ;
        }
    }
}
