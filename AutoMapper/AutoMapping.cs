using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CinemaApi.Models;
using CinemaApi.ViewModel;

namespace CinemaApi.AutoMapper
{
    public class AutoMapping :Profile
    {
        public AutoMapping()
        {
            CreateMap<Movie, MovieViewModel>().ReverseMap();
        }
    }
}
