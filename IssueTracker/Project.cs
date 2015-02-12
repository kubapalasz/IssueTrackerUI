using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace IssueTracker
{
    public class Project
    {
        private readonly ICollection<Issue> _issues = new Collection<Issue>();

        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<Issue> Issues
        {
            get { return _issues; }
        }
    }
}
