using Alura.ListaLeitura.Seguranca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.HttpClients
{
    // Criando uma classe que vai conter a idéia de login
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; }
    }

    // Class que vai consumir minha Api de authorization
    public class AuthApiClient
    {
        private HttpClient _httpclient;

        public AuthApiClient(HttpClient httpClients)
        {
            _httpclient = httpClients;
        }

        // Método que vai retornar o meu token
        public async Task<LoginResult> PostLoginAsync(LoginModel model)
        {
            // Pegando a minha requisição Jason com o meu token
            // Como eu tenho duas opção de string a serialização é feita pelo sistema .Net
            var resposta = await _httpclient.PostAsJsonAsync("login", model);

            // Passando um resposta de sucesso e retornando o meu token.
            return new LoginResult
            {
                Succeeded = resposta.IsSuccessStatusCode,
                Token = await resposta.Content.ReadAsStringAsync()
            };

        }

    }
}
