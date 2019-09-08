using LandmarkRemark.Models;
using LandmarkRemark.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace LandmarkRemark.Tests.Repositories
{
    // This fixture should run only 1 before every test
    // to create a sample user
    public class NoteRepositoryTestBase : RepositoryTestBase, IDisposable
    {
        protected int _addedOwnerId = -1;
        protected User owner = new User()
        {
            Username = "Test user"
        };

        public NoteRepositoryTestBase()
        {
            var addedUser = _landmarkRemarkContext.Users.Add(owner);
            _landmarkRemarkContext.SaveChanges();
            _addedOwnerId = addedUser.Entity.Id;
        }
        // clean up user, make sure the test doesn't run in paralel with User repository
        public new void Dispose()
        {
            // Clear table users after every test
            _landmarkRemarkContext.Database.ExecuteSqlCommand("DELETE FROM Users");
            base.Dispose();
        }
    }
    // need this for repository test to run sequentially
    [Collection("RepositoryTest")]
    public class NoteRepositoryTest : NoteRepositoryTestBase, IDisposable
    {
        protected INoteRepository _noteRepository;

        private Note _testNote = new Note() {
            Lat = 100.00,
            Lng = -30.00,
            Content = "Test Note",
            UserId = -1
        };

        private Note _testNote2 = new Note()
        {
            Lat = 100.00,
            Lng = -30.00,
            Content = "Test Note 2",
            UserId = -1
        };


        private int _addedTestNoteId = -1;
    
        // setup test
        public NoteRepositoryTest() : base()
        {
            _noteRepository = new NoteRepository(_landmarkRemarkContext);
            // need to add owwner first to get userId

            _testNote.UserId = _addedOwnerId;
            _testNote2.UserId = _addedOwnerId;
            //
            var addedNote = _noteRepository.Add(_testNote);
            _noteRepository.Add(_testNote2);
            _addedTestNoteId = addedNote.Id;
        }
        // helper methods
        private bool FindContentContainWordTest(Note note)
        {
            return note.Content.Contains("tEst", StringComparison.OrdinalIgnoreCase);
        }
        private bool FindContentContainWordRandom(Note note)
        {
            return note.Content.Contains("random");
        }

        [Fact]
        public void Find_FoundNoteWithWordTestInContent()
        {
            var foundNotes = _noteRepository.Find(FindContentContainWordTest);
            Assert.Equal(2,foundNotes.Count());
            var firstNote = foundNotes.First();
            Assert.Equal(_addedTestNoteId, firstNote.Id);
            Assert.Equal(_addedOwnerId, firstNote.UserId);
            Assert.Equal("Test Note", firstNote.Content);
            Assert.Equal(100.00, firstNote.Lat);
            Assert.Equal(-30.00, firstNote.Lng);
        }
        [Fact]
        public void Find_DoesNotFoundNoteWithWordRandomInContent()
        {
            var foundNotes = _noteRepository.Find(FindContentContainWordRandom);
            Assert.Empty(foundNotes);
        }
        public new void Dispose()
        {
            // Clear table users after every test
            _landmarkRemarkContext.Database.ExecuteSqlCommand("DELETE FROM Notes");
            base.Dispose();
        }
    }
}
