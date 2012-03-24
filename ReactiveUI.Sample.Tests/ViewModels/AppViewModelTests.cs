using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using FluentAssertions;
using Ninject.MockingKernel.Moq;
using Xunit;
using ReactiveUI.Sample.ViewModels;

namespace ReactiveUI.Sample.ViewModels.Tests
{
    public class AppViewModelTests : IEnableLogger
    {
        [Fact]
        public void LoginScenarioRoutingTest()
        {
            var mock = new MoqMockingKernel();
            mock.Bind<ILoginViewModel>().To<LoginViewModel>();

            mock.Bind<Func<IObservable<Unit>>>()
                .ToConstant<Func<IObservable<Unit>>>(() => Observable.Return(Unit.Default))
                .Named("confirmUserPass");

            var fixture = new AppViewModel(mock);

            this.Log().Info("Current Route: {0}", fixture.Router.GetUrlForCurrentRoute());
            var loginModel = fixture.Router.CurrentViewModel.First() as ILoginViewModel;
            loginModel.Should().NotBeNull();

            loginModel.User = "foo";
            loginModel.Password = "bar";
            loginModel.Confirm.Execute(null);

            this.Log().Info("Current Route: {0}", fixture.Router.GetUrlForCurrentRoute());
            (fixture.Router.CurrentViewModel.First() is IRepoViewModel).Should().BeTrue();
        }
    }
}