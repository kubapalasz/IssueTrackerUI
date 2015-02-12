using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Interfaces
{
    public interface IIssueRepository
    {
        void PersistIssue(Issue newIssue);

        IEnumerable<Issue> GetByTitle(string title);

        IEnumerable<Issue> GetByDueDate(DateTime dt);

        IEnumerable<Issue> GetByDescription(string description);

    }
}
