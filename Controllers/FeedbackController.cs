using InsanK.Data;
using InsanK.DTOs;
using InsanK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsanK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeedbackResponseDTO>>> GetFeedbacks()
        {
            var feedbacks = await _context.Feedbacks
                .Select(f => new FeedbackResponseDTO
                {
                    Id = f.Id,
                    Subject = f.Subject,
                    Message = f.Message,
                    Rating = f.Rating,
                    UserEmail = f.UserEmail,
                    UserName = f.UserName,
                    CreatedAt = f.CreatedAt,
                    IsResolved = f.IsResolved
                })
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return Ok(feedbacks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FeedbackResponseDTO>> GetFeedback(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);

            if (feedback == null)
            {
                return NotFound();
            }

            var feedbackResponse = new FeedbackResponseDTO
            {
                Id = feedback.Id,
                Subject = feedback.Subject,
                Message = feedback.Message,
                Rating = feedback.Rating,
                UserEmail = feedback.UserEmail,
                UserName = feedback.UserName,
                CreatedAt = feedback.CreatedAt,
                IsResolved = feedback.IsResolved
            };

            return Ok(feedbackResponse);
        }

        [HttpPost]
        public async Task<ActionResult<FeedbackResponseDTO>> PostFeedback(FeedbackDTO feedbackDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var feedback = new Feedback
            {
                Subject = feedbackDto.Subject,
                Message = feedbackDto.Message,
                Rating = feedbackDto.Rating,
                UserEmail = feedbackDto.UserEmail,
                UserName = feedbackDto.UserName,
                CreatedAt = DateTime.Now,
                IsResolved = false
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            var feedbackResponse = new FeedbackResponseDTO
            {
                Id = feedback.Id,
                Subject = feedback.Subject,
                Message = feedback.Message,
                Rating = feedback.Rating,
                UserEmail = feedback.UserEmail,
                UserName = feedback.UserName,
                CreatedAt = feedback.CreatedAt,
                IsResolved = feedback.IsResolved
            };

            return CreatedAtAction(nameof(GetFeedback), new { id = feedback.Id }, feedbackResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeedback(int id, FeedbackDTO feedbackDto)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            feedback.Subject = feedbackDto.Subject;
            feedback.Message = feedbackDto.Message;
            feedback.Rating = feedbackDto.Rating;
            feedback.UserEmail = feedbackDto.UserEmail;
            feedback.UserName = feedbackDto.UserName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbackExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPatch("{id}/resolve")]
        public async Task<IActionResult> ResolveFeedback(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            feedback.IsResolved = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbackExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedbacks.Any(e => e.Id == id);
        }
    }
}