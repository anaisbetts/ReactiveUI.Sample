using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Ninject;
using ReactiveUI.Routing;

namespace ReactiveUI.Sample.ViewModels
{
    public class AppViewModel : ReactiveObject, IScreen
    {
        string _User;
        public string User
        {
            get { return _User; }
            set { this.RaiseAndSetIfChanged(x => x.User, value); }
        }

        string _Password;
        public string Password
        {
            get { return _Password; }
            set { this.RaiseAndSetIfChanged(x => x.Password, value); }
        }

        public RoutingState Router { get; protected set; }

        public AppViewModel(IKernel testKernel)
        {
            Router = new RoutingState();

            Kernel = testKernel ?? Kernel;
            Kernel.Bind<IScreen>().ToConstant(this);

            MessageBus.Current.Listen<Tuple<string, string>>("login").Subscribe(login => {
                User = login.Item1;
                Password = login.Item2;

                Router.NavigateAndReset.Execute(Kernel.Get<IRepoViewModel>());
            });

            Router.Navigate.Execute(Kernel.Get<ILoginViewModel>());
        }


        //
        // NInject static setup
        //

        static AppViewModel()
        {
            Kernel = new StandardKernel();
            ServiceLocator.SetLocatorProvider(() => new NInjectServiceLocator());

            Kernel.Bind<ILoginViewModel>().To<LoginViewModel>();
            Kernel.Bind<IRepoViewModel>().To<RepoViewModel>();
        }

        public static IKernel Kernel { get; set; }
    }

    class NInjectServiceLocator : ServiceLocatorImplBase
    {
        protected override object DoGetInstance(Type serviceType, string key)
        {
            return AppViewModel.Kernel.Get(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return AppViewModel.Kernel.GetAll(serviceType);
        }
    }
}
