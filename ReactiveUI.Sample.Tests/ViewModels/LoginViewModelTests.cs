using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Moq;
using Ninject;
using Ninject.MockingKernel;
using Ninject.MockingKernel.Moq;
using ReactiveUI.Routing;
using ReactiveUI.Testing;
using Xunit;

namespace ReactiveUI.Sample.ViewModels.Tests
{
    public class LoginViewModelTests : IEnableLogger
    {
        [Fact]
        public void SuccessfulLoginShouldActuallyLogIn()
        {
            var mock = new MoqMockingKernel();
            mock.Bind<ILoginViewModel>().To(typeof(LoginViewModel));

            mock.Bind<Func<IObservable<Unit>>>()
                .ToConstant<Func<IObservable<Unit>>>(() => Observable.Return(Unit.Default))
                .Named("confirmUserPass");

            var fixture = mock.Get<ILoginViewModel>();

            fixture.User = "xpaulbettsx";
            fixture.Password = "theCorrectPassword";

            fixture.Confirm.CanExecute(null).Should().BeTrue();

            Tuple<string, string> result = null;
            MessageBus.Current.Listen<Tuple<string, string>>("login").Subscribe(x => result = x);
            fixture.Confirm.Execute(null);

            result.Should().NotBeNull();
            result.Item1.Should().Be(fixture.User);
            result.Item2.Should().Be(fixture.Password);
        }

        [Fact]
        public void BlankFieldsMeansNoLogin()
        {
            var mock = new MoqMockingKernel();
            mock.Bind<ILoginViewModel>().To(typeof(LoginViewModel));

            var fixture = mock.Get<ILoginViewModel>();

            fixture.Confirm.CanExecute(null).Should().BeFalse();

            fixture.User = "Foo";
            fixture.Confirm.CanExecute(null).Should().BeFalse();

            fixture.User = "";
            fixture.Password = "Bar";
            fixture.Confirm.CanExecute(null).Should().BeFalse();

            fixture.User = "Foo";
            fixture.Confirm.CanExecute(null).Should().BeTrue();
        }


        [Fact]
        public void BadPasswordMeansErrorMessage()
        {
            var mock = new MoqMockingKernel();

            mock.Bind<ILoginViewModel>().To(typeof(LoginViewModel));

            mock.Bind<Func<IObservable<Unit>>>()
                .ToConstant<Func<IObservable<Unit>>>(() => Observable.Throw<Unit>(new Exception("Bad Stuff")))
                .Named("confirmUserPass");

            var fixture = mock.Get<ILoginViewModel>();

            fixture.User = "herpderp";
            fixture.Password = "woefawoeifjwoefijwe";

            fixture.Confirm.CanExecute(null).Should().BeTrue();

            string result = null;
            fixture.ObservableForProperty(x => x.ErrorMessage).Subscribe(x => result = x.Value);
            fixture.Confirm.Execute(null);

            this.Log().Info("Error Message: {0}", result);
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void LoginFailureIntegrationTest()
        {
            var mock = new MoqMockingKernel();

            mock.Bind<ILoginViewModel>().To(typeof(LoginViewModel));

            var fixture = mock.Get<ILoginViewModel>() as LoginViewModel;
            fixture.User = "herpderp";
            fixture.Password = "woefawoeifjwoefijwe";

            fixture.TestUserNameAndPassword().Select(_ => false)
                .Catch(Observable.Return(true))
                .Timeout(TimeSpan.FromSeconds(10), RxApp.TaskpoolScheduler)
                .First()
                .Should().BeTrue();
        }
    }
}