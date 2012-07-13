using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Ninject;
using ReactiveUI.Routing;
using RestSharp;

namespace ReactiveUI.Sample.ViewModels
{
    public interface IAppViewModel : IReactiveNotifyPropertyChanged
    {
        IRestClient CreateClient();
    }

    public class AppViewModel : ReactiveObject, IAppViewModel, IScreen
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

        public IRoutingState Router { get; protected set; }

        public AppViewModel(IKernel testKernel)
        {
            Router = new RoutingState();

            Kernel = testKernel ?? Kernel;
            Kernel.Bind<IScreen>().ToConstant(this);
            Kernel.Bind<IAppViewModel>().ToConstant(this);

            MessageBus.Current.Listen<Tuple<string, string>>("login").Subscribe(login => {
                User = login.Item1;
                Password = login.Item2;

                Router.NavigateAndReset.Execute(Kernel.Get<IRepoSelectionViewModel>());
            });

            Router.Navigate.Execute(Kernel.Get<ILoginViewModel>());
        }

        public IRestClient CreateClient()
        {
            return new RestClient("https://api.github.com") {
                Authenticator = new HttpBasicAuthenticator(User, Password)
            };
        }


        //
        // NInject static setup
        //

        static AppViewModel()
        {
            Kernel = new StandardKernel();
            ServiceLocator.SetLocatorProvider(() => new NInjectServiceLocator());

            Kernel.Bind<ILoginViewModel>().To<LoginViewModel>();
            Kernel.Bind<IRepoSelectionViewModel>().To<RepoSelectionViewModel>();
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
