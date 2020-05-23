using System.Threading.Tasks;
using DatingApp.ApI.Models;

namespace dotnet_rpg.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(User user,string password);
        Task<User> Login(string username,string password);
        Task<bool> UserExist(string username);
        
    }
}