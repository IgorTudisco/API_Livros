using Alura.ListaLeitura.Persistencia;
using Alura.ListaLeitura.Seguranca;
using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Alura.WebAPI.WebApp.Formartters;
using System;
using Microsoft.IdentityModel.Tokens;
using Alura.ListaLeitura.HttpClients;

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
            services.AddDbContext<LeituraContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("ListaLeitura"));
            });

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

            services.AddTransient<IRepository<Livro>, RepositorioBaseEF<Livro>>();

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
