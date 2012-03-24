using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows.Media;
using Ninject;
using ReactiveUI.Routing;
using ReactiveUI.Sample.Models;

namespace ReactiveUI.Sample.ViewModels
{
    public interface IRepoSelectionViewModel : IRoutableViewModel
    {
        ReactiveCollection<OrganizationTileViewModel> Organizations { get; }
    }

    public class RepoTileViewModel : ReactiveObject
    {
        public GitHubRepo Model { get; set; }
    }

    public class OrganizationTileViewModel : ReactiveObject
    {
        public ImageSource Image { get; set; }
        public string Name { get; set; }

        public ReactiveCollection<RepoTileViewModel> Repositories { get; protected set; }

        public OrganizationTileViewModel(IObservable<List<RepoTileViewModel>> repositories)
        {
            Repositories = new ReactiveCollection<RepoTileViewModel>();

            repositories
                .ObserveOn(RxApp.DeferredScheduler)
                .Subscribe(x => x.ForEach(Repositories.Add));
        }
    }

    public class RepoSelectionViewModel : ReactiveObject, IRepoSelectionViewModel
    {
        public ReactiveCollection<OrganizationTileViewModel> Organizations { get; protected set; }

        public string UrlPathSegment {
            get { return "repos"; }
        }

        public IScreen HostScreen { get; protected set; }

        [Inject]
        public RepoSelectionViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;
            Organizations = new ReactiveCollection<OrganizationTileViewModel>();
        }
    }
}