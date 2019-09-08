using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LandmarkRemark.Models
{
    public struct UserDataAnnotationError
    {
        public const string USERNAME_IS_REQUIRED = "Username is required";
    }
    public class User : EntityBase
    {
        [Required(ErrorMessage= UserDataAnnotationError.USERNAME_IS_REQUIRED)]
        public string Username { get; set; }
        public List<Note> Notes { get; set; }
    }
}