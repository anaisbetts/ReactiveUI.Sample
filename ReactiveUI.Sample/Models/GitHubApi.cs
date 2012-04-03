using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using Ninject;
using ReactiveUI.Sample.Helpers;
using RestSharp;

namespace ReactiveUI.Sample.Models
{
    public interface IGitHubApi
    {
        IObservable<List<GitHubOrgInfo>> GetOrganizationsForUser();
        IObservable<List<GitHubRepo>> GetReposForUser();
        IObservable<GitHubUser> GetCurrentUser();
    }

    public class GitHubApi : IGitHubApi
    {
        readonly IRestClient client;

        public string UserName { get; set; }

        [Inject]
        public GitHubApi(IRestClient authedClient)
        {
            client = authedClient;
        }

        public IObservable<List<GitHubOrgInfo>> GetOrganizationsForUser()
        {
            var req = new RestRequest("user/orgs");
            return client.RequestAsync<List<GitHubOrgInfo>>(req).Select(x => x.Data);
        }

        public IObservable<List<GitHubRepo>> GetReposForUser()
        {
            var req = new RestRequest("user/repos");
            return client.RequestAsync<List<GitHubRepo>>(req).Select(x => x.Data);
        }

        public IObservable<GitHubUser> GetCurrentUser()
        {
            var req = new RestRequest("user");
            return client.RequestAsync<GitHubUser>(req).Select(x => x.Data);
        }
    }
}