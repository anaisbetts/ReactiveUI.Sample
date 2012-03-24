using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using Ninject;
using RestSharp;

namespace ReactiveUI.Sample.Models
{
    public interface IGitHubApi
    {
        IObservable<List<GitHubOrgInfo>> GetOrganizationsForUser();
        IObservable<List<GitHubRepo>> GetReposForUser();
    }

    public class GitHubApi : IGitHubApi
    {
        [Inject]
        public GitHubApi(IRestClient authedClient)
        {
        }

        public IObservable<List<GitHubOrgInfo>> GetOrganizationsForUser()
        {
            throw new NotImplementedException();
        }

        public IObservable<List<GitHubRepo>> GetReposForUser()
        {
            throw new NotImplementedException();
        }
    }
}