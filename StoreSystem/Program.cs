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

            //註冊Filter
            builder.Services.AddMvc(options =>
            {
                options.Filters.Add(typeof(AuthFilter));
            });


            #region Cookie & Session

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(opt =>
            {
                opt.Cookie.HttpOnly = true; // 保護 Cookie，不允許 JavaScript 訪問
                opt.Cookie.IsEssential = true; // 強制要求 Cookie 是必要的
            });

            builder.Services.Configure<CookiePolicyOptions>(opt =>
            {
                // 啟用 GDPR 同意檢查（可選）
                opt.CheckConsentNeeded = context => true;
                // 設置 SameSite 屬性
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
