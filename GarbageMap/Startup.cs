using GarbageMap.GarbageDetection.Helpers;
using GarbageMap.GarbageDetection.ML;
using GarbageMap.GarbageDetection.ML.DataModels;
using GarbageMap.GarbageDetection.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GarbageMap.Models.DbModels;
using GarbageMap.Models.Initializer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ML;

namespace GarbageMap
{
    public class Startup
    {
        private readonly string _garbageDetectionModelPath;
        private readonly string _zipArchiveModelPath;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _garbageDetectionModelPath = CommonHelpers.GetAbsolutePath(Configuration["Path:GarbageDetectionModelPath"]);
            _zipArchiveModelPath = CommonHelpers.GetAbsolutePath(Configuration[$"Path:ZipArchiveModelPath"]);

            var garbageDetectionModelConfigurator = new GarbageDetectionModelConfigurator(new TinyYoloModel(_garbageDetectionModelPath));

            garbageDetectionModelConfigurator.SaveMLNetModel(_zipArchiveModelPath);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IndividualPerson, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddPredictionEnginePool<ImageInputData, TinyYoloPrediction>().FromFile(_zipArchiveModelPath);

            services.AddTransient<IImageFileWriter, ImageFileWriter>();
            services.AddTransient<IObjectDetectionService, ObjectDetectionService>();

            services.AddHttpClient();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context, UserManager<IndividualPerson> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
