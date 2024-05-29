using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MessagingApp1.Data;

namespace MessagingApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Настройка контекста базы данных
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Добавление сервисов MVC
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "account",
                    pattern: "Account/Register",
                    defaults: new { controller = "Account", action = "Register" });

                endpoints.MapControllerRoute(
                    name: "account",
                    pattern: "Account/Login",
                    defaults: new { controller = "Account", action = "Login" });

                endpoints.MapControllerRoute(
                    name: "account",
                    pattern: "Account/Messages/{userId}",
                    defaults: new { controller = "Account", action = "Messages" });

                endpoints.MapControllerRoute(
                    name: "account",
                    pattern: "Account/GetMessageText/{id}",
                    defaults: new { controller = "Account", action = "GetMessageText" });

                endpoints.MapControllerRoute(
                    name: "account",
                    pattern: "Account/SendMessage/{userId}/{recipient}/{subject}/{text}",
                    defaults: new { controller = "Account", action = "SendMessage" });
            });
        }
    }
}
