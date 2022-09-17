using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DemoMusicMVC.ViewModel
{
    public class SongViewModel
    {
        [Key]
        public int songId { get; set; }
        [Required, MaxLength(60, ErrorMessage = "Name Cannot Exceed 60 Characters")]
        [Display(Name = "Song Name")]
        public string songName { get; set; }
        [Display(Name = "Photo For the Song")]
        public IFormFile photo { get; set; }
        [Required]
        [Display(Name = "Song File")]
        [DataType(DataType.Upload)]
        /*[UploadValidation]*/
        public IFormFile song { get; set; }
    }
    /*public class UploadValidation : ValidationAttribute
    {
        private string FileIsNull = "No file uploaded!";
        private string FileTooBig = "Uploaded file is too big (maximum size is " + MaximumFileSize + " B)";
        private string FileIsNotImg = "This file is not in a supported format! (.mp3, .mp4, .m4a, or .ogg)";

        private const int MaximumFileSize = 5 * 1024 * 1024;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IFormFile file = value as IFormFile;
            *//*if (file == null)
            {
                return new ValidationResult(FileIsNull);
            }*//*

            if (file.Length > MaximumFileSize)
            {
                return new ValidationResult(FileTooBig);
            }

            string ext = Path.GetExtension(file.FileName);
            var isimg = false;
            switch (ext)
            {
                case ".mp3": isimg = true; break;
                case ".mp4": isimg = true; break;
                case ".ogg": isimg = true; break;
                case ".m4a": isimg = true; break;
            }

            if (!isimg)
            {
                return new ValidationResult(FileIsNotImg);
            }
            return ValidationResult.Success;
        }
    }*/
}
