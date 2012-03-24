using ReactiveUI.Routing;

namespace ReactiveUI.Sample.ViewModels
{
    public interface IRepoViewModel : IRoutableViewModel
    {
    }

    public class RepoViewModel : ReactiveObject, IRepoViewModel
    {
        public string UrlPathSegment {
            get { return "repos"; }
        }

        public IScreen HostScreen { get; protected set; }

        public RepoViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;
        }
    }
}