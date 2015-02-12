using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IssueTracker.Interfaces;
using IssueTracker.Repositories;
using NUnit.Framework;
using Rhino.Mocks;

namespace IssueTracker.Unit.Tests
{
    [TestFixture]
    public class ProjectIssueMappingTests
    {
        private IProjectRepository _issueProjectRepository;
        private IIssueRepository _issueRepository;

        [SetUp]
        public void SetUp()
        {
            _issueProjectRepository = MockRepository.GenerateMock<IProjectRepository>();
            _issueRepository = MockRepository.GenerateMock<IIssueRepository>();
        }

        [Test]
        public void AddIssueToProjectReturnsSuccess()
        {
            // Arrange
            _issueRepository.Stub(c => c.PersistIssue(null)).IgnoreArguments().Do(new Action<Issue>(c => { })); // is this needed?
            var issueService = new IssueService(_issueRepository);
            var actual = issueService.Create("foo", "bar", DateTime.Today.AddDays(0),123);

            _issueProjectRepository.Stub(c => c.Add(null)).IgnoreArguments().Do(new Action<Project>(c => { })); // is this needed?
            var projectService = new ProjectService(_issueProjectRepository);
            var project = projectService.Create("foo", "bar");

            projectService.AddIssue(project, actual);
        }

    }
}
