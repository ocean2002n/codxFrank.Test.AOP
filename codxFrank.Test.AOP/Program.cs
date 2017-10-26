using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace codxFrank.Test.AOP
{
    #region Sample 1

    public class ApplicationInterceptor : IInterceptor
    {
        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine("call started " + invocation.Method.Name);
            invocation.Proceed();
            Console.WriteLine("call ended " + (invocation.ReturnValue ?? "NULL"));
        }

        #endregion
    }

    public interface IUserApplication
    {
        void Show();
    }

    public class UserApplication : IUserApplication
    {
        private static IUserResoistory _userResoistory;

        public UserApplication(IUserResoistory userResoistory)
        {
            _userResoistory = userResoistory;
        }
        public void Show()
        {
            Console.WriteLine(_userResoistory.Name);
        }
    }

    public interface IUserResoistory
    {
        string Name { get; }
    }

    public class UserResoistory : IUserResoistory
    {
        public string Name { get; private set; }

        public UserResoistory()
        {
            Name = "Frank";
        }
    }

    #endregion

    #region Sample2

    public interface ILogger
    {
        void Debug(string msg);
    }

    public class ConsoleLogger : ILogger
    {
        public void Debug(string msg)
        {
            Console.WriteLine("Console Debug :" + msg);
        }
    }

    public class ConsoleWithTimeLogger : ILogger
    {
        public void Debug(string msg)
        {
            Console.WriteLine("Console Debug :" + msg + Environment.NewLine + "Time :" + DateTime.Now);
        }
    }

    public interface IWindsorInstaller
    {
        void Install(IWindsorContainer container, IConfigurationStore store);
    }

    public class MyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ILogger>().ImplementedBy<ConsoleLogger>().LifestyleSingleton());

            //            //CastleWindsor.IEntity是实现类所在的空间
            //            container.Register(Classes.FromThisAssembly().InNamespace("CastleWindsor.IEntity").WithService.DefaultInterfaces());

            //            //继承两个接口
            //             container.Register(
            //             Component.For<IUserRepository, IRepository>()
            //             .ImplementedBy<MyRepository>()
            //            );

            //            //简单工厂
            //            container
            //             .Register(
            //             Component.For<IMyService>()
            //             .UsingFactoryMethod(
            //             () => MyLegacyServiceFactory.CreateMyService())
            //             );

            //            //泛型配置
            //            container.Register(
            //             Component.For(typeof(IRepository<>)
            //             .ImplementedBy(typeof(NHRepository<>)
            //            );

            //            //实体生命周期
            //            container.Register(
            //             Component.For<IMyService>()
            //             .ImplementedBy<MyServiceImpl>()
            //             .LifeStyle.Transient
            //            .Named("myservice.default")
            //             );

            //            //取先注册的
            //            container.Register(
            //             Component.For<IMyService>().ImplementedBy<MyServiceImpl>(),
            //             Component.For<IMyService>().ImplementedBy<OtherServiceImpl>()
            //            );

            //            //强制取后注册的
            //            container.Register(
            //             Component.For<IMyService>().ImplementedBy<MyServiceImpl>(),
            //             Component.For<IMyService>().Named("OtherServiceImpl").ImplementedBy<OtherServiceImpl>().IsDefault()
            //            );

            //            //注册已经存在的
            //            var customer = new CustomerImpl();
            //            container.Register(
            //             Component.For<ICustomer>().Instance(customer)
            //             );

        }
    }


    //public class WindsorInit
    //{
    //    private static WindsorContainer _container;
    //    public static WindsorContainer GetContainer()
    //    {
    //        if (_container == null)
    //        {
    //            _container = new WindsorContainer();
    //            _container.Install(
    //                new MyInstaller()
    //            );
    //        }
    //        return _container;
    //    }

    //    public void CloseContex()
    //    {
    //        _container.Dispose();
    //    }
    //}

    #endregion

    class Program
    {
        static void Main(string[] args)
        {
            #region Sample2
            IWindsorContainer container2 = new WindsorContainer();
            container2.Register(Component.For<ILogger>().ImplementedBy<ConsoleWithTimeLogger>().LifestyleSingleton());

            var logger = container2.Resolve<ILogger>();
            logger.Debug("Frank");
            //ILogger logger = WindsorInit.GetContainer().Resolve<ILogger>();
            //logger.Debug("记录日志");
            Console.ReadKey();
            #endregion

            #region Sample1

            IWindsorContainer container = new WindsorContainer();
            container.Register(Classes.FromAssembly(Assembly.GetExecutingAssembly()).BasedOn<IUserResoistory>().WithServiceAllInterfaces());
            container.Register(Component.For<IInterceptor>().ImplementedBy<ApplicationInterceptor>().Named("myinterceptor"));
            container.Register(Component.For<IUserApplication>().ImplementedBy<UserApplication>().Interceptors(InterceptorReference.ForKey("myinterceptor")).Anywhere);

            var userApplication = container.Resolve<IUserApplication>();

            userApplication.Show();

            Console.ReadKey();

            #endregion




        }
    }

   
}
