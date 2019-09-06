using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LandmarkRemark.Models;
using LandmarkRemark.Repositories;
namespace LandmarkRemark.Services
{
    public interface IUserService
    {
        IEnumerable<User> GetUsers();
        Task<User> GetAsync(int userId);
        Task<User> GetUserByUsername(string username);
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
        
    }
}
