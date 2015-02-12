using System;
using System.Data;
using FluentAssertions;
using IssueTracker.Repositories;
using NUnit.Framework;
using ServiceStack.Redis;

namespace IssueTracker.Integration.Tests
{
    [TestFixture]
    public class UserDataAccessTests
    {
        [Test]
        public void CreateUser_ShouldBeStoredInRedisDatabase()
        {
            new UserRepository().RemoveByLogin("asdfasdf");
            var redisUserId = new UserService().Create("asdfasdf", "asdfasd", "password").Id;


            User redisUser = null;
            using (IRedisClient client = new RedisClient())
            {
                redisUser = client.GetById<User>(redisUserId);
            }

            redisUser.Login.Should().Be("asdfasdf");
            redisUser.Name.Should().Be("asdfasd");
            redisUser.PasswordHash.Should().Be("5f4dcc3b5aa765d61d8327deb882cf99");
        }

        [Test]
        public void GetUserByUserName_ReturnsUser()
        {
            new UserRepository().RemoveByLogin("asdfasdfX1");
            var userId = new UserService().Create("asdfasdfX1", "asdfasd123", "password123").Id;

            var redisUser = new UserService().GetUserByLogin("asdfasdfX1");
            new UserRepository().RemoveByLogin("asdfasdfX1");

            redisUser.Should().NotBeNull();
        }

        [Test]
        public void CreateMultipleUsers_ShouldReturnMultipleUsersByUniqueIds()
        {
            new UserRepository().RemoveByLogin("asdfasdf1");
            new UserRepository().RemoveByLogin("asdfasdf2");

            var firstRedisUserId = new UserService().Create("asdfasdf1", "asdfasd1", "password1").Id;
            var secondRedisUserId = new UserService().Create("asdfasdf2", "asdfasd2", "password2").Id;

            //act
            User firstRedisUser = null;
            User secondRedisUser = null;
            using (IRedisClient client = new RedisClient())
            {
                firstRedisUser = client.GetById<User>(firstRedisUserId);
                secondRedisUser = client.GetById<User>(secondRedisUserId);
            }
            //goto redis db, read user entry by id

            //assert
            firstRedisUser.Login.Should().Be("asdfasdf1");
            secondRedisUser.Login.Should().Be("asdfasdf2");
        }

        [Test]
        public void CreateTwoUsersWithSameLoginName_ShouldFail()
        {
            new UserRepository().RemoveByLogin("asdfasdf1");

            //arrange
            new UserService().Create("asdfasdf1", "asdfasd1", "password1");
            Action act = () =>  new UserService().Create("asdfasdf1", "asdfasd2", "password2");

            //assert
            act.ShouldThrow<DuplicateNameException>();
        }

        [Test]
        public void DeletedUser_CannotBeFoundByLogin()
        {
            new UserRepository().RemoveByLogin("blabla");
            new UserService().Create("blabla", "asdfasd1", "password1");
            new UserRepository().RemoveByLogin("blabla");

            new UserService().GetUserByLogin("blabla").Should().BeNull();
        }

    }
}
