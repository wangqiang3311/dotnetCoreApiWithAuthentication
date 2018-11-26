using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

//引用命名空间
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace JwtAuthSample.Controllers
{
    [Route("api/[controller]")]
    public class AuthorizeController : Controller
    {
        private JwtSettings _jwtSettings;

        public AuthorizeController(IOptions<JwtSettings> _jwtSettingsAccesser)
        {
            _jwtSettings=_jwtSettingsAccesser.Value;
        }

        [HttpPost]
        public IActionResult Token([FromBody]LoginViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                if(!(viewModel.User=="wbq"&&viewModel.Password=="1234567a"))
                {
                    return BadRequest();
                }


                var claims=new Claim[]{
                    new Claim(ClaimTypes.Name,"wbq"),
                    new Claim(ClaimTypes.Role,"admin")
                };

                //对称秘钥
                var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                //签名证书(秘钥，加密算法)
                var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
                
                var expires = DateTime.Now.AddMinutes(30);

               var token=new JwtSecurityToken(
                    issuer:_jwtSettings.Issuer,
                    audience:_jwtSettings.Audience,
                    claims:claims,
                    notBefore:DateTime.Now,
                    expires: expires,
                    signingCredentials: creds
                    );

                return Ok(new {token=new JwtSecurityTokenHandler().WriteToken(token)});

            }

            return BadRequest();
        }
    }
}