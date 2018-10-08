using System.ComponentModel.DataAnnotations;

namespace Breathtaking.Models
{
    public class Like
    {
        [Key]
        public int like_id {get;set;}
        public int review_id {get;set;}
        public virtual Review Review {get;set;}
        public int user_id {get;set;}
        public int user_liked {get;set;}
        public virtual User User {get;set;}
    }
}