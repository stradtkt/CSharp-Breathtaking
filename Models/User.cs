using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Breathtaking.Models
{
    public class User : BaseEntity
    {
        [Key]
        public int user_id {get;set;}
        public string first_name {get;set;}
        public string last_name {get;set;}
        public string email {get;set;}
        public string password {get;set;}
        public List<Comment> Comments {get;set;}
        public List<Review> Reviews {get;set;}
        public User()
        {
            Reviews = new List<Review>();
            Comments = new List<Comment>();
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
        }
    }   
}