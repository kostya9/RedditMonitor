using System;
using Amazon.SQS;
using KPI.RedditMonitor.Application.Similarity;
using KPI.RedditMonitor.Collector.RedditPull;
using KPI.RedditMonitor.Collector.RedditPull.Collectors;
using KPI.RedditMonitor.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Swagger;

namespace KPI.RedditMonitor.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MongoDbConfig>(c => Configuration.Bind("MongoDb", c));
            services.Configure<RedditOptions>(o => Configuration.Bind("Reddit", o));
            services.Configure<PostQueueOptions>(o => Configuration.Bind("PostQueue", o));

            services.AddSingleton<ImagePostsRepository>();
            services.AddSingleton<IRedditCollector, RedditNetCollector>();
            services.AddSingleton<PostInserter>();
            services.AddSingleton<RedditPullStats>();

            services.AddAWSService<IAmazonSQS>();

            if (Configuration.GetValue<bool>("RedditCollector:Run"))
            {
                services.AddHostedService<RedditCollectorService>();
            }

            services.AddSingleton<IMongoClient>(p =>
            {
                var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
                ConventionRegistry.Register("ignoreExtra", conventionPack, t => true);
                var config = p.GetRequiredService<IOptions<MongoDbConfig>>();
                return new MongoClient(config.Value.ConnectionString);
            });

            services.AddSingleton<ImagePostsRepository>();
            services.AddSingleton<TopImageDbAdapter>();
            services.AddSingleton<SimilarityService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "RedditMonitor API", Version = "v1" });
            });

            services.AddCors();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(b =>
            {
                b.WithOrigins("http://localhost:8080")
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
