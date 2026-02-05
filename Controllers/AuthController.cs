using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyEcommerce.DTOs;
using MyEcommerce.Models;

namespace MyEcommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager, IConfiguration configuration) : ControllerBase
{
   
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        
        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            FullName = model.FullName
        };
        
        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return Ok("User registered successfully!");
        }
        return BadRequest(result.Errors);
    }
    
    
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
      
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
        {
           
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            
            var userRoles = await userManager.GetRolesAsync(user);

            if (userRoles != null && userRoles.Any())
            {
                foreach (var role in userRoles)
                {
                    
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                    authClaims.Add(new Claim("role", role)); 
                }
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                expires: DateTime.Now.AddDays(double.Parse(configuration["JWT:DurationInDays"])),
                claims: authClaims, 
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        return Unauthorized("Invalid Email or Password");
    }
    
    [HttpPost("add-role")]
    public async Task<IActionResult> AddRole(string roleName)
    {
        var cleanRoleName = roleName.Trim(); 
    
        if (!await roleManager.RoleExistsAsync(cleanRoleName))
        {
            await roleManager.CreateAsync(new IdentityRole(cleanRoleName));
            return Ok("Role Created Successfully");
        }
        return BadRequest("Role already exists");
    }
    
    
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole(string email, string roleName)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user != null)
        {
            await userManager.AddToRoleAsync(user, roleName);
            return Ok("User assigned to role");
        }
        return BadRequest("User not found");
    }
}