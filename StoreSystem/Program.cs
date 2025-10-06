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
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()    // ���\�Ҧ��ӷ�
                        .AllowAnyMethod()    // ���\�Ҧ� HTTP ��k
                        .AllowAnyHeader();   // ���\�Ҧ����Y
                });
            });

            var app = builder.Build();

            // �K�[�۩w�q CORS ������
            app.Use(async (context, next) =>
            {
                var origin = context.Request.Headers["Origin"];
                if (!string.IsNullOrEmpty(origin)) //�p�G�n�ϥΥզW�檺�覡�i��G && allowedOrigins.Contains(origin)
                {
                    // �o�̥i�H�ھڻݭn�[�J��h���ӷ��L�o�޿�
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
