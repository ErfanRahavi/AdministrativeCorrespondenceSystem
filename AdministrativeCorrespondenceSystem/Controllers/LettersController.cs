using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdministrativeCorrespondenceSystem.Models;
using AdministrativeCorrespondenceSystem.Data;
using AdministrativeCorrespondenceSystem.Helpers;
using AdministrativeCorrespondenceSystem.DTOs;

namespace AdministrativeCorrespondenceSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LettersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LettersController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllLetters()
        {
            var letters = await _context.Letters
                .Include(l => l.LetterType)
                .Include(l => l.Sender)
                .Include(l => l.Receiver)
                .Select(l => new LetterDto
                {
                    Id = l.Id,
                    Subject = l.Subject,
                    Body = l.Body,
                    SenderName = l.Sender != null ? l.Sender.Name : "Unknown",
                    ReceiverName = l.Receiver != null ? l.Receiver.Name : "Unknown",
                    SentDate = l.SentDate == DateTime.MinValue ? null : l.SentDate,
                    LetterTypeName = l.LetterType != null ? l.LetterType.Name : "Unknown",
                    ParentLetterId = l.ParentLetterId,
                    AttachmentPath = l.AttachmentPath
                })
                .ToListAsync();

            return Ok(letters);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetLetterById(int id)
        {
            var letter = await _context.Letters
                .Include(l => l.LetterType)
                .Include(l => l.Sender)
                .Include(l => l.Receiver)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (letter == null)
                return NotFound("Letter not found.");

            var letterDto = new LetterDetailDto
            {
                Id = letter.Id,
                Subject = letter.Subject,
                Body = letter.Body,
                SentDate = letter.SentDate,
                Sender = letter.Sender?.Name,
                Receiver = letter.Receiver?.Name,
                LetterTypeName = letter.LetterType?.Name,
                AttachmentPath = letter.AttachmentPath
            };

            return Ok(letterDto);
        }


        [HttpPost]
        public async Task<IActionResult> CreateLetter([FromForm] CreateLetterRequest request, IFormFile? file)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? attachmentPath = null;
            if (file != null && file.Length > 0)
            {
                try
                {
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    attachmentPath = FileHelper.SaveFile(file, folderPath);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"File upload failed: {ex.Message}");
                }
            }

            var letter = new Letter
            {
                Subject = request.Subject,
                Body = request.Body,
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                LetterTypeId = request.LetterTypeId,
                Status = LetterStatus.Pending,
                AttachmentPath = attachmentPath,
                SentDate = DateTime.UtcNow
            };

            _context.Letters.Add(letter);
            await _context.SaveChangesAsync();


            var letterDto = await _context.Letters
                .Where(l => l.Id == letter.Id)
                .Select(l => new LetterDto
                {
                    Id = l.Id,
                    Subject = l.Subject,
                    Body = l.Body,
                    SenderName = l.Sender.Name,
                    ReceiverName = l.Receiver.Name,
                    LetterTypeName = l.LetterType.Name,
                    SentDate = l.SentDate,
                    AttachmentPath = l.AttachmentPath
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetLetterById), new { id = letterDto.Id }, letterDto);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLetter(int id, [FromBody] UpdateLetterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingLetter = await _context.Letters.FindAsync(id);
            if (existingLetter == null)
                return NotFound("Letter not found.");

            existingLetter.Subject = request.Subject;
            existingLetter.Body = request.Body;
            existingLetter.SenderId = request.SenderId;
            existingLetter.ReceiverId = request.ReceiverId;
            existingLetter.LetterTypeId = request.LetterTypeId;

            _context.Letters.Update(existingLetter);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLetter(int id)
        {
            var letter = await _context.Letters.FindAsync(id);
            if (letter == null)
                return NotFound("Letter not found.");

            if (!string.IsNullOrEmpty(letter.AttachmentPath))
            {
                try
                {
                    FileHelper.DeleteFile(letter.AttachmentPath);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Failed to delete file: {ex.Message}");
                }
            }

            _context.Letters.Remove(letter);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost("{id}/reply")]
        public async Task<IActionResult> ReplyToLetter(int id, [FromBody] ReplyLetterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var originalLetter = await _context.Letters.FindAsync(id);
            if (originalLetter == null)
                return NotFound("Original letter not found.");

            var replyLetter = new Letter
            {
                Subject = request.Subject,
                Body = request.Body,
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                ParentLetterId = id,
                SentDate = DateTime.UtcNow,
                LetterTypeId = request.LetterTypeId
            };

            _context.Letters.Add(replyLetter);
            await _context.SaveChangesAsync();

            var replyDto = await _context.Letters
                .Where(l => l.Id == replyLetter.Id)
                .Select(l => new LetterDto
                {
                    Id = l.Id,
                    Subject = l.Subject,
                    Body = l.Body,
                    SenderName = l.Sender.Name,
                    ReceiverName = l.Receiver.Name,
                    SentDate = l.SentDate,
                    LetterTypeName = l.LetterType.Name,
                    ParentLetterId = l.ParentLetterId
                })
                .FirstOrDefaultAsync();

            return Ok(replyDto);
        }


        [HttpPost("{id}/forward")]
        public async Task<IActionResult> ForwardLetter(int id, [FromBody] ForwardLetterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var originalLetter = await _context.Letters
                .Include(l => l.LetterType)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (originalLetter == null)
                return NotFound("Original letter not found.");

            var forwardLetter = new Letter
            {
                Subject = originalLetter.Subject,
                Body = originalLetter.Body,
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                ParentLetterId = id,
                SentDate = DateTime.UtcNow,
                LetterTypeId = originalLetter.LetterTypeId,
                AttachmentPath = originalLetter.AttachmentPath
            };

            _context.Letters.Add(forwardLetter);
            await _context.SaveChangesAsync();

            var forwardDto = await _context.Letters
                .Where(l => l.Id == forwardLetter.Id)
                .Select(l => new LetterDto
                {
                    Id = l.Id,
                    Subject = l.Subject,
                    Body = l.Body,
                    SenderName = l.Sender.Name,
                    ReceiverName = l.Receiver.Name,
                    SentDate = l.SentDate,
                    LetterTypeName = l.LetterType.Name,
                    ParentLetterId = l.ParentLetterId,
                    AttachmentPath = l.AttachmentPath
                })
                .FirstOrDefaultAsync();

            return Ok(forwardDto);
        }


        [HttpGet("report")]
        public async Task<IActionResult> GetLetterReport(
            [FromQuery] int? senderId,
            [FromQuery] int? receiverId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? subject,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Letters
                .Include(l => l.LetterType)
                .Include(l => l.Sender)
                .Include(l => l.Receiver)
                .AsQueryable();

            if (senderId.HasValue)
                query = query.Where(l => l.SenderId == senderId.Value);

            if (receiverId.HasValue)
                query = query.Where(l => l.ReceiverId == receiverId.Value);

            if (startDate.HasValue)
                query = query.Where(l => l.SentDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(l => l.SentDate <= endDate.Value);

            if (!string.IsNullOrEmpty(subject))
                query = query.Where(l => l.Subject.Contains(subject));

            var totalRecords = await query.CountAsync();
            var letters = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var reportData = letters.Select(l => new LetterDto
            {
                Id = l.Id,
                Subject = l.Subject,
                Body = l.Body,
                SenderName = l.Sender?.Name,
                ReceiverName = l.Receiver?.Name,
                SentDate = l.SentDate,
                LetterTypeName = l.LetterType?.Name,
                ParentLetterId = l.ParentLetterId,
                AttachmentPath = l.AttachmentPath
            });

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = reportData
            });
        }

    }
}
