using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
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
        string ErrorMessage { get; }
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

        ObservableAsPropertyHelper<string> _ErrorMessage;
        public string ErrorMessage {
            get { return _ErrorMessage.Value; }
        }

        public string UrlPathSegment
        {
            get { return "login"; }
        }

        public IScreen HostScreen { get; protected set; }

        public ReactiveAsyncCommand Confirm { get; protected set; }

        public LoginViewModel(ReactiveAsyncCommand confirm = null)
        {
            var canConfirm = this.WhenAny(x => x.User, x => x.Password,
                (u, p) => !String.IsNullOrWhiteSpace(u.Value) && !String.IsNullOrWhiteSpace(p.Value));

            Confirm = confirm ?? new ReactiveAsyncCommand(canConfirm);

            var result = Confirm.RegisterAsyncObservable(_ => {
                var client = new RestClient("https://api.github.com");
                client.Authenticator = new HttpBasicAuthenticator(User, Password);

                return client.RequestAsync<GitHubUser>(new RestRequest("user"))
                    .Do(res => this.Log().Info("User's URL: {0}", res.Data.url))
                    .Select(res => new Tuple<string, string>(User, Password));
            });

            MessageBus.Current.RegisterMessageSource(result, "login");

            Confirm.ThrownExceptions
                .Select(x => x.Message)
                .ToProperty(this, x => x.ErrorMessage);
        }
    }
}