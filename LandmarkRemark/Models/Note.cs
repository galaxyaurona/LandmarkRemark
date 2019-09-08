using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LandmarkRemark.Models
{
    public struct NoteDataAnnotationError
    {
        public const string CONTENT_IS_REQUIRED = "Content is required";
        public const string LAT_IS_REQUIRED = "Lat is required";
        public const string LNG_IS_REQUIRED = "Lng is required";
        public const string USER_ID_IS_REQUIRED = "User is required";
     
    }
    public class Note : EntityBase
    {
        [Required(ErrorMessage = NoteDataAnnotationError.CONTENT_IS_REQUIRED)]
        public string Content { get; set; }
        [Required(ErrorMessage = NoteDataAnnotationError.LAT_IS_REQUIRED)]
        public double Lat { get; set; }
        [Required(ErrorMessage = NoteDataAnnotationError.LNG_IS_REQUIRED)]
        public double Lng { get; set; }
        public DateTime Timestamp { get; set; }
        [Required(ErrorMessage = NoteDataAnnotationError.USER_ID_IS_REQUIRED)]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User Owner { get; set; }
    }
}