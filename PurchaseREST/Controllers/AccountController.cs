using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PurchaseREST.Infrastructure;

namespace PurchaseREST.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        PurchasesContext pc;

        public AccountController(PurchasesContext _pc)
        {
            pc = _pc;
        }

        [HttpPost]
        [Route("registry")]
        public JsonResult Registry()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            var passwordMd5 = GetMD5HashData(password);
            var person = new Users { Login = username, Password = passwordMd5};

            var existLogin = pc.Users.FirstOrDefault(q => q.Login == username);
            if (existLogin != null)
            {
                return Json(new { message = "Such user already exists" });
            }

            pc.Users.Add(person);
            pc.SaveChanges();

            return Json(new { message = "User created"});
        }

        [HttpPost]
        [Route("token")]
        public JsonResult Token()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            var identity = GetIdentity(username, password);
            if (identity == null)
            {
                Response.StatusCode = 400;
                //  await Response.WriteAsync("Invalid username or password.");
                // return Json(new { dd = 'dsd' });
                return Json(new { message= "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name,
                message = "ok"
            };
            return Json(response);
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            var passwordMd5 = GetMD5HashData(password);

            Users user = pc.Users.FirstOrDefault(x => x.Login == username && x.Password == passwordMd5);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "somerole"),
                    new Claim("userId", user.UserId.ToString())
                    

                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token");
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }

        private string GetMD5HashData(string data)
        {
            MD5 md5 = MD5.Create();

            byte[] hashData = md5.ComputeHash(Encoding.Default.GetBytes(data));

            StringBuilder returnValue = new StringBuilder();

            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }
            return returnValue.ToString();
        }
    }
}