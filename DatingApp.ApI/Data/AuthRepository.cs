using System.Threading.Tasks;

using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.EntityFrameworkCore;
using DatingApp.ApI.Data;
using DatingApp.ApI.Models;

namespace dotnet_rpg.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext context;
        public AuthRepository(DataContext context)
        {
            this.context = context;
        }


        public async Task<User> Login(string username, string password)
        {
            User user = await context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null)
            {
               return null;
            }
            else if (VerifyPassword(password, user.PasswordHash, user.PasswordSalt) == false)
            {
               return null;
            }
           
            return user;
        }
        public async Task<User> Register(User user, string password)
        {
            
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

           return user;
        }

        public async Task<bool> UserExist(string username)
        {
            if (await context.Users.AnyAsync(x => x.Username.ToLower() == username))
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var ComputePasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (var i = 0; i < ComputePasswordHash.Length; i++)
                {
                    if (ComputePasswordHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }


    }
}