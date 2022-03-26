using Alura.ListaLeitura.Api.Formartters;
using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Alura.WebAPI.Api.Filtros;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api
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


            services.AddTransient<IRepository<Livro>, RepositorioBaseEF<Livro>>();

            // Add opção de formatação.
            services.AddMvc(

                options =>
                {
                    // Formatação
                    options.OutputFormatters.Add(new LivroCsvFormatter());

                    // Passando o Filtro de erro para toda a minha API.
                    options.Filters.Add(typeof(ErrorResponseFilter));
                }

                ).AddXmlSerializerFormatters();

            // Desabilitando a função automática da API Controller
            services.Configure<ApiBehaviorOptions>(
                
                option =>
                {
                    option.SuppressModelStateInvalidFilter = true;
                }
            );

            services.AddAuthentication(

            options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";

            }).AddJwtBearer("JwtBearer", options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("zika-igor-authentication-valid")),
                    ClockSkew = TimeSpan.FromMinutes(5),
                                // Nome dos meus validadores.
                                ValidIssuer = "Alura.WebApp",
                    ValidAudience = "Postman",
                };
            });


            // Add meu versionamento
            // No curso foi decidido que o versionamento será feito pela URL.
            // A url só aceita o versionamento por ela, sendo a menos flexivel das opções
            // Mas por uma questão de facilidade, vamos usa-lá. 
            services.AddApiVersioning();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();

            }

            app.UseAuthentication();

            app.UseMvc();


        }

    }
}

/*
 * 
 * Indicação de leitura. https://github.com/dotnet/aspnet-api-versioning/wiki
 * 
 */