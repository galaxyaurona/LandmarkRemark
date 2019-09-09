using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LandmarkRemark.Models;
using LandmarkRemark.Services;

namespace LandmarkRemark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AddNote(Note note)
        {
            note.Timestamp = DateTime.Now;
            var result = await _noteService.AddNote(note);
            if (result.Success)
            {
                return CreatedAtAction(nameof(AddNote), result.Data);
            }
            else
            {
                return UnprocessableEntity(new ErrorResponse()
                {
                    Errors = result.Errors?.ToList(),
                    Message = "Error creating note"
                });
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public ActionResult<IEnumerable<Note>> FindNoteContainWords(string searchTerm)
        {
            // don't search for empty search term
            if (String.IsNullOrWhiteSpace(searchTerm))
                return UnprocessableEntity(new ErrorResponse()
                {
                    Errors = new List<string>
                     {
                         "Search term is empty"
                     },
                    Message = "Error searching note"
                });
            //finding terms
            var result = _noteService.FindNoteContainWords(searchTerm);
            // avoid circular reference when serialize
            var resultExcludeCircularReference = result.Select(x => { x.Owner.Notes = null; return x; });
            //return search term for front end discard invalid results
            return Ok(new { searchTerm, result = resultExcludeCircularReference});
        }
    }
}