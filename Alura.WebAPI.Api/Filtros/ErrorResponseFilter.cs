using Alura.WebAPI.Api.Modelo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api.Filtros
{
    /*
     * Para a minha class filtrar os erros ela tem
     * que extender de IExpceptionFilter
    */
    public class ErrorResponseFilter : IExceptionFilter
    {
        // Método que vai fazer o filtro.
        public void OnException(ExceptionContext context)
        {
            /*
             * O erro será filtrado e vai lançar
             * a minha class de erro costumizada.
            */
            var errorResponse = ErrorResponse.From(context.Exception);

            // Contexto em que vem a minha requisição.
            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = 500
            };

        }

    }
}
