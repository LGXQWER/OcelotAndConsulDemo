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
            //ע����Ŀ�����ķ���
            lifetime.ApplicationStarted.Register(OnStart);
            //ע����Ŀ�رյķ���
            lifetime.ApplicationStarted.Register(OnStopped);
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        //�رյ�ʱ����consul���Ƴ�
        private void OnStopped()
        {
            var client = new ConsulClient();
            //����ID��consul���Ƴ���ǰ����
            client.Agent.ServiceDeregister("servicename:93");
        }
        private void OnStart()
        {
            var client = new ConsulClient();
            //�������
            var httpCheck = new AgentServiceCheck()
            {
                //�������һ���Ӻ� ���Զ��Ƴ�
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                //ÿ10�뷢��һ������ ����������ַ �����ַ���ǵ�ǰAPI��Դ�ĵ�ַ
                Interval = TimeSpan.FromSeconds(10),
                HTTP = $"http://localhost:93/HealthCheck"
            };

            var agentReg = new AgentServiceRegistration()
            {
                //��̨��Դ�����ΨһID
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
