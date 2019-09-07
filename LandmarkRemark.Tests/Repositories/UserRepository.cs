using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Xunit;
using LandmarkRemark.Models;
using LandmarkRemark.Repositories;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.ComponentModel.DataAnnotations;

namespace LandmarkRemark.Tests.Repositories
{


    public class UserRepositoryTest : RepositoryTestBase, IDisposable
    {
        protected IUserRepository _userRepository;
        private User _testUser = new User() { Username = "Test user" };
        public UserRepositoryTest()
        {
            _userRepository = new UserRepository(_landmarkRemarkContext);
            _userRepository.Add(_testUser);
        }

        private async Task<User> SeedTestUser()
        {
            return await _userRepository.AddAsync(_testUser);
        }

        [Fact]
        public async Task GetUserById_ReturnCorrectUser()
        {
            // seed user

            var foundUser = await _userRepository.GetAsync(_testUser.Id);
            Assert.Equal(_testUser.Id, foundUser.Id);
            Assert.Equal(_testUser.Username, foundUser.Username);

        }
        // TODO: write test for add, add validation, exists ,


        [Fact]
        public async Task AddAsync_ThrowExceptionForEmptyUsername()
        {

            var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
                await _userRepository.AddAsync(new User()));
            Assert.Equal("Username is required", ex.Message);

        }
        // encounter this issue
        //https://stackoverflow.com/questions/57791172/field-not-found-microsoft-entityframeworkcore-metadata-internal-entitymaterial
        // need to include entityFrameworkCore.Relational in this project
        [Fact]
        public async Task AddAsync_AddUser()
        {
            var newUsername = "New user";
            var newUser = await _userRepository.AddAsync(new User()
            {
                Username = newUsername
            });
            Assert.NotNull(newUser.Id);
            Assert.Equal(newUser.Username, newUsername);
        }

        [Fact]
        public async Task AddAsync_ThrowExceptionDuplicateUsername()
        {
            // have to instantiate new object because test user contains ID 
            var ex = await Assert.ThrowsAsync<DbUpdateException>(async () =>
            await _userRepository.AddAsync(new User()
            {
                Username = _testUser.Username
            }));
            var innerExceptionMessage = ex.InnerException.Message;
            // losely check exception message
            Assert.Contains("UNIQUE", innerExceptionMessage);
            Assert.Contains("Users.Username", innerExceptionMessage);
        }

        public new void Dispose()
        {
            // Clear table users after every test
            _landmarkRemarkContext.Database.ExecuteSqlCommand("DELETE FROM Users");
            base.Dispose();
        }
    }
}
