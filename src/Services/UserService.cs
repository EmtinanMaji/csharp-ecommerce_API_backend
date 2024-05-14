using System;
using System.Collections.Generic;
using System.Linq;
using api.EntityFramework;
using api.Helpers;
using api.Model;
using api.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace api.Services
{
    public class UserService
    {
        private AppDbContext _appDbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        public UserService(AppDbContext appDbContext, IPasswordHasher<User> passwordHasher)
        {
            _appDbContext = appDbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<User>> GetAllUsersService()
        {
            return await _appDbContext.Users.ToListAsync();
        }

        public async Task<User?> GetUserById(Guid userId)
        {
            return await _appDbContext.Users.FirstOrDefaultAsync(user => user.UserId == userId);
        }

        public async Task<User> CreateUserService(User newUser)
        {
            newUser.UserId = Guid.NewGuid();
            newUser.CreatedAt = DateTime.UtcNow;
            _appDbContext.Users.Add(newUser);
            await _appDbContext.SaveChangesAsync();
            return newUser;
        }

        public async Task<User?> UpdateUserService(Guid userId, User updateUser)
        {
            var existingUser = _appDbContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (existingUser != null)
            {
                existingUser.Name = updateUser.Name;
                existingUser.Email = updateUser.Email;
                existingUser.Password = updateUser.Password;
                existingUser.Address = updateUser.Address;
                existingUser.Image = updateUser.Image;
                existingUser.IsAdmin = updateUser.IsAdmin;
                existingUser.IsBanned = updateUser.IsBanned;
            }
            await _appDbContext.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> DeleteUserService(Guid userId)
        {
            var userToRemove = _appDbContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (userToRemove != null)
            {
                _appDbContext.Users.Remove(userToRemove);
                await _appDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
        
        public async Task<UserModel?> LoginUserAsync(LoginModel loginModel)
        {


            var user = await _appDbContext.Users.SingleOrDefaultAsync(u => u.Email == loginModel.Email);
            if (user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginModel.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var userModel = new UserModel
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                Address = user.Address,
                Image = user.Image,
                IsAdmin = user.IsAdmin,
                IsBanned = user.IsBanned,
            };

            return userModel;

        }

    }
}