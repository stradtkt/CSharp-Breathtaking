using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Breathtaking.Models
{
    public class Review : BaseEntity
    {
        [Key]
        public int review_id {get;set;}
        public int user_id {get;set;}
        public User User {get;set;}
        [Required]
        [Display(Name="Rating")]
        public int rating {get;set;}
        [Required(ErrorMessage="Review is required")]
        [MinLength(5, ErrorMessage="Review has a min length of 5 characters")]
        [MaxLength(2000, ErrorMessage="Review has a max length of 2000 characters")]
        [Display(Name="Review")]
        public string review {get;set;}
        [Required]
        [DataType(DataType.Date)]
        [Display(Name="Start Date")]
        public DateTime start_visit_date {get;set;}
        [Required]
        [DataType(DataType.Date)]
        [Display(Name="End Date")]
        public DateTime end_visit_date {get;set;}
        public List<Like> Likes {get;set;}
        public Review()
        {
            Likes = new List<Like>();
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
        }
    }
}