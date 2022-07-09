using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApi.ViewModel
{
    public class MovieViewModel
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string Language { get;  set; }
    }
}
