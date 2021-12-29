using Alura.ListaLeitura.Seguranca;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.Services
{
    [ApiController]
    [Route("api/[Controller]")]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<Usuario> _signInManager;

        public LoginController(SignInManager<Usuario> signInManager)
        {

            _signInManager = signInManager;

        }

        [HttpPost]
        public async Task<IActionResult> Toke(LoginModel model)
        {
            // Validando se tem alguma coisa no meu login
            if (ModelState.IsValid )
            {

                // validando se o que foi passado está ok
                var result = await _signInManager.PasswordSignInAsync(model.Login, model.Password, true, true);
                if (result.Succeeded)
                {

                    // Criando o Token - JWT (header + payload (Claim => direitos) + signature)
                    var direitos = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, model.Login),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    // Colocando a mesma chave passada no Startup
                    var chave = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("zika-igor-authentication-valid"));

                    // Gerando as credenciais.
                    // Usando o algoritimo HmacSha256
                    var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

                    // Gerando o Token com as infos criadas.
                    var token = new JwtSecurityToken(
                    
                        // Tem que ser igual a do Startup
                        issuer: "Alura.WebApp",
                        audience: "Postman",
                        claims: direitos,
                        signingCredentials: credenciais,
                        expires: DateTime.Now.AddMinutes(30)

                        );

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                    return Ok(tokenString);

                }

                // Se não for bem sucedido, vai retornar não autorizado
                return Unauthorized(); // 401
            }

            return BadRequest(); // 400

        }

    }
}
