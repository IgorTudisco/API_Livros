using Alura.ListaLeitura.Seguranca;
using Alura.WebAPI.WebApp.Formartters;
using Alura.ListaLeitura.HttpClients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

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
            
            services.AddDbContext<AuthDbContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("AuthDB"));
            });

            services.AddIdentity<Usuario, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            }).AddEntityFrameworkStores<AuthDbContext>();

            /*
             * Passando um serviço que vai contruir o meu obj e
             * vai receber o meu base endpoint. Assim eu desaclopo essa
             * funcionalidade da minha class.
             */
            services.AddHttpClient<LivroApiClient>(client => {
                client.BaseAddress = new Uri("http://localhost:6000/api/");
            });

            services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/Usuario/Login";
            });


            // Add opção de formatação.
            services.AddMvc(

                options => { options.OutputFormatters.Add(new LivroCsvFormatter()); }

                ).AddXmlSerializerFormatters();
                        
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
 
 */