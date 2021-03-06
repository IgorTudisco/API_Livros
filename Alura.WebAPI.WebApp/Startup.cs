using Alura.ListaLeitura.HttpClients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Alura.WebAPI.WebApp.Formartters;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Alura.ListaLeitura.WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            
            // Injetando o serviço do contexto http
            services.AddHttpContextAccessor();

            // Mudando minha identificação para o esquema cookies
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            // Passando informações para configurar o meu cookies.
            .AddCookie( opition => {
                opition.LoginPath = "/Usuario/Login";
            });

            // Passando o versionamento para o meu enderaço base.
            services.AddHttpClient<LivroApiClient>(client => {
                client.BaseAddress = new Uri("http://localhost:6000/api/v1.0/");
            });

            // Colocando o serviço de consumo da minha api.
            services.AddHttpClient<AuthApiClient>(client => {

                client.BaseAddress = new Uri("http://localhost:5000/api/");

            });

            services.AddMvc(options => {
                options.OutputFormatters.Add(new LivroCsvFormatter());
            }).AddXmlSerializerFormatters();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}



/*

   Essas foram as referências que utilizei para preparar essa aula.

    Manual da Microsoft para APIs Rest
    https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md

    Iniciando Requisições HTTP (artigo)
    https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests

    Classe HttpClient (referência)
    https://docs.microsoft.com/pt-br/dotnet/api/system.net.http.httpclient

    Paralelismo com C# na Alura
    https://www.alura.com.br/curso-online-csharp-paralelismo-no-mundo-real

    Sobre a convenção de finalizar nomes de métodos com o sufixo Async
    https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/#BKMK_NamingConvention
    
    Cookie Authentication
    https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie

    Como utilizar IHttpContextAccessor
    https://www.strathweb.com/2016/12/accessing-httpcontext-outside-of-framework-components-in-asp-net-core/

    Anúncio da MS indicando que o objeto para acessar o contexto Http não será mais injetado por padrão
    https://github.com/aspnet/Hosting/issues/793

    Artigo de Scott Hanselman sobre o assunto
    https://www.hanselman.com/blog/ASPNETCoreRESTfulWebAPIVersioningMadeEasy.aspx

    Repositório e documentação do pacote AspNet Api Versioning
    https://github.com/Microsoft/aspnet-api-versioning

    Parte do manual da MS que fala sobre coleções com muitos dados
    https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md#94-big-collections

    Artigos que implementam essas soluções e pelos quais me baseei
    Paginação: https://tahirnaushad.com/2017/09/01/paging-in-asp-net-core-2-0-web-api/
    Ordenação: https://tahirnaushad.com/2017/09/01/sorting-in-asp-net-core-2-0-web-api/
    Filtro: https://tahirnaushad.com/2017/09/01/filtering-in-asp-net-core-2-0-web-api/

    Tópico sobre erros no guideline da MS
    https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md#51-errors

    Documentação sobre filtros no AspNet Core
    https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters

    Filtro de ModelState Invalid nos ApiControllers e como desabilitar
    http://www.talkingdotnet.com/disable-automatic-model-state-validation-in-asp-net-core-2-1/

 */