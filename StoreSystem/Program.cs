using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using StoreSystem.Database;
using StoreSystem.Filter;
using StoreSystem.Services;
using System.Net;

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
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()    // 允許所有來源
                        .AllowAnyMethod()    // 允許所有 HTTP 方法
                        .AllowAnyHeader();   // 允許所有標頭
                });
            });

            var app = builder.Build();

            // 添加自定義 CORS 中間件
            app.Use(async (context, next) =>
            {
                var origin = context.Request.Headers["Origin"];
                if (!string.IsNullOrEmpty(origin)) //如果要使用白名單的方式進行： && allowedOrigins.Contains(origin)
                {
                    // 這裡可以根據需要加入更多的來源過濾邏輯
                    context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET,HEAD,OPTIONS,POST,PUT");
                    context.Response.Headers.Add("Access-Control-Allow-Headers", "ngrok-skip-browser-warning, Origin, X-Requested-With, Content-Type, Accept, Authorization");
                    context.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
                }

                if (context.Request.Method == HttpMethods.Options)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    return;
                }

                await next();
            });
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            //app.UseCors("AllowAll");
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
