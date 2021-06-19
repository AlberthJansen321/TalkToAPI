using TalkToAPI.V1.Repositories;
using TalkToAPI.V1.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TalkToAPI.Herlpers.Swagger;
using TalkToAPI.V1.Models;
using System.Threading.Tasks;
using AutoMapper;
using TalkToAPI.Helpers.AutoMapper;

namespace TalkToAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region AutoMapper-Config
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new DTOMapperProfile()));
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            #endregion

            #region Usar configurações e  filtragens dos models no controller
            services.Configure<ApiBehaviorOptions>(config =>
            {

                config.SuppressModelStateInvalidFilter = true;

            });
            #endregion

            #region Repositories - ininjeção de dependência
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IMensagemRepository, MensagemRepository>();
            #endregion

            #region Banco de Dados  
            services.AddDbContext<TalkToDBcontext>(config =>
               config.UseSqlite(Configuration.GetConnectionString("SQLLite"))
            );
            #endregion

            #region Versionamento
            services.AddApiVersioning(cfg =>
            {
                cfg.ReportApiVersions = true;
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            });
            #endregion

            #region Configuração do SwaggerGen
            services.AddSwaggerGen(cfg =>
            {
                //adicionar TokenJWT no Swagger
                cfg.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "Adicione o JSON Web Token(JWT) para autenticar.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"

                });

                cfg.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                { new OpenApiSecurityScheme {
                 Reference = new OpenApiReference{
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme
                 }
                 },new List<string>()
                 }});
                //------------------------------------------------------------------

                cfg.ResolveConflictingActions(conflito => conflito.First());

                cfg.SwaggerDoc("v1.0", new OpenApiInfo()
                {
                    Title = "TalkToAPI - V1.0",
                    Version = "V1.0"
                });


                var CaminhoProjeto = PlatformServices.Default.Application.ApplicationBasePath;
                var NomeArquivo = $"{PlatformServices.Default.Application.ApplicationName}.xml";
                var ArquivodeComentario = Path.Combine(CaminhoProjeto, NomeArquivo);
                cfg.IncludeXmlComments(ArquivodeComentario);

                cfg.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();
                // would mean this action is unversioned and should be included everywhere
                if (actionApiVersionModel == null)
                    {
                        return true;
                    }
                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
                    }
                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });

                cfg.OperationFilter<ApiVersionOperationFilter>();

            });
            #endregion

            #region adicionar o service do identity via token
            services.AddIdentity<ApplicationUSER, IdentityRole>()
                        .AddEntityFrameworkStores<TalkToDBcontext>()
                        .AddDefaultTokenProviders();
            #endregion

            #region Concerta o erro de loop infinito json entre outras configurações
            services.AddMvc(config =>
            {
                config.EnableEndpointRouting = false;
                config.ReturnHttpNotAcceptable = true;
                config.InputFormatters.Add(new XmlSerializerInputFormatter(config));
                config.OutputFormatters.Add(new XmlSerializerOutputFormatter());

            }).AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            #endregion

            #region Redirecionamento de Login e cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;

                };
            });
            #endregion

            #region Adicionar autenticação por Token Jwt
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Chavetoken"]))
                };

            });
            //
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build());
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                        c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "TalkToAPI - V1.0");
                });
            }
            else
            {
                app.UseHsts();
            }

            app.UseStatusCodePages();
            app.UseAuthentication();
            app.UseHttpsRedirection();

            app.UseMvc();



        }
    }
}
