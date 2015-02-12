using FluentAssertions;
using IssueTracker.Interfaces;
using IssueTracker.Repositories;
using NUnit.Framework;
using Rhino.Mocks;

namespace IssueTracker.Unit.Tests
{
    [TestFixture]
    public class UserCreateTests
    {
        [Test]
        public void CreateUser()
        {
            var userRepo = MockRepository.GenerateStub<IUserRepository>();
            var newUser = new UserService(userRepo).Create("myLogin", "My Name", "password");

            newUser.Login.Should().Be("myLogin");
            newUser.Name.Should().Be("My Name");
            newUser.PasswordHash.Should().Be("5f4dcc3b5aa765d61d8327deb882cf99");
        }

        [Test]
        public void CreateUsersWithDifferentPasswords_ShouldHaveDifferentHashes()
        {
            var userRepo = MockRepository.GenerateStub<IUserRepository>();
            var userService = new UserService(userRepo);

            var user1 = userService.Create("username1", "My Name", "password1");
            var user2 = userService.Create("username2", "My Name", "password2");

            user1.PasswordHash.Should().NotBe(user2.PasswordHash);
        }
    }
}
