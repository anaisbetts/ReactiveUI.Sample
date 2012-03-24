using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Xunit;

namespace ReactiveUI.Sample.ViewModels.Tests
{
    public class LoginViewModelTests : IEnableLogger
    {
        [Fact]
        public void SuccessfulLoginShouldActuallyLogIn()
        {
            var fixture = new LoginViewModel();

            fixture.User = "xpaulbettsx";
            fixture.Password = "62etmdtmwtmTM!";

            fixture.Confirm.CanExecute(null).Should().BeTrue();
            fixture.Confirm.Execute(null);

            var result = MessageBus.Current.Listen<Tuple<string, string>>("login")
                .Timeout(TimeSpan.FromSeconds(10), RxApp.TaskpoolScheduler)
                .First();

            result.Should().NotBeNull();
            result.Item1.Should().Be(fixture.User);
            result.Item2.Should().Be(fixture.Password);
        }

        [Fact]
        public void BlankFieldsMeansNoLogin()
        {
            var fixture = new LoginViewModel();

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
            var fixture = new LoginViewModel();

            fixture.User = "herpderp";
            fixture.Password = "woefawoeifjwoefijwe";

            fixture.Confirm.CanExecute(null).Should().BeTrue();
            fixture.Confirm.Execute(null);

            var result = fixture.ObservableForProperty(x => x.ErrorMessage)
                .Timeout(TimeSpan.FromSeconds(10), RxApp.TaskpoolScheduler)
                .First();

            this.Log().Info("Error Message: {0}", result.Value);
            result.Value.Should().NotBeNullOrEmpty();
        }
    }
}
