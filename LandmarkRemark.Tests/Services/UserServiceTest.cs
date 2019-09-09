using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LandmarkRemark.Models;
using LandmarkRemark.Repositories;
using LandmarkRemark.Services;
using Moq;
using Xunit;

namespace LandmarkRemark.Tests.Services
{
    // Mock model class, override id  for mocking id for unit test
    public class MockUser : User, IEquatable<MockUser>
    {
        new public int Id
        {
            get;
            set;
        }
        // losely check for equality of 2 user
        public bool Equals(MockUser user)
        {
            return (Id == user.Id) && (Username == user.Username);
        }
    }
    public class UserServiceTest : IDisposable
    {
        private IUserService _userService;
        private Mock<IUserRepository> _mockUserRepository;
        private const string _usernameTemplate = "Test user ";
        private static string _testUser1Username = _usernameTemplate + "1";
        private static MockUser _testUser1 = new MockUser()
        {
            Id = 1,
            Username = _testUser1Username,
            Notes = new List<Note>()
        };
        private List<MockUser> _seedUsers = new List<MockUser>()
            {
                _testUser1,
                new MockUser()
                {
                    Id = 2,
                    Username = "Test user 2",
                    Notes = new List<Note>()
                }
            };



        // setup mocking service
        public UserServiceTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUserRepository.Setup(mr => mr.GetUsers())
                .Returns(_seedUsers);
            _mockUserRepository.Setup(mr => mr.GetAsync(It.IsAny<int>()))
                .Returns(async (int id) =>
                    await Task.FromResult(_seedUsers.FirstOrDefault(x => x.Id == id)));

            _mockUserRepository.Setup(mr => mr.AddAsync(It.IsAny<User>()))
                .Returns(async (User u) => await Task.FromResult(u));

            _mockUserRepository.Setup(mr => mr.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(async (Expression<Func<User, bool>> e) =>
                   await Task.FromResult(_seedUsers.FirstOrDefault(e.Compile())));

            _mockUserRepository.Setup(mr => mr.UserExistAsync(It.IsAny<int>()))
                .Returns(async (int id) => await Task.FromResult(_seedUsers.Any(u => u.Id == id)));

            _userService = new UserService(_mockUserRepository.Object);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetAsync_ReturnTestUsers(int userId)
        {
            // downcast to get the mock class id id
            var testUser = (MockUser)await _userService.GetAsync(userId);
            _mockUserRepository.Verify(mr => mr.GetAsync(It.IsAny<int>()), Times.Once());
            Assert.Equal(_usernameTemplate + userId, testUser.Username);


            Assert.Equal(userId, testUser.Id);
        }
        [Fact]
        public async void GetAsync_ReturnNullForNonExistUser()
        {
            var nonExistUser = await _userService.GetAsync(10);
            _mockUserRepository.Verify(mr => mr.GetAsync(It.IsAny<int>()), Times.Once());
            Assert.Equal(default(User), nonExistUser);
        }

        [Fact]
        public void GetUsers_ReturnOnlySeedUsers()
        {
            var users = _userService.GetUsers();
            _mockUserRepository.Verify(mr => mr.GetUsers(), Times.Once());
            Assert.Equal(2, users.Count());
            Assert.Contains(_testUser1, users);
            Assert.Contains(new MockUser()
            {
                Id = 2,
                Username = _usernameTemplate + "2",
            }, users);
            Assert.DoesNotContain(new MockUser()
            {
                Id = 4,
                Username = "Random user"
            }, users);
        }

        [Fact]
        public async void GetUserByUsername_ReturnNullForNonExistUser()
        {
            var nonExistUser = await _userService.GetUserByUsername("Random");
            _mockUserRepository.Verify(mr => mr.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>())
            , Times.Once());
            Assert.Equal(default(User), nonExistUser);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetUserByUsername_ReturnUsersForValidUsername(int userId)
        {
            var usernameQuery = _usernameTemplate + userId;
            var validUser = (MockUser)await _userService.GetUserByUsername(usernameQuery);
            _mockUserRepository.Verify(mr => 
                mr.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once());
            Assert.Equal(usernameQuery, validUser.Username);
            Assert.Equal(userId, validUser.Id);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void UserExists_FoundTestUsers(int userId)
        {
            var exist = await _userService.UserExistsAsync(userId);
            _mockUserRepository.Verify(mr => mr.UserExistAsync(It.IsAny<int>()), Times.Once());
            Assert.True(exist);
        }

        [Fact]
        public async void UserExists_NotFoundNonExistingUser()
        {
            var exist = await _userService.UserExistsAsync(1);
            _mockUserRepository.Verify(mr => mr.UserExistAsync(It.IsAny<int>()), Times.Once());
            Assert.True(exist);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void AddUserAsync_DoesNotAddUserWithEmptyUsername(string username)
        {
            var actionResult = await _userService.AddUserAsync(username);

            Assert.False(actionResult.Success);
            // errors contain username empty error
            Assert.Contains(UserServiceError.EMPTY_USERNAME_ERROR, actionResult.Errors);
            // does not invoke repository add
            _mockUserRepository.Verify(mr => mr.AddAsync(It.IsAny<User>()), Times.Never());
        }

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        public async void AddUserAsync_DoesNotAddUserExistedUsername(string usernameSuffix)
        {
            var testUsername = _usernameTemplate + usernameSuffix;
            var actionResult = await _userService.AddUserAsync(testUsername);
            Assert.False(actionResult.Success);
            // contain the error username exist message
            Assert.Contains(UserServiceError.USERNAME_EXISTED_ERROR, actionResult.Errors);
            // call find one to check username exist
            _mockUserRepository.Verify(mr => mr.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once());
            // but does not invoke repository add
            _mockUserRepository.Verify(mr => mr.AddAsync(It.IsAny<User>()), Times.Never());
        }

        [Fact]
        public async void AddUserAsync_AddUserWithValidUsername()
        {
            var addingUsername = "Adding user";
            var actionResult = await _userService.AddUserAsync(addingUsername);
            Assert.True(actionResult.Success);
            Assert.Empty(actionResult.Errors);
            var addedUser = actionResult.Data;
            Assert.Equal(addingUsername, addedUser.Username);
            _mockUserRepository.Verify(mr => mr.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once());
            // run add async
            _mockUserRepository.Verify(mr => mr.AddAsync(It.IsAny<User>()), Times.Once());
        }

        public void Dispose()
        {
            _mockUserRepository.Verify();
            _mockUserRepository.VerifyNoOtherCalls();
        }
    }
}
