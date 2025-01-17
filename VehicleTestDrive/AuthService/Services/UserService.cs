using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Data;
using AuthService.Model;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services
{
    public class UserService : IUserService
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;
        public UserService(IConfiguration configuration)
        {
            this._context = new AuthDbContext();
            this._configuration = configuration;
        }

        public void AddUser(string username, string password, string role)
        {
            User userTocreate = new User()
            {
                Username = username,
                Password = HashPassword(password),
                Role = role
            };
            _context.Users.Add(userTocreate);
            _context.SaveChanges();
        }

        public string Authenticate(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user is null || (user.Password != HashPassword(password)))
            {
                return null;
            }

            var authToken = GenerateAccessToken(user);

            return authToken;
        }

        private string GenerateAccessToken(User user)
        {
            var claims = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, user.Username)
            });

            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration["jwt:key"];
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["jwt:issuer"],
                Audience = _configuration["jwt:audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string retVal = tokenHandler.WriteToken(token);
            return retVal;
        }

        private string HashPassword(string password)
        {
            SHA512 myHash = SHA512.Create();
            byte[] hash = myHash.ComputeHash(Encoding.UTF8.GetBytes(password));
            string retVal = Convert.ToBase64String(hash);
            return retVal;
        }
    }
}
