using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandmarkRemark.Models
{
    public class ErrorResponse
    {
        public List<string> Errors { get; set; }
        public string Message { get; set; }
        public ErrorResponse()
        {
            Errors = new List<string>();
        }

    }
}
