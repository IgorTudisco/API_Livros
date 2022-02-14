using Alura.ListaLeitura.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace Alura.WebAPI.Api.Modelo
{

    public static class LivroOrdemExtention
    {
        // 
        public static IQueryable<Livro> AplicaOrdem(this IQueryable<Livro> query, LivroOrdem ordem)
        {

            if (ordem != null)
            {

                // Ondenando pelo orderby
                // Usando o pacote System.Linq.Dynamic.Core, podemos passar parâmetros no orderby.
                query = query.OrderBy(ordem.OrdenarPor);

            }
            return query;

        }
    }

    // Dados a ser ordernado
    public class LivroOrdem
    {

        public string OrdenarPor { get; set; }

    }
}
