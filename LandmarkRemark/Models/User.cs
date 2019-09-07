using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LandmarkRemark.Models
{
    public class User : EntityBase
    {
        [Required(ErrorMessage="Username is required")]
        public string Username { get; set; }
        public List<Location> Locations { get; set; }
    }
}