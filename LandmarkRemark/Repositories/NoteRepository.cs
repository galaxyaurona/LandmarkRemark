using LandmarkRemark.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandmarkRemark.Repositories
{
    public interface INoteRepository
    {
        IEnumerable<Note> Find(Func<Note, bool> predicate);
        Task<Note> AddAsync(Note note);
        Note Add(Note note);
    }
    public class NoteRepository :INoteRepository
    {
        private readonly LandmarkRemarkContext _context;

        public NoteRepository(LandmarkRemarkContext context)
        {
            _context = context;
        }
        // only include username, avoid circular referece
        private IIncludableQueryable<Note, User> _noteWithIncludes
        {
            get
            {
                return _context.Notes.Include(x => x.Owner);
            }
        }

        // need to do pagination here because potentially thousand of note

        public IEnumerable<Note> Find(Func<Note, bool> predicate)
        {
            return _noteWithIncludes.Where(predicate);

        }

        public async Task<Note> AddAsync(Note note)
        {
            var result = await _context.Notes.AddAsync(note);
            // to trigger validation
            await _context.SaveChangesAsyncWithValidation();
            
            return result.Entity;
        }

        public Note Add(Note note)
        {
            var result =  _context.Notes.Add(note);
            // to trigger validation
             _context.SaveChanges();

            return result.Entity;
        }
    }
}
