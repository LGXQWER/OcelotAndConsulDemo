using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API1
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
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Microsoft.AspNetCore.Hosting.IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //注册项目启动的方法
            lifetime.ApplicationStarted.Register(OnStart);
            //注册项目关闭的方法
            lifetime.ApplicationStarted.Register(OnStopped);
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        //关闭的时候在consul中移除
        private void OnStopped()
        {
            var client = new ConsulClient();
            //根据ID在consul中移除当前服务
            client.Agent.ServiceDeregister("servicename:93");
        }
        private void OnStart()
        {
            var client = new ConsulClient();
            //健康检查
            var httpCheck = new AgentServiceCheck()
            {
                //服务出错一分钟后 会自动移除
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                //每10秒发送一次请求到 下面的这个地址 这个地址就是当前API资源的地址
                Interval = TimeSpan.FromSeconds(10),
                HTTP = $"http://localhost:93/HealthCheck"
            };

            var agentReg = new AgentServiceRegistration()
            {
                //这台资源服务的唯一ID
                ID = "servicename:93",
                Check = httpCheck,
                Address = "localhsot",
                Name = "servicename",
                Port = 93
            };
            client.Agent.ServiceRegister(agentReg).ConfigureAwait(false);
        }
    }
}
