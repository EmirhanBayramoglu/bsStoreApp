using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NLog.LayoutRenderers.Wrappers;
using Services.Contract;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    internal class AuthenticationManager : IAuthenticationService
    {
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        private User? _user;

        public AuthenticationManager(ILoggerService logger, 
            IMapper mapper, 
            UserManager<User> userManager,
            IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<TokenDto> CreateToken(bool populateExp)
        {
            var signinCredentials = GetSigninCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signinCredentials,claims);

            var refreshToken = GenerateRefreshToken();
            _user.RefreshToken = refreshToken;

            if (populateExp)
                _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(_user);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistrationDto)
        {
            var user = _mapper.Map<User>(userForRegistrationDto);

            var result = await _userManager.CreateAsync(user, userForRegistrationDto.Password);

            if(result.Succeeded)
                await _userManager.AddToRolesAsync(user, userForRegistrationDto.Roles);
            return result;  
        }

        public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuthDto)
        {
            _user = await _userManager.FindByNameAsync(userForAuthDto.UserName);
            var result = (_user != null && await _userManager.CheckPasswordAsync(_user, userForAuthDto.Password));
            if(!result)
            {
                _logger.LogWarning($"{nameof(ValidateUser)} : Authentication failed. Wrong username or password.");
            }
            return result;
        }

        private SigningCredentials GetSigninCredentials() //kimlik bilgilerini doğrulama
        {
            var jwtSettings = _configuration.GetSection("JwtSettings"); //setting'lerin bulunduğu yeri ayarla
            var key = Encoding.UTF8.GetBytes(jwtSettings["secretKey"]); //settinglerin bulunduğu yerden secretKey'i al
            var secret = new SymmetricSecurityKey(key); //secretKey'i çözümle

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256); //anahtarı ve algoritmayı döndür
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, _user.UserName)
            };

            var roles = await _userManager
                .GetRolesAsync(_user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signinCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings["validIssuer"],
                audience: jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
                signingCredentials: signinCredentials);
            return tokenOptions;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["secretKey"];

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, 
                out securityToken); //out parametre düzenleyicidir ne olması gerektiğini bilecek

            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token.");
            }
                

            return principal;
        }

        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);//gelen tokenı doğrula

            var user = await _userManager.FindByNameAsync(principal.Identity.Name);//token ile gelen kullanıcı halen var mı

            if (user == null || 
                user.RefreshToken != tokenDto.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.Now) 
            {
                throw new RefreshTokenBadRequestException();
            }

            _user = user;
            return await CreateToken(populateExp: false);
        }
    }
}
