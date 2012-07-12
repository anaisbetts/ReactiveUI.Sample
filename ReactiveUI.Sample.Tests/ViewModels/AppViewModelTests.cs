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
using ReactiveUI.Routing;

namespace ReactiveUI.Sample.ViewModels.Tests
{
    public class AppViewModelTests : IEnableLogger
    {
        [Fact]
        public void LoginScenarioRoutingTest()
        {
            var mock = new MoqMockingKernel();
            mock.Bind<ILoginViewModel>().To<LoginViewModel>();

            // Fake out the actual login code that makes sure the password is right
            mock.Bind<Func<IObservable<Unit>>>()
                .ToConstant<Func<IObservable<Unit>>>(() => Observable.Return(Unit.Default))
                .Named("confirmUserPass");

            var fixture = new AppViewModel(mock);

            // Our app starts on the Login page by default
            this.Log().Info("Current Route: {0}", fixture.Router.GetUrlForCurrentRoute());
            var loginModel = fixture.Router.GetCurrentViewModel() as ILoginViewModel;
            loginModel.Should().NotBeNull();

            // Put in a fake user/pass and hit the Ok button
            loginModel.User = "foo";
            loginModel.Password = "bar";
            loginModel.Confirm.Execute(null);

            // Make sure we're now showing the repo page
            this.Log().Info("Current Route: {0}", fixture.Router.GetUrlForCurrentRoute());
            (fixture.Router.GetCurrentViewModel() is IRepoSelectionViewModel).Should().BeTrue();
        }
    }
}