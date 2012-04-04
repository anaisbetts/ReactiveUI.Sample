using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Media;
using Ninject;
using ReactiveUI.Routing;
using ReactiveUI.Sample.Models;

namespace ReactiveUI.Sample.ViewModels
{
    public class RepoTileViewModel : ReactiveObject
    {
        public GitHubRepo Model { get; set; }
    }

    public class OrganizationTileViewModel : ReactiveObject
    {
        public GitHubOrgInfo Model { get; set; }
        public ImageSource Image { get; set; }
        public string Name { get; set; }

        public ReactiveCollection<RepoTileViewModel> Repositories { get; protected set; }

        public OrganizationTileViewModel(GitHubOrgInfo model, IObservable<List<GitHubRepo>> repositories)
        {
            Model = model;
            Repositories = new ReactiveCollection<RepoTileViewModel>();

            repositories
                .SelectMany(x => x.ToObservable())
                .ObserveOn(RxApp.DeferredScheduler)
                .Where(x => model.login == x.owner.login)
                .Subscribe(x => Repositories.Add(new RepoTileViewModel() { Model = x }));
        }
    }

    public interface IRepoSelectionViewModel : IRoutableViewModel
    {
        ReactiveCollection<OrganizationTileViewModel> Organizations { get; }
        IObservable<Unit> FinishedLoading { get; }
    }

    public class RepoSelectionViewModel : ReactiveObject, IRepoSelectionViewModel
    {
        public string UrlPathSegment {
            get { return "repos"; }
        }

        public IScreen HostScreen { get; protected set; }

        public IObservable<Unit> FinishedLoading { get; protected set; }

        public ReactiveCollection<OrganizationTileViewModel> Organizations { get; protected set; }

        [Inject]
        public RepoSelectionViewModel(IScreen hostScreen, IGitHubApi api)
        {
            HostScreen = hostScreen;

            var orgs = api.GetOrganizationsForUser();
            var userInfo = getUserAsOrgInfo(api);

            var repoTable = orgs.SelectMany(x => x.ToObservable())
                .Select(x => new { Key = x, Value = api.GetReposFromOrganization(x) })
                .Concat(userInfo.Select(x => new { Key = x, Value = api.GetReposForUser() }));

            Organizations = repoTable.Select(x => new OrganizationTileViewModel(x.Key, x.Value)).CreateCollection();

            FinishedLoading = repoTable.Aggregate(Unit.Default, (acc, x) => Unit.Default);
        }

        ReactiveCollection<OrganizationTileViewModel> createOrgViewModels(IEnumerable<GitHubOrgInfo> gitHubOrgInfos, IObservable<List<GitHubRepo>> repos)
        {
            return new ReactiveCollection<OrganizationTileViewModel>(
                gitHubOrgInfos.Select(x => new OrganizationTileViewModel(x, repos)));
        }

        IObservable<GitHubOrgInfo> getUserAsOrgInfo(IGitHubApi api)
        {
            return api.GetCurrentUser()
                .Select( x => new GitHubOrgInfo() {login = x.login, avatar_url = x.avatar_url, url = x.url});
        }
    }
}