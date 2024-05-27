using System;
using System.Collections.Generic;
using System.Linq;
using api.EntityFramework;
using api.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using api.DTOs;

namespace api.Services
{
    public class UserService
    {
        private AppDbContext _appDbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper;
        public UserService(AppDbContext appDbContext, IPasswordHasher<User> passwordHasher, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        // public async Task<IEnumerable<UserDto>> GetAllUsersService()
        // {
        //     var users = await _appDbContext.Users.Select(user => _mapper.Map<UserDto>(user)).ToListAsync();
        //     return users;
        // }
        public async Task<PaginationDto<User>> GetAllUsersService(QueryParameters queryParams)

        {
            // Start with a base query
            var query = _appDbContext.Users.AsQueryable();

            // Apply search keyword filter
            if (!string.IsNullOrEmpty(queryParams.SearchKeyword))
            {
                query = query.Where(p => p.Name.ToLower().Contains(queryParams.SearchKeyword.ToLower()) );
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortOrder == "desc"
                ? query.OrderByDescending(u => EF.Property<object>(u, queryParams.SortBy))
                : query.OrderBy(u => EF.Property<object>(u, queryParams.SortBy));
            }

            var totalUserCount = await query.CountAsync();

            var users = await query
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();
            return new PaginationDto<User>
            {
                Items = users,
                TotalCount = totalUserCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<UserDto?> GetUserById(Guid userId)
        {
            var user = await _appDbContext.Users.FindAsync(userId);
            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        public async Task<User?> CreateUserService(User newUser)
        {
            User createUser = new User
            {
                Name = newUser.Name,
                Email = newUser.Email,
                Password = _passwordHasher.HashPassword(null, newUser.Password),
                CreatedAt = DateTime.UtcNow,
                Address = newUser.Address,
                Image = newUser.Image,
                IsAdmin = newUser.IsAdmin,
            };
            _appDbContext.Users.Add(createUser);
            await _appDbContext.SaveChangesAsync();
            return createUser;
        }
        public async Task<User?> UpdateUserService(Guid userId, UpdateUserDto updateUser)
        {
            var user = await _appDbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(updateUser.Name))
            {
                updateUser.Name = user.Name;
            }
            if (string.IsNullOrEmpty(updateUser.Address))
            {
                updateUser.Address = user.Address;
            }
            if (string.IsNullOrEmpty(updateUser.Image))
            {
                updateUser.Image = user.Image;
            }
            _mapper.Map(updateUser, user);
            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync();
            return _mapper.Map<User>(user);
        }
        //DONE
        // public async Task<UserDto?> UpdateUserService(Guid userId, UpdateUserDto updateUser)
        // {

        //     var existingUser = await _appDbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        //     if (existingUser == null){
        //         return null;
        //     }

        //     _mapper.Map(updateUser, existingUser);

        //     if (updateUser.IsAdmin.HasValue)
        //         existingUser.IsAdmin = updateUser.IsAdmin.Value;
        //     if (updateUser.IsBanned.HasValue)
        //         existingUser.IsBanned = updateUser.IsAdmin.Value;

        //     _appDbContext.Users.Update(existingUser);
        //     await _appDbContext.SaveChangesAsync();
        //     return _mapper.Map<UserDto>(existingUser);
        // }

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

        //Done
        public async Task<UserDto?> LoginUserAsync(LoginDto loginDto)
        {
            var user = await _appDbContext.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
            return result == PasswordVerificationResult.Failed ? null : _mapper.Map<UserDto>(user);
            /*if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var userDto = new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                Image = user.Image,
                IsAdmin = user.IsAdmin,
                IsBanned = user.IsBanned,
            };

            return userDto;*/
        }

        public async Task<bool> Ban_UnbanUserAsync(Guid userId)
        {
            var user = await _appDbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.IsBanned = !user.IsBanned;

            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync();

            return true;
        }

    }
}