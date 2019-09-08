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

        Task<bool> UserExistAsync(int userId);
        User Add(User user);
    }
    public class UserRepository : IUserRepository
    {
        private readonly LandmarkRemarkContext _context;

        public UserRepository(LandmarkRemarkContext context)
        {
            _context = context;
        }
        private IIncludableQueryable<User,List<Note>> _usersWithIncludes
        {
            get
            {
                return _context.Users.Include(x => x.Notes);
            }
        }
        // helpful for seeding in unit test class construct
        public User Add(User user)
        {
            var result =  _context.Users.Add(user);
            // to trigger validation
             _context.SaveChanges();

            return result.Entity;
        }

        public async Task<User> AddAsync(User user)
        {
            var result = await _context.Users.AddAsync(user);
            // to trigger validation
            await _context.SaveChangesAsyncWithValidation();
            
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

        // Generic find, can be useful to find userId with terms
        public IEnumerable<User> Find(Func<User,bool> predicate)
        {
            return _usersWithIncludes.Where(predicate);
        }
        public async Task<User> FindOneAsync(Expression<Func<User, bool>> predicate)
        {
            return await _usersWithIncludes
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> UserExistAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }

    }
}
