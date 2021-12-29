using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.Api.Formartters
{
    // Erdando da class abstrata que indica que é um formato do tipo text que vai sair.
    public class LivroCsvFormatter : TextOutputFormatter
    {
        public LivroCsvFormatter()
        {
            var textCsvMediaType = MediaTypeHeaderValue.Parse("text/csv");
            var appCsvMediaType = MediaTypeHeaderValue.Parse("application/csv");
            SupportedMediaTypes.Add(textCsvMediaType);
            SupportedMediaTypes.Add(appCsvMediaType);
            SupportedEncodings.Add(Encoding.UTF8);
        }

        // Sobrecrevendo para que seja feito a converção apenas para um tipo
        protected override bool CanWriteType(Type type)
        {
            return type == typeof(LivroApi);
        }

        // Método da class implementado.
        // vai ser subescrito por esse.
        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            // Criando a variavel que vai ter o meu formado de escrita.
            var livroEmCsv = "";

            // Verificando o obj
            if (context.Object is LivroApi)
            {
                // Passando o obj como um livroApi
                var livro = context.Object as LivroApi;

                // Pegando o obj e colocando no formado que eu quero.
                livroEmCsv = $"{livro.Titulo};{livro.Subtitulo};{livro.Autor};{livro.Lista}";

            }

            // Usando o escritor para passar na minha requisição.
            using (var escritor = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            {
                return escritor.WriteAsync(livroEmCsv);
            }// escritor.Close()

        }
    }
}
