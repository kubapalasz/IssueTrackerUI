using System;
using System.Collections.Generic;
using IssueTracker.Interfaces;
using IssueTracker.Repositories;


namespace IssueTracker
{
    public class IssueTrackerBoard
    {

        private readonly IIssueRepository _issueRepository;

        public IssueTrackerBoard(IIssueRepository issueRepository = null)
        {
            _issueRepository = issueRepository ?? new IssueRepository();
        }

        public IEnumerable<Issue> FindByTitle(string title)
        {
            return _issueRepository.GetByTitle(title);
        }

        public IEnumerable<Issue> FindByDueDate(DateTime dateTime)
        {
            return _issueRepository.GetByDueDate(dateTime);
        }

        public IEnumerable<Issue> FindByDescription(string description)
        {
            return _issueRepository.GetByDescription(description);
        }
    }
}