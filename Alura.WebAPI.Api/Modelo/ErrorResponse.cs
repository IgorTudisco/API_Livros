using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api.Modelo
{
    public class ErrorResponse
    {

        // Parâmetros do meu erro.
        public int Codigo { get; set; }
        public string Mensagem { get; set; }
        public ErrorResponse InnerError { get; set; }
        public String[] Detalhes { get; set; }

        // Método que constroi a minha 
        internal static ErrorResponse From(Exception e)
        {

            if (e == null)
            {

                return null;

            }

            return new ErrorResponse
            {

                Codigo = e.HResult,
                Mensagem = e.Message,
                InnerError = ErrorResponse.From(e.InnerException)

            };

        }

        // Método que vai responder ao erro 400.
        public static ErrorResponse FromModelState(ModelStateDictionary modelState)
        {

            /*
             * Convertendo um erro do modo estage para uma string.
             * 
             * Usando o SelectMany conseguimos tetirar
             * a lista que está no segundo nível
             */
            var erros = modelState.Values.SelectMany(m => m.Errors);

            // Retorno do meu erro.
            return new ErrorResponse
            {
                // Dados escolhido de forma arbitrária
                Codigo = 100,
                Mensagem = "Houve um erro no envio da requisição.",
                /*
                 * Esse erro não é um InnerError,
                 * por isso temos que tirar ele do State
                */
                Detalhes = erros.Select(e => e.ErrorMessage).ToArray()
            };
        }

    }
}
