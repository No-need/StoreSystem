using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using StoreSystem.Database;
using StoreSystem.Filter;
using StoreSystem.Services;

namespace StoreSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            //���UFilter
            builder.Services.AddMvc(options =>
            {
                options.Filters.Add(typeof(AuthFilter));
            });


            #region Cookie & Session

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(opt =>
            {
                opt.Cookie.HttpOnly = true; // �O�@ Cookie�A�����\ JavaScript �X��
                opt.Cookie.IsEssential = true; // �j��n�D Cookie �O���n��
            });

            builder.Services.Configure<CookiePolicyOptions>(opt =>
            {
                // �ҥ� GDPR �P�N�ˬd�]�i��^
                opt.CheckConsentNeeded = context => true;
                // �]�m SameSite �ݩ�
                opt.MinimumSameSitePolicy = SameSiteMode.None;
            });

            #endregion

            builder.Services.AddDbContext<StoreSystemContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("StoreSystemConnection"));
            });

            builder.Services.AddScoped<StoreService>();
            builder.Services.AddScoped<FileService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
