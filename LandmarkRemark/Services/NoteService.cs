using LandmarkRemark.Models;
using LandmarkRemark.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandmarkRemark.Services
{
    public interface INoteService
    {
        IEnumerable<Note> FindNoteContainWords(string searchTerm);
        Task<ModifyingActionResult<Note>> AddNote(Note note);
    }
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;
        private readonly IUserRepository _userRepository;
        public NoteService(INoteRepository noteRepository, IUserRepository userRepository)
        {
            _noteRepository = noteRepository;
            _userRepository = userRepository;
        }

        public async Task<ModifyingActionResult<Note>> AddNote(Note note)
        {

            var errors = new List<string>();
            var userExisted = await _userRepository.UserExistAsync(note.UserId);
            if (!userExisted)
            {
                errors.Add("User with this id does not exist in tabase");
            }
            var result = new ModifyingActionResult<Note>()
            {
                Success = false,
                Errors = errors,
            };
            if (errors.Count > 0)
            {
                return result;
            }

            var addedNote = await _noteRepository.AddAsync(note);
            result.Success = true;
            result.Data = addedNote;
            return result;
        }

        public IEnumerable<Note> FindNoteContainWords(string searchTerm)
        {
            var notesWithTermInContent = _noteRepository.Find(x => x.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            var usersWithTermInUsername = _userRepository.Find(x => x.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            // map each user to its notes
            var notesFromUsers = usersWithTermInUsername.Select(user =>
            {
                // inverse the relationship
                return user.Notes.Select(note => {
                    note.Owner = user;

                    return note;
                });
            });
            var result = notesWithTermInContent;
            foreach (var notes in notesFromUsers)
            {
                // merge the list, and remove duplicates note with same id
                result = result.Union(notes).GroupBy(x => x.Id).Select(g => g.First());
            }
            return result.ToList();
        }
    }
}
