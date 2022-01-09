using Alura.ListaLeitura.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
// Passando um apelido para a minha importação
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivroApiClient
    {
        // Endpoint de ref.
        //http://localhost:6000/api/Livros/{id}
        //http://localhost:6000/api/ListasLeitura/paraler
        //http://localhost:6000/api/Livros/{id}/capa

        private readonly HttpClient _httpClient;

        public LivroApiClient(HttpClient httpClient)
        {

            _httpClient = httpClient;

        }

        // Método que vai consumir a minha lista de leitura.
        public async Task<Lista> GetListaLeituraAsync(TipoListaLeitura tipo)
        {
            // GetAsync vai me trazer a lista.
            var resposta = await _httpClient.GetAsync($"listasleitura/{tipo}");
            resposta.EnsureSuccessStatusCode();

            return await resposta.Content.ReadAsAsync<Lista>();

        }

        // Método de consumo que vai deletar o meu livro.
        public async Task DeleteLivroAsunc(int id)
        {

            var resposta = await _httpClient.DeleteAsync($"livros/{id}");
            resposta.EnsureSuccessStatusCode();

        }

        // Método que vai consumir a minha API e vai me voltar a capa de um livro
        public async Task<byte[]> GetCapaLivroAsync(int id)
        {

            // Consumindo a minha API REST
                        
            /*
             * Criando a variá vel que vai conter a minha resposta.
             * Como eu preciso esperar que a minha API retorne
             * uma resposta. Ela deve ser assincrona.
             * Fazendo a interpolação com o meu endpoint, passando
             * o que falta na minha uri.
            */
            HttpResponseMessage resposta = await _httpClient.GetAsync($"Livros/{id}/capa");

            /*
             * Método que verifica que eu recebi um status dá família
             * 200. Se eu receber ele não faz nada, mas se eu não receber
             * ele vai lançar uma exceção. Assim garantindo que tenha um status 200 
             */
            resposta.EnsureSuccessStatusCode();

            // Retornando a capa convertida.
            return await resposta.Content.ReadAsByteArrayAsync();

        }

        // Método que vai consumir a minha API e vai me voltar um livro
        public async Task<LivroApi> GetLivroAsync(int id)
        {

            // Consumindo a minha API REST

            HttpResponseMessage resposta = await _httpClient.GetAsync($"Livros/{id}");

            resposta.EnsureSuccessStatusCode();

            return await resposta.Content.ReadAsAsync<LivroApi>();

        }

    }
}
