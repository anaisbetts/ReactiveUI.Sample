using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Ninject;
using Ninject.MockingKernel.Moq;
using ReactiveUI.Sample.Models;
using ReactiveUI.Sample.ViewModels;
using ReactiveUI.Testing;
using RestSharp;
using Xunit;

namespace ReactiveUI.Sample.Tests.ViewModels
{
    public class RepoSelectionViewModelTests : IEnableLogger
    {
        [Fact]
        public void RepoSelectionIntegrationSmokeTest()
        {
            var mock = new MoqMockingKernel();

            var client = new RestClient("https://api.github.com") {
                Authenticator = new HttpBasicAuthenticator("fakepaulbetts", "omg this password is sekrit"),
            };

            mock.Bind<IRestClient>().ToConstant(client);
            mock.Bind<IGitHubApi>().To<GitHubApi>();
            mock.Bind<IRepoSelectionViewModel>().To<RepoSelectionViewModel>();

            (new TestScheduler()).With(sched => {
                var fixture = mock.Get<IRepoSelectionViewModel>();
                ReactiveCollection<OrganizationTileViewModel> repos = null;
                fixture.ObservableForProperty(x => x.Organizations).Subscribe(x => repos = x.Value);

                sched.Start();

                foreach(var v in repos) {
                    v.Model.Should().NotBeNull();
                    this.Log().Info(v.Model.login);

                    v.Repositories.Count.Should().BeGreaterThan(0);
                }

                repos.Count.Should().BeGreaterThan(0);
            });
        }
    }
}
