using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DemoMusicMVC.Models
{
    public class Song
    {
        [Key]
        public int songId { get; set; }
        [Required, MaxLength(20, ErrorMessage = "Name Cannot Exceed 20 Characters")]
        [Display(Name = "Song Name")]
        public string songName { get; set; }
        [Required]
        public string photoPath { get; set; }
        [Required]
        public string songPath { get; set; }
    }
}
