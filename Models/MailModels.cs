using System.ComponentModel.DataAnnotations;

namespace Breathtaking.Models
{
    public class MailModels
    {
        [Required(ErrorMessage="Name is required")]
        public string name {get;set;}
        [Required(ErrorMessage="Email is required")]
        [EmailAddress]
        public string email {get;set;}
        [Required(ErrorMessage="Subject is required")]
        public string subject {get;set;}
        [Required(ErrorMessage="Message is required")]
        public string msg {get;set;}
    }
}