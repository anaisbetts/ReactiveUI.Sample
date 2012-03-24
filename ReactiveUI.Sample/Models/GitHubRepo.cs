using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactiveUI.Sample.Models
{
    public class GitHubRepoInfo
    {
        public string login { get; set; }
        public int id { get; set; }
        public string url { get; set; }
        public string avatar_url { get; set; }
    }

    public class GitHubRepo
    {
        public string url { get; set; }
        public string html_url { get; set; }
        public string clone_url { get; set; }
        public string git_url { get; set; }
        public string ssh_url { get; set; }
        public string svn_url { get; set; }
        public string mirror_url { get; set; }
        public int id { get; set; }
        public GitHubUser owner { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string homepage { get; set; }
        public object language { get; set; }
        public bool @private { get; set; }
        public bool fork { get; set; }
        public int forks { get; set; }
        public int watchers { get; set; }
        public int size { get; set; }
        public string master_branch { get; set; }
        public int open_issues { get; set; }
        public string pushed_at { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }
}