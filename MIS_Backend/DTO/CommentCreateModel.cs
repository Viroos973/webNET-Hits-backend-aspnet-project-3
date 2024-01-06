﻿using MIS_Backend.Database.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class CommentCreateModel
    {
        [Required]
        [MinLength(1)]
        public string Content { get; set; }

        [Required]
        public Guid ParentId { get; set; }
    }
}