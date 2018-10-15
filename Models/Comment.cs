using System;
using System.ComponentModel.DataAnnotations;

namespace Breathtaking.Models
{
    public class Comment : BaseEntity
    {
        [Key]
        public int comment_id {get;set;}
        public int review_id {get;set;}
        public int user_id {get;set;}
        public User User {get;set;}
        public Review Review {get;set;}
        public Comment() 
        {
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
        }
    }
}