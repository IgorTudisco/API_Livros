using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// Criando um apelido para aminha lista
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.Api.Controllers
{
    [Authorize]
    [Controller]
    // Suportando as duas versões.
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[Controller]")]
    public class ListasLeituraController : ControllerBase
    {
        // Criando um repositorio
        private readonly IRepository<Livro> _repo;

        // Colocando uma dependência.
        public ListasLeituraController(IRepository<Livro> repository)
        {
            _repo = repository;
        }

        private Lista CriaLista(TipoListaLeitura tipo)
        {
            return new Lista
            {
                Tipo = tipo.ParaString(),
                Livros = _repo.All
                    .Where(l => l.Lista == tipo)
                    // Convertendo livro para livroApi
                    .Select(l => l.ToApi())
                    .ToList()
            };
        }

        [HttpGet]
        public IActionResult TodasLista()
        {
            Lista paraLer = CriaLista(TipoListaLeitura.ParaLer);
            Lista lendo = CriaLista(TipoListaLeitura.Lendo);
            Lista lido = CriaLista(TipoListaLeitura.Lidos);
            var colecao = new List<Lista> { paraLer, lendo, lido };
            return Ok(colecao);

        }

        [HttpGet("{tipo}")]
        public IActionResult Reculpera(TipoListaLeitura tipo)
        {
            
            var lista = CriaLista(tipo);
            return Ok(lista);
        }

    }
}
