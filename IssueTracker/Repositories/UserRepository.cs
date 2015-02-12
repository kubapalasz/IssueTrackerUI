using System.Data;
using System.Globalization;
using IssueTracker.Interfaces;
using ServiceStack.Redis;

namespace IssueTracker.Repositories
{
    public class UserRepository : IUserRepository
    {
        private const string UserIndexKey = "urn:User";

        public void PersistUser(User newUser)
        {
            using (IRedisClient redisClient = new RedisClient())
            {
                if (redisClient.HashContainsEntry(UserIndexKey, newUser.Login))
                    throw new DuplicateNameException();

                var userClient = redisClient.As<User>();
                newUser.Id = userClient.GetNextSequence();
                userClient.Store(newUser);
                redisClient.SetEntryInHash(UserIndexKey, newUser.Login,
                    newUser.Id.ToString(CultureInfo.InvariantCulture));
            }
        }

        public User GetUserByLogin(string login)
        {
            using (IRedisClient redisClient = new RedisClient())
            {
                var userId = redisClient.GetValueFromHash(UserIndexKey, login);
                return redisClient.GetById<User>(userId);
            }
        }

        public void RemoveByLogin(string login)
        {
            using (IRedisClient client = new RedisClient())
            {
                var user = new UserService().GetUserByLogin(login);
                if (user != null)
                {
                    client.DeleteById<User>(user.Id);
                    client.RemoveEntryFromHash(UserIndexKey, user.Login);
                }
            }
        }
    }
}