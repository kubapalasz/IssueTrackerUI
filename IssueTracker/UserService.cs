using System.Security.Cryptography;
using System.Text;
using IssueTracker.Interfaces;
using IssueTracker.Repositories;

namespace IssueTracker
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository = null)
        {
            _userRepository = userRepository ?? new UserRepository();
        }

        public User Create(string login, string name, string password)
        {
            var newUser = new User
            {
                Name = name,
                Login = login,
                PasswordHash = GetMD5(password)
            };

            _userRepository.PersistUser(newUser);
            return newUser;
        }

        public User GetUserByLogin(string login)
        {
            return _userRepository.GetUserByLogin(login);
        }

        private string GetMD5(string password)
        {
            var bytes = MD5.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(password));

            var hashBuilder = new StringBuilder();
            foreach (var @byte in bytes)
            {
                hashBuilder.Append(@byte.ToString("x2"));
            }

            return hashBuilder.ToString();
        }
    }
}
