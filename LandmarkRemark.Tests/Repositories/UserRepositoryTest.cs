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
using System.Linq;
namespace LandmarkRemark.Tests.Repositories
{
    // need this for repository test to run sequentially
    [Collection("RepositoryTest")]
    public class UserRepositoryTest : RepositoryTestBase, IDisposable
    {
        protected IUserRepository _userRepository;
        private User _testUser = new User() { Username = "Test user" };
        private string _newUsername = "New user";
        private int _addedTestUserId = -1;
        public UserRepositoryTest()
        {
            _userRepository = new UserRepository(_landmarkRemarkContext);
            var addedUser = _userRepository.Add(_testUser);
            _addedTestUserId = addedUser.Id;
        }

        [Fact]
        public void GetUsers_ReturnCorrectList()
        {
            var users = _userRepository.GetUsers();
            Assert.Single(users);
            Assert.Contains(_testUser, users);
            Assert.DoesNotContain(new User()
            {
                Username = _newUsername
            }, users);
        }

        [Fact]
        public async Task GetUserById_ReturnCorrectUser()
        {

            var foundUser = await _userRepository.GetAsync(_addedTestUserId);
            Assert.Equal(_addedTestUserId, foundUser.Id);
            Assert.Equal(_testUser.Username, foundUser.Username);

        }

        [Fact]
        public async Task GetUserById_ReturnDefaultForNotExistingId()
        {
            var notAddedUsers = await _userRepository.GetAsync(-1);
            Assert.Equal(default(User), notAddedUsers);
        }


        [Fact]
        public void Add_AddValidUser()
        {
            var newUser = _userRepository.Add(new User()
            {
                Username = _newUsername
            });
            //positive id mean save succesfully created user
            Assert.InRange(newUser.Id, 1, 999999);
            Assert.Equal(newUser.Username, _newUsername);
        }
        [Fact]
        public void Add_ThrowExceptionForEmptyUsername()
        {
            var ex = Assert.Throws<ValidationException>(() =>
                _userRepository.Add(new User()));
            Assert.Equal(UserDataAnnotationError.USERNAME_IS_REQUIRED, ex.Message);
        }

        [Fact]
        public void Add_ThrowExceptionDuplicateUsername()
        {
            // have to instantiate new object because test user contains ID 
            var ex = Assert.Throws<DbUpdateException>(() =>
              _userRepository.Add(new User()
              {
                  Username = _testUser.Username
              }));
            var innerExceptionMessage = ex.InnerException.Message;
            // losely check exception message
            Assert.Contains("UNIQUE", innerExceptionMessage);
            Assert.Contains("Users.Username", innerExceptionMessage);
        }

        [Fact]
        public async Task AddAsync_ThrowExceptionForEmptyUsername()
        {
            var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
                await _userRepository.AddAsync(new User()));
            Assert.Equal(UserDataAnnotationError.USERNAME_IS_REQUIRED, ex.Message);
        }
        // encounter this issue
        //https://stackoverflow.com/questions/57791172/field-not-found-microsoft-entityframeworkcore-metadata-internal-entitymaterial
        // need to include entityFrameworkCore.Relational in this project
        [Fact]
        public async Task AddAsync_AddUser()
        {
            var newUser = await _userRepository.AddAsync(new User()
            {
                Username = _newUsername
            });
            //positive id mean save succesfully
            Assert.InRange(newUser.Id, 1, 999999);
            Assert.Equal(newUser.Username, _newUsername);
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

        [Fact]
        public async Task UserExistAsync_FoundSeedUser()
        {
            var exist = await _userRepository.UserExistAsync(_addedTestUserId);
            Assert.True(exist);
        }

        [Fact]
        public async Task UserExistAsync_NotFoundInvalidId()
        {
            var exist = await _userRepository.UserExistAsync(-1);
            Assert.False(exist);
        }
        // helper method for test Find with predicate
        private bool FindUsernameContainsWordTestIgnorecase(User user)
        {
            return user.Username.Contains("tEst", StringComparison.OrdinalIgnoreCase);
        }

        private bool FindUsernameContainWordRandom(User user)
        {
            return user.Username.Contains("random");
        }

        [Fact]
        public void Find_FoundUserWhomUsernameContainWordTest()
        {
            var foundUsers = _userRepository.Find(FindUsernameContainsWordTestIgnorecase);
            var foundTestUser = foundUsers.FirstOrDefault();
            Assert.Single(foundUsers);
            Assert.Equal(_addedTestUserId, foundTestUser.Id);
            Assert.Equal(_testUser.Username, foundTestUser.Username);

        }

        [Fact]
        public void Find_DoesNotFindUsernameWithWordRandom()
        {
            var foundUsers = _userRepository.Find(FindUsernameContainWordRandom);
            Assert.Empty(foundUsers);
        }

        [Fact]
        public async Task FindOneAsync_FoundUserWhomUsernameContainWordTest()
        {
            var foundUser = await _userRepository.FindOneAsync(user => FindUsernameContainsWordTestIgnorecase(user));
            Assert.Equal(_addedTestUserId, foundUser.Id);
            Assert.Equal(_testUser.Username, foundUser.Username);
        }

        [Fact]
        public async Task FindOneAsync_DoesNotFindUsernameWithWordRandom()
        {
            var foundUser = await _userRepository.FindOneAsync(user => FindUsernameContainWordRandom(user));
            Assert.Equal(default(User), foundUser);
        }


        public new void Dispose()
        {
            // Clear table users after every test
            _landmarkRemarkContext.Database.ExecuteSqlCommand("DELETE FROM Users");
            base.Dispose();
        }
    }
}
