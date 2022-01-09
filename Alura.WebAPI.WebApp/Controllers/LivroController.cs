using System.Linq;
using Alura.ListaLeitura.Persistencia;
using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.WebApp.Controllers
{
    [Authorize]
    public class LivroController : Controller
    {
        private readonly IRepository<Livro> _repo;

        public LivroController(IRepository<Livro> repository)
        {
            _repo = repository;
        }

        [HttpGet]
        public IActionResult Novo()
        {
            return View(new LivroUpload());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Novo(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                _repo.Incluir(model.ToLivro());
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ImagemCapa(int id)
        {
            byte[] img = _repo.All
                .Where(l => l.Id == id)
                .Select(l => l.ImagemCapa)
                .FirstOrDefault();
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
            //http://localhost:6000/api/Livros/{id}
            //http://localhost:6000/api/ListasLeitura/paraler
            //http://localhost:6000/api/Livros/{id}/capa

            // Consumindo a minha API REST

            // Criando um obj http
            HttpClient httpClient = new HttpClient();

            /*
             * Passando um endereço/endpoint que será igual para as
             * minhas requisições.
             */
            httpClient.BaseAddress = new System.Uri("http://localhost:6000/api/");

            /*
             * Criando a variá vel que vai conter a minha resposta.
             * Como eu preciso esperar que a minha API retorne
             * uma resposta. Ela deve ser assincrona.
             * Fazendo a interpolação com o meu endpoint, passando
             * o que falta na minha uri.
            */
            HttpResponseMessage resposta = await httpClient.GetAsync($"Livros/{id}");

            /*
             * Método que verifica que eu recebi um status dá família
             * 200. Se eu receber ele não faz nada, mas se eu não receber
             * ele vai lançar uma exceção. Assim garantindo que tenha um status 200 
             */
            resposta.EnsureSuccessStatusCode();

            // Convertendo a minha resposta pra um tipo livro.
            var model = await resposta.Content.ReadAsAsync<LivroApi>();
            
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
        public IActionResult Remover(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            _repo.Excluir(model);
            return RedirectToAction("Index", "Home");
        }
    }
}