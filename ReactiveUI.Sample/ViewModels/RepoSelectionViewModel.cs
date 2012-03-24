using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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
    }

    public class RepoSelectionViewModel : ReactiveObject, IRepoSelectionViewModel
    {
        ObservableAsPropertyHelper<ReactiveCollection<OrganizationTileViewModel>> _Organizations;
        public ReactiveCollection<OrganizationTileViewModel> Organizations {
            get { return _Organizations.Value;  }
        }

        public string UrlPathSegment {
            get { return "repos"; }
        }

        public IScreen HostScreen { get; protected set; }

        [Inject]
        public RepoSelectionViewModel(IScreen hostScreen, IGitHubApi api)
        {
            HostScreen = hostScreen;

            var repos = api.GetReposForUser();

            api.GetOrganizationsForUser()
                .Select(x => createOrgViewModels(x, repos))
                .ToProperty(this, x => x.Organizations);
        }

        ReactiveCollection<OrganizationTileViewModel> createOrgViewModels(IEnumerable<GitHubOrgInfo> gitHubOrgInfos, IObservable<List<GitHubRepo>> repos)
        {
            return new ReactiveCollection<OrganizationTileViewModel>(
                gitHubOrgInfos.Select(x => new OrganizationTileViewModel(x, repos)));
        }
    }
}