using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactiveUI.Sample.Models
{
    public class Plan
    {
        public string name { get; set; }
        public int space { get; set; }
        public int collaborators { get; set; }
        public int private_repos { get; set; }
    }

    public class GitHubUser
    {
        public string login { get; set; }
        public int id { get; set; }
        public string avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string company { get; set; }
        public string blog { get; set; }
        public string location { get; set; }
        public string email { get; set; }
        public bool hireable { get; set; }
        public string bio { get; set; }
        public int public_repos { get; set; }
        public int public_gists { get; set; }
        public int followers { get; set; }
        public int following { get; set; }
        public string html_url { get; set; }
        public string created_at { get; set; }
        public string type { get; set; }
        public int total_private_repos { get; set; }
        public int owned_private_repos { get; set; }
        public int private_gists { get; set; }
        public int disk_usage { get; set; }
        public int collaborators { get; set; }
        public Plan plan { get; set; }
    }
}