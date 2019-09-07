using LandmarkRemark.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LandmarkRemark.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        Task<User> GetAsync(int userId);
        Task<User> AddAsync(User user);
        IEnumerable<User> Find(Func<User, bool> predicate);
        Task<User> FindOneAsync(Expression<Func<User, bool>> predicate);

        Task<bool> UserExist(int userId);
    }
    public class UserRepository : IUserRepository
    {
        private readonly LandmarkRemarkContext _context;

        public UserRepository(LandmarkRemarkContext context)
        {
            _context = context;
        }
        private IIncludableQueryable<User,List<Location>> _usersWithIncludes
        {
            get
            {
                return _context.Users.Include(x => x.Locations);
            }
           
        }
        public async Task<User> AddAsync(User user)
        {
            var result = await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<User> GetAsync(int userId)
        {
            return await _usersWithIncludes
              .FirstOrDefaultAsync(user => user.Id == userId);
        }

        public IEnumerable<User> GetUsers()
        {
            return _context.Users.AsEnumerable();
        }
        public IEnumerable<User> Find(Func<User,bool> predicate)
        {
            return _usersWithIncludes.Where(predicate);
        }
        public async Task<User> FindOneAsync(Expression<Func<User, bool>> predicate)
        {
            return await _usersWithIncludes
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> UserExist(int userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }

    }
}
