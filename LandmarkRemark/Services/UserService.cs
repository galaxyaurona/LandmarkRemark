using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LandmarkRemark.Models;
using LandmarkRemark.Repositories;
namespace LandmarkRemark.Services
{
    public struct UserServiceError
    {
        public const string EMPTY_USERNAME_ERROR = "Username cannot be empty";
        public const string USERNAME_EXISTED_ERROR = "This username already exist";
    }
    public interface IUserService
    {
        IEnumerable<User> GetUsers();
        Task<User> GetAsync(int userId);
        Task<User> GetUserByUsername(string username);
        Task<ModifyingActionResult<User>> AddUserAsync(string username);
        Task<bool> UserExistsAsync(int userId);
    
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetAsync(int userId)
        {
            return await _userRepository.GetAsync(userId);
        }

        public IEnumerable<User> GetUsers()
        {
            return  _userRepository.GetUsers();
        }

        public Task<User> GetUserByUsername(string username)
        {
            return _userRepository
                .FindOneAsync(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<ModifyingActionResult<User>> AddUserAsync(string username)
        {

            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(username)){
                errors.Add(UserServiceError.EMPTY_USERNAME_ERROR);
            }
            else if (await GetUserByUsername(username) != default(User))
            {
                errors.Add(UserServiceError.USERNAME_EXISTED_ERROR);
            }
            var result = new ModifyingActionResult<User>()
            {
                Success = false,
                Errors = errors,
            };
            
            if (errors.Count > 0)
            {
                return result;  
            }
            var addedUser = await _userRepository.AddAsync(new User()
            {
                Username = username
            });
            result.Success = true;
            result.Data = addedUser;
            return result;
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _userRepository.UserExistAsync(userId);
        }
        
    }
}
