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
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            // Abilitando o gerenciamento do Swagger
            services.AddSwaggerGen(options => {

                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                    var versions = methodInfo.DeclaringType
                        .GetCustomAttributes(true)
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    return versions.Any(v => $"v{v.ToString()}" == docName);
                });

                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey",
                    Description = "Autenticação Bearer via JWT"
                });
                options.AddSecurityRequirement(
                    new Dictionary<string, IEnumerable<string>> {
                        { "Bearer", new string[] { } }
                });

                options.EnableAnnotations();

                options.DescribeAllEnumsAsStrings();
                options.DescribeStringEnumsInCamelCase();

                options.DocumentFilter<TagDescriptionsDocumentFilter>();
                options.OperationFilter<AuthResponsesOperationFilter>();
                options.OperationFilter<AddInfoToParamVersionOperationFilter>();

                options.SwaggerDoc("v1.0", new Info { Title = "Lista de Leitura API - v1.0", Version = "1.0" });
                options.SwaggerDoc(
                    "v2.0",
                    new Info
                    {
                        Title = "Lista de Leitura API",
                        Description = "API com serviços relacionados às listas de leitura, produzidas para a Alura.",
                        Version = "2.0"
                    }
                );
            });

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();

            }

            app.UseAuthentication();

            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c => {

                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Versão 1.0");

                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Versão 2.0");

            });

        }

    }
}

/*
 * 
 * Indicação de leitura. https://github.com/dotnet/aspnet-api-versioning/wiki
 * 
 */