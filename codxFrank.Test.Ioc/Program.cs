using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using codxFrank.Test.Ioc.QueryAccountSummaryService;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.InternetBank.Services;
using IQueryAccountSummaryService = Microsoft.InternetBank.Services.AccountQuery.IQueryAccountSummaryService;

namespace codxFrank.Test.Ioc
{
    class Program
    {
        static void Main(string[] args)
        {
            var aaa = new QueryAccountSummaryServiceClient();
            var bbb = aaa.FetchAuthorizationAccounts(true);

            var container = GetContainer2();
            var reverser = container.Resolve<IQueryAccountSummaryService>();

            //var input = Console.ReadLine();
            var output = reverser.FetchAuthorizationAccounts(true);
            Console.WriteLine(output);
        }

        private static WindsorContainer GetContainer2()
        {
            var _container = new WindsorContainer();
            _container = new WindsorContainer();
            _container.AddFacility<WcfFacility>();
            _container.Register(
                Component.For<IQueryAccountSummaryService>()
                    .AsWcfClient(new DefaultClientModel(WcfEndpoint
                        .BoundTo(new NetTcpBinding { PortSharingEnabled = true })
                        //.BoundTo(new NetTcpBinding("NetTcpBinding_IQueryAccountSummaryService"))
                        .At("net.tcp://sk0341vm231190.mktb.com.tw/SKB.iBankService/QueryAccountSummaryService.svc")))
            //Component.For<IQueryAccountSummaryService>()
            //    .AsWcfClient(new DefaultClientModel(WcfEndpoint.
            //        BoundTo(new BasicHttpBinding("defaultBinding"))
            //        .At("http://sk0341vm231190.mktb.com.tw/SKB.iBankService/QueryAccountSummaryService.svc")))
            );
            return _container;
        }

        private static WindsorContainer GetContainer()
        {
            var container = new WindsorContainer();
            container.Register(
                Component.For<IQueryAccountSummaryService>()
                    .AsWcfClient(WcfEndpoint.FromConfiguration("*"))
            );
            return container;
        }
    }

    //public static class WindsorContainerExtension
    //{
    //    private static readonly CommonLog.ILogger logger = CommonLog.LogManager.GetCurrentClassLogger();

    //    /// <summary>
    //    /// 註冊WcfClient
    //    /// </summary>
    //    /// <param name="container"></param>
    //    /// <param name="type">要註冊的介面或實體</param>
    //    /// <param name="configurationNodeName">web.config內的endpoint節點name</param>
    //    public static void Register(this IWindsorContainer container, Type type, string configurationNodeName = "")
    //    {

    //        if (!container.Kernel.HasComponent(type.Name))
    //        {
    //            if (string.IsNullOrEmpty(configurationNodeName) && (type.IsInterface && type.Name.StartsWith("I") && type.Name.EndsWith("Service")))
    //                configurationNodeName = type.Name.Remove(0, 1);

    //            logger.Info($"register [{type.FullName}] by Name: {type.Name} and Endpoint name: {configurationNodeName}");

    //            container
    //                .Register(
    //                    Component.For(type)
    //                        .Named(type.Name)
    //                        .AsWcfClient(new DefaultClientModel(WcfEndpoint.FromConfiguration(configurationNodeName)))
    //                        //.Interceptors<LogInterceptor>()
    //                        .LifestyleTransient()
    //                );
    //        }
    //    }
    //}
}
