using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Alura.ListaLeitura.Api.Controllers
{
    [ApiController]
    [Authorize]
    // Anotação que vai dizer qual é a minha verção.
    // Por default ele vai ler o meu versionamento por query string
    /* 
     * É só colocar a query (?api-version=2) na frente da
     * rota que ele vai entender qual v. estamos apontando
    */
    [ApiVersion("2.0")]
    [Route("api/livros")]
    public class Livros2Controller : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public Livros2Controller(IRepository<Livro> repository)
        {
            _repo = repository;
        }

        // Método que vai me trazer uma lista de obj
        public IActionResult ListaDeLivros()
        {
            /*
             * Para trazer os meus obj do meu db
             * eu passo um select do linq que é parecido com
             * os comandos do DB.
             */
            var lista = _repo.All.Select(l => l.ToApi()).ToList();
            return Ok(lista);
        }

        // Informando que o meu id vai vir pela rota.
        [HttpGet("{id}")]
        public IActionResult Recuperar(int id)
        {

            var model = _repo.Find(id);
            if (model == null)
            {
                return BadRequest();
            }

            /* Como não tenho mais o view e nem o json,
             * no ControllerBase. Eu uso o proprio status como retorno.
            */
            return Ok(model);

        }

        // Criando uma rota para a minha capa.
        // Esse método trata a minha capa.
        [HttpGet("{id}/capa")]
        public IActionResult ImagemCapa(int id)
        {
            byte[] img = _repo.All
                              .Where(l => l.Id == id)
                              .Select(l => l.ImagemCapa)
                              .FirstOrDefault();
            if (img != null)
            {

                return File(img, "imagem/png");

            }

            return File("~/imagens/capas/capa-vazia.png", "imagem/png");

        }

        // Criando um método que vai incluir um novo livro
        [HttpPost]
        public IActionResult Incluir([FromForm] LivroUpload model)
        {

            if (ModelState.IsValid)
            {

                // Vai converter um LivroUpload para um Livro.
                var livro = model.ToLivro();
                _repo.Incluir(livro);
                var uri = Url.Action("Recuperar", new { id = livro.Id });
                return Created(uri, livro);

            }

            return BadRequest();

        }

        // Método de alteração/atualização
        [HttpPut]
        public IActionResult Alterar([FromForm] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                if (model.Capa == null)
                {
                    /* Função que irá pesquisar e comparar
                     * os IDs para alterar o livro certo
                    */
                    livro.ImagemCapa = _repo.All
                        .Where(l => l.Id == livro.Id)
                        .Select(l => l.ImagemCapa)
                        .FirstOrDefault();

                }

                _repo.Alterar(livro);
                return Ok(); // 200

            }

            return BadRequest(); // 400

        }

        // Método de deleção
        // Informando que o meu id virá na rota.
        [HttpDelete("{id}")]
        public IActionResult Remover(int id)
        {

            var model = _repo.Find(id);
            if (model == null)
            {

                return NotFound();

            }

            _repo.Excluir(model);
            return NoContent(); // 203

        }

    }
}
