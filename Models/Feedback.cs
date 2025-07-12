using System;
using System.ComponentModel.DataAnnotations;

namespace InsanK.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        
        [Required]
        public string Subject { get; set; }
        
        [Required]
        public string Message { get; set; }
        
        public int? Rating { get; set; }
        
        public string UserEmail { get; set; }
        
        public string UserName { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public bool IsResolved { get; set; } = false;
    }
}