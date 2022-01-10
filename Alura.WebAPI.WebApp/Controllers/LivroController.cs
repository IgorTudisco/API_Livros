using System.Linq;
using Alura.ListaLeitura.Persistencia;
using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Alura.ListaLeitura.HttpClients;

namespace Alura.ListaLeitura.WebApp.Controllers
{
    [Authorize]
    public class LivroController : Controller
    {
        private readonly IRepository<Livro> _repo;

        // Passando a minha dependência da class que está consumindo a minha API
        private readonly LivroApiClient _api;

        public LivroController(IRepository<Livro> repository, LivroApiClient api)
        {
            _repo = repository;
            _api = api;
        }

        [HttpGet]
        public IActionResult Novo()
        {
            return View(new LivroUpload());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Novo(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                // Passando o método que está consumindo a minha api e vai incluir os dados
                await _api.PostLivroAsync(model);

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ImagemCapa(int id)
        {
            /*
             * Chamando o meu método que vai me trazer a
             * minha capa, pela minha dependência.
             */
            byte[] img = await _api.GetCapaLivroAsync(id);

            if (img != null)
            {
                return File(img, "image/png");
            }
            return File("~/images/capas/capa-vazia.png", "image/png");
        }

        // refatoração do método
        public Livro RecuperaLivro(int id)
        {

            return _repo.Find(id);

        }

        [HttpGet]
        public async Task<IActionResult> Detalhes(int id)
        {
            /*
            * Chamando o meu método que vai me trazer a
            * minha capa, pela minha dependência.
            */
            var model = await _api.GetLivroAsync(id);
            
            if (model == null)
            {
                return NotFound();
            }
            // Retorna uma ViewResult
            return View(model.ToUpload());
        }


        // Alternativa ao método detalhe com asp 
        public ActionResult<LivroUpload> DetalhesJson(int id)
        {

            var model = RecuperaLivro(id);
            if(model == null)
            {
                return NotFound();
            }

            /* 
             * Não precisei usar o metodo Json aqui, pois
             * o ActionResult me possibilita retornar tando um obj
             * quando uma ação http.
            */
            return model.ToModel();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Detalhes(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                if (model.Capa == null)
                {
                    livro.ImagemCapa = _repo.All
                        .Where(l => l.Id == livro.Id)
                        .Select(l => l.ImagemCapa)
                        .FirstOrDefault();
                }
                _repo.Alterar(livro);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remover(int id)
        {
            // Chamando o método que vai retornar o meu obj
            var model = await _api.GetLivroAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            // Passando o livro que vai ser deletado
            await _api.DeleteLivroAsunc(id);
            return RedirectToAction("Index", "Home");
        }
    }
}