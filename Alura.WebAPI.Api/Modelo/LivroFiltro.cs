using Alura.ListaLeitura.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api.Modelo
{

    public static class LivrosFiltroExtention
    {
        // Método que vai retornar o dado filtrado.
        public static IQueryable<Livro> AplicaFiltro(this IQueryable<Livro> query, LivroFiltro filtro)
        {
            // Se não for nulo ele deve fazer a pesquisa.
            if (filtro != null)
            {

                // Para Titulo
                if (!string.IsNullOrEmpty(filtro.Titulo))
                {
                    // Onde o livro conter titulo.
                    query = query.Where(l => l.Titulo.Contains(filtro.Titulo));
                }

                // Para Autor
                if (!string.IsNullOrEmpty(filtro.Autor))
                {
                    query = query.Where(l => l.Autor.Contains(filtro.Autor));
                }

                // Para Subtitulo
                if (!string.IsNullOrEmpty(filtro.Subtitulo))
                {
                    query = query.Where(l => l.Subtitulo.Contains(filtro.Subtitulo));
                }

                // Para Lista
                if (!string.IsNullOrEmpty(filtro.Lista))
                {
                    query = query.Where(l => l.Lista == filtro.Lista.ParaTipo());
                }

            }

            return query;
        }
    }

    // Dados dá pesquisa.

    public class LivroFiltro
    {
        public String Titulo { get; set; }
        public String Subtitulo { get; set; }
        public String Autor { get; set; }
        public String Lista { get; set; }

    }
}
