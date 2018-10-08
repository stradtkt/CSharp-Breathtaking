using System.ComponentModel.DataAnnotations;

namespace Breathtaking.Models
{
    public class Like
    {
        [Key]
        public int like_id {get;set;}
        public int review_id {get;set;}
        public Review Review {get;set;}
        public int user_id {get;set;}
        public User User {get;set;}
        public byte user_liked {get;set;}
    }
}