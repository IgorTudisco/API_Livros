using Alura.ListaLeitura.Seguranca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.HttpClients
{
    // Class que vai consumir minha Api de authorization
    public class AuthApiClient
    {
        private HttpClient _httpclient;

        public AuthApiClient(HttpClient httpClients)
        {
            _httpclient = httpClients;
        }

        // Método que vai retornar o meu token
        public async Task<string> PostLoginAsync(LoginModel model)
        {
            // Pegando a minha requisição Jason com o meu token
            // Como eu tenho duas opção de string a serialização é feita pelo sistema .Net
            var resposta = await _httpclient.PostAsJsonAsync("login", model);

            // Retornando o meu token.
            return await resposta.Content.ReadAsStringAsync();

        }

    }
}
