using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAPI.Data;
using UserAPI.Data.Interfaces;
using UserAPI.Model.Dto;
using UserAPI.Model.Entity;

namespace UserAPI.Business.Services
{
    public class UserService
    {
        private readonly IRepository<User> _repository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IRepository<User> repository,
                          IPasswordHasher<User> passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginDto> AuthenticateAsync(string email, string password)
        {
            var user = null; //Repository metotları güncellenecek.

            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(
                user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            user.LastLogin = DateTime.UtcNow;

            return null; //Dönüştürme işlemi yapılmalı..
        }
    }
}
