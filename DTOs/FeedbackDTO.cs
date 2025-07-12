using System;
using System.ComponentModel.DataAnnotations;

namespace InsanK.DTOs
{
    public class FeedbackDTO
    {
        [Required(ErrorMessage = "Konu alanı zorunludur")]
        public string Subject { get; set; }
        
        [Required(ErrorMessage = "Mesaj alanı zorunludur")]
        public string Message { get; set; }
        
        public int? Rating { get; set; }
        
        public string UserEmail { get; set; }
        
        public string UserName { get; set; }
    }
    
    public class FeedbackResponseDTO
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int? Rating { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsResolved { get; set; }
    }
}