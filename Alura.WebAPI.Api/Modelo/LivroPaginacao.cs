using System;
using System.Collections.Generic;
using Alura.ListaLeitura.Modelos;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api.Modelo
{    
    public static class LivroPaginadoExtensions{

        public static LivroPaginado ToLivroPaginado(this IQueryable<LivroApi> query, LivroPaginacao paginacao)
        {

            int totalItens = query.Count();
            int totalPaginas = (int)Math.Ceiling(totalItens / (double)paginacao.Tamanho);

            return new LivroPaginado
            {

                Total = totalItens,
                TotalPaginas = totalPaginas,
                NumeroPaginas = paginacao.Pagina,
                TamanhoPaginas = paginacao.Tamanho,
                // Quanditade de itens 
                Resultado = query
                    /* Para descartar os itens das páginas que já passaram,
                     * assim se eu pegar a 3º pg ele descarta os itens dá pagina 1 e 2.
                    */
                    .Skip(paginacao.Tamanho * (paginacao.Pagina - 1))
                    .Take(paginacao.Tamanho).ToList(),
                // Lógica pra pegar a próxima pg. e a anterior
                Anterior = (paginacao.Pagina > 1) ?
                    $"livros?tamanho={paginacao.Pagina-1}&pagina={paginacao.Tamanho}" : "",
                Proximo = (paginacao.Pagina < totalPaginas) ?
                    $"livros?tamanho={paginacao.Pagina+1}&pagina={paginacao.Tamanho}" : "",

            };

        }
        
    }

    public class LivroPaginado
    {
        public int Total { get; set; }
        public int TotalPaginas { get; set; }
        public int TamanhoPaginas { get; set; }
        public int NumeroPaginas { get; set; }
        public IList<LivroApi> Resultado { get; set; }
        public String Anterior { get; set; }
        public String Proximo { get; set; }

    }

    public class LivroPaginacao
    {

        public int Pagina { get; set; } = 1;
        public int Tamanho { get; set; } = 25;

    }
}
