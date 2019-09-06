using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LandmarkRemark.Models
{
    public class Location : EntityBase
    {
        
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Note { get; set; }
        public DateTime Timestamp { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User Owner { get; set; }
    }
}