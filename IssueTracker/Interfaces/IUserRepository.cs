namespace IssueTracker.Interfaces
{
    public interface IUserRepository
    {
        void PersistUser(User newUser);
        User GetUserByLogin(string login);
    }
}