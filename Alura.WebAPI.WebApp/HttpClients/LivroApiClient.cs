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
        public async Task DeleteLivroAsync(int id)
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

        // Método para ajudar na hora de passar as partes para o meu conteudo
        private string EnvolveComAspasDuplas(string valor)
        {

            return $"\"{valor}\"";

        }

        // Método que vai servir de apoio para a criação do partformData.
        private HttpContent CreateMultipartFormDataContent(LivroUpload model)
        {
            // Criando o meu conteudo
            var content = new MultipartFormDataContent();

            // Add as partes do nosso conteudo, essas partes são cada campo de formulário.

            // Obrigatório
            content.Add(new StringContent(model.Titulo), EnvolveComAspasDuplas("titulo"));
            content.Add(new StringContent(model.Lista.ParaString()), EnvolveComAspasDuplas("lista"));

            // Não obrigatório
            if (!string.IsNullOrEmpty(model.Subtitulo))
            {
                content.Add(new StringContent(model.Subtitulo), EnvolveComAspasDuplas("subtitulo"));
            }

            if (!string.IsNullOrEmpty(model.Resumo))
            {
                content.Add(new StringContent(model.Resumo), EnvolveComAspasDuplas("resumo"));
            }

            if (!string.IsNullOrEmpty(model.Autor))
            {
                content.Add(new StringContent(model.Autor), EnvolveComAspasDuplas("autor"));
            }

            // Fazendo a verificação do meu obj, se ele existir. Esse obj vai ser alterado e não criado.
            if (model.Id > 0)
            {

                content.Add(new StringContent(model.Id.ToString()), EnvolveComAspasDuplas("id"));

            }

            // Verificando o arquivo de upload
            if (model.Capa != null)
            {
                // Variável que vai conter a minha converção da minha img para bytes.
                var imagemContent = new ByteArrayContent(model.Capa.ConvertToBytes());

                // Informando no headers qual o tipo de conteudo que eu estou esperando.
                // O ByteArray converte quanquer arquivo.
                imagemContent.Headers.Add("contet-type", "imagem/png");

                // Add a img no meu content
                // Quando se passa um arquivo, devemos passar um nome para ele. Mesmo sendo um genérico
                content.Add(imagemContent,
                    EnvolveComAspasDuplas("capa"),
                    EnvolveComAspasDuplas("capa.png")
                );
            }

            return content;

        }

        // Método que vai passar algo pra a minha api.
        public async Task PostLivroAsync(LivroUpload model)
        {
            /*
             * O tipo HttpContent é do tipo abstrato, assim não podemos
             * criar obj dele, mas podemos criar filhos.
             * O conteudo tem que ser do tipo HttpContent. 
             */
            HttpContent content = CreateMultipartFormDataContent(model);

            // Nesse caso além do endpoint temos que enviar um conteudo.
            var resposta = await _httpClient.PostAsync("livros", content);
            resposta.EnsureSuccessStatusCode();

        }

        // Método que vai fazer a alteração na minha api
        public async Task PutLivroAsync(LivroUpload model)
        {

            HttpContent content = CreateMultipartFormDataContent(model);
            var resposta = await _httpClient.PutAsync("livros", content);
            resposta.EnsureSuccessStatusCode();

        }
                
    }
}
