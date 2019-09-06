using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Xunit;
using LandmarkRemark.Models;
using LandmarkRemark.Repositories;
namespace LandmarkRemark.Tests.Repositories
{
   
    public class UserRepository 
    {
        [Fact]
        public User TestGetUser()
        {
            return new User();
        }
    }
}
