using System;
using LandmarkRemark.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LandmarkRemark.Tests.Repositories
{
    public class RepositoryTestBase : IDisposable
    {
        protected readonly LandmarkRemarkContext _landmarkRemarkContext;
        //base class shouldn't be able to access this,
        // only keep reference to close connection
        private readonly SqliteConnection connection;
        // use in memory database instead of mocking for repositories test
        // ensure new context is instantiate every test , matching the
        // DI scoped life cycle
        public RepositoryTestBase()
        {
           
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<LandmarkRemarkContext>()
                       .UseSqlite(connection)
                       .Options;
            _landmarkRemarkContext = new LandmarkRemarkContext(options);
            _landmarkRemarkContext.Database.EnsureCreated();
        }
        // cleanup after everytest
        public void Dispose()
        {
            _landmarkRemarkContext.Dispose();
            connection.Close();
        }

    }
}
