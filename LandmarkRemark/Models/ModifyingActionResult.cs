using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandmarkRemark.Models
{
    // this class is to act as data transfer object
    // ideally to let service communicate with
    // controller or other services
    // the result of its Create,Delete, Update operation
    public class ModifyingActionResult<T> where T : EntityBase
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }

        public T Data { get; set; }
     
    }
}
