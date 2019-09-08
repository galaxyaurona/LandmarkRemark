using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
namespace LandmarkRemark.Models
{
    public static class LandmarkRemarkContextExtension
    {
        // SaveChangeAsync is extension method static, need to
        public async static Task<int> SaveChangesAsyncWithValidation(this LandmarkRemarkContext instance)
        {
            var entities = instance.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity.Entity);
                Validator.ValidateObject(entity.Entity, validationContext, true);
            }
            return await instance.SaveChangesAsync();
        }
    }
    public class LandmarkRemarkContext : DbContext
    {
        public LandmarkRemarkContext(DbContextOptions<LandmarkRemarkContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Model creating logic goes here
            // user constraint
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
            

            // user relationship

        }
        // add validation here using Data annotation
        public override int SaveChanges()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity.Entity);
                Validator.ValidateObject(entity.Entity, validationContext, true);
            }
            return base.SaveChanges();
        }
        // override async method to include validation with data annotation


        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
    }
}