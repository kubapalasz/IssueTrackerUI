using System;
using System.Collections.Generic;

namespace IssueTracker
{
    public class Issue
    {
        private readonly List<byte[]> _attachments = new List<byte[]>();

        public String Title { get; set; }
        public String Description { get; set; }
        public DateTime DueDate { get; set; }
        public IssueStatus Status { get; set; }
        public List<byte[]> Attachments {
            get { return _attachments; }
        }

        public long Id { get; set; }

        public override bool Equals(object obj)
        {
            var objAsIssue = (Issue) obj;
            return objAsIssue.Title.Equals(Title) &&
                   objAsIssue.Description.Equals(Description) &&
                   objAsIssue.DueDate.Equals(DueDate) &&
                   objAsIssue.Status.Equals(Status);
        }
    }
}
