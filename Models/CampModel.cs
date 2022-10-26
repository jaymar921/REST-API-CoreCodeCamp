using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Models
{
    /*
     * This class will be returned to the users instead of
     * the entity itself, we used automapper
     */
    public class CampModel
    {
        [Required]
        [StringLength(100)] // max string length of 100
        public string Name { get; set; }
        [Required]
        public string Moniker { get; set; }
        public DateTime EventDate { get; set; } = DateTime.MinValue;
        [Range(1,100)]
        public int Length { get; set; } = 1;

        public LocationModel Location { get; set; }

        public ICollection<TalkModel> Talks { get; set; }
    }
}
