using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IssueTracker.Repositories;
using NUnit.Framework;
using Rhino.Mocks;
using IssueTracker.Interfaces;

namespace IssueTracker.Unit.Tests
{
    [TestFixture]
    public class IssueCreateTests
    {
        private IIssueRepository _issueTrackerRepository;

        [SetUp]
        public void SetUp()
        {
            _issueTrackerRepository = MockRepository.GenerateMock<IIssueRepository>();
        }

        [Test]
        public void CreateIssueReturnsIssue()
        {
            _issueTrackerRepository.Stub(c => c.PersistIssue(null)).IgnoreArguments().Do(new Action<Issue>(c => { }));
            var service = new IssueService(_issueTrackerRepository);
            var actual = service.Create("foo", "bar", DateTime.Today,123);
            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        [TestCase("foo", "bar", 0)]
        [TestCase("baz", "qux", 1)]
        [TestCase("rar", "far", 2)]
        public void CreateIssueWithValuesReturnsIssue(string title, string description, int todaysOffset)
        {
            _issueTrackerRepository.Stub(c => c.PersistIssue(null)).IgnoreArguments().Do(new Action<Issue>(c => { }));
            var service = new IssueService(_issueTrackerRepository);
            var actual = service.Create(title, description, DateTime.Today.AddDays(todaysOffset),123);

            Assert.That(actual.Title, Is.EqualTo(title));
            Assert.That(actual.Description, Is.EqualTo(description));
            Assert.That(actual.DueDate, Is.EqualTo(DateTime.Today.AddDays(todaysOffset)));
            Assert.That(actual.Status, Is.EqualTo(IssueStatus.Open));
        }
    }
}
