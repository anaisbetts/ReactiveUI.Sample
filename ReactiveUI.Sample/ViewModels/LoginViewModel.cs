using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using Ninject;
using ReactiveUI;
using ReactiveUI.Routing;
using ReactiveUI.Sample.Models;
using ReactiveUI.Xaml;
using RestSharp;
using ReactiveUI.Sample.Helpers;

namespace ReactiveUI.Sample.ViewModels
{
    public interface ILoginViewModel : IRoutableViewModel
    {
        string User { get; set; }
        string Password { get; set; }
        ReactiveAsyncCommand Confirm { get; }
    }

    public class LoginViewModel : ReactiveObject, ILoginViewModel
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

        public string UrlPathSegment
        {
            get { return "login"; }
        }

        public IScreen HostScreen { get; protected set; }

        public ReactiveAsyncCommand Confirm { get; protected set; }

        [Inject]
        public LoginViewModel(IScreen hostScreen, [Named("confirmUserPass")] [Optional] Func<IObservable<Unit>> confirmUserPassMock = null)
        {
            HostScreen = hostScreen;

            var canConfirm = this.WhenAny(x => x.User, x => x.Password,
                (u, p) => !String.IsNullOrWhiteSpace(u.Value) && !String.IsNullOrWhiteSpace(p.Value));

            Confirm = new ReactiveAsyncCommand(canConfirm);

            var confirmFunc = confirmUserPassMock ?? TestUserNameAndPassword;
            var result = Confirm.RegisterAsyncObservable(_ => confirmFunc());

            MessageBus.Current.RegisterMessageSource(result.Select(res => new Tuple<string, string>(User, Password)), "login");

            Confirm.ThrownExceptions.Subscribe(x => 
                UserError.Throw(new UserError("Couldn't Log In", x.Message, new[] {RecoveryCommand.Ok}, null, x)));
        }

        public IObservable<Unit> TestUserNameAndPassword()
        {
            var client = new RestClient("https://api.github.com");
            client.Authenticator = new HttpBasicAuthenticator(User, Password);

            return client.RequestAsync<GitHubUser>(new RestRequest("user"))
                .Do(res => this.Log().Info("User's URL: {0}", res.Data.url))
                .Select(_ => Unit.Default);
        }
    }
}