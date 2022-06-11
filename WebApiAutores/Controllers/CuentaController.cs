using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiAutores.DTOs;
//using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/acount")]
    public class CuentaController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;

        public CuentaController(UserManager<IdentityUser> userManager, 
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {
            var result = await signInManager.PasswordSignInAsync(credencialesUsuario.Email,
                credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<RespuestaAutenticacion>> Post (CredencialesUsuario credencialesUsuario)
        {
            var user = new IdentityUser { UserName = credencialesUsuario.Email, Email = credencialesUsuario.Email };    
            var result = await userManager.CreateAsync(user, credencialesUsuario.Password);

            if (result.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpGet("renewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacion>> Renew()
        {
            var email = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "email").Value;
            var userCredentials = new CredencialesUsuario()
            {
                Email = email,
            };
            return await ConstruirToken(userCredentials);
        }

        [HttpPost("CreatedAdmin")]
        public async Task<ActionResult> CreateAdmin(EditarAdminDTO editarAdminDTO)
        {
            var user = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.AddClaimAsync(user, new Claim("Admin", "1"));
            return NoContent();
        }

        [HttpPost("RemovedAdmin")]
        public async Task<ActionResult> RemoveAdmin(EditarAdminDTO editarAdminDTO)
        {
            var user = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.RemoveClaimAsync(user, new Claim("Admin", "1"));
            return NoContent();
        }
        private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", credencialesUsuario.Email)
            };

            var user = await userManager.FindByEmailAsync(credencialesUsuario.Email);
            var claimsDB = await userManager.GetClaimsAsync(user);
            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddMinutes(15);
            var security = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials: creds);
            return new RespuestaAutenticacion(){
                Token = new JwtSecurityTokenHandler().WriteToken(security),
                Expiracion = expiracion
            };
        }
    }
}
