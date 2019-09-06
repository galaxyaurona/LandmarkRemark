using Microsoft.EntityFrameworkCore;
namespace LandmarkRemark.Models
{
    public class LandmarkRemarkContext : DbContext
    {
        public LandmarkRemarkContext(DbContextOptions<LandmarkRemarkContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}