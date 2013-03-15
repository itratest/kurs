﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;


namespace MusicService.Models
{
    [Table("Post")]
    public class Post
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PostId { get; set; }
        public string Message { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
    }

     
    [Table("UsersPosts")]
    public class UsersPosts
    {
        [Key]
        public UserProfile User { get; set; }
        public Post Post { get; set; }
    }

    [Table("TracksInPost")]
    public class TracksInPost
    {
        [Key]
        public Post Post { get; set; }
        public Track Track { get; set; }
    }
}