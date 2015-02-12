using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IssueTracker.Interfaces;
using IssueTracker.Repositories;
using NUnit.Framework;
using Rhino.Mocks;
using IssueTracker;

namespace IssueTracker.Unit.Tests
{
    [TestFixture]
    public class ProjectCreateTests
    {
        private IProjectRepository _issueProjectRepository;

        [SetUp]
        public void SetUp()
        {
            _issueProjectRepository = MockRepository.GenerateMock<IProjectRepository>();
        }


        [Test]
        public void CreateProjectReturnsProject()
        {
            _issueProjectRepository.Stub(c => c.Add(null)).IgnoreArguments().Do(new Action<Project>(c => { }));
            var service = new ProjectService(_issueProjectRepository);
            var actual = service.Create("foo", "bar");
            Assert.That(actual, Is.Not.Null);
            
        }

        [Test]
        [TestCase("foo", "bar")]
        [TestCase("baz", "qux")]
        [TestCase("gar", "fas")]
        public void CreateProjectWithValuesReturnsIssue(string title, string description)
        {
            _issueProjectRepository.Stub(c => c.Add(null)).IgnoreArguments().Do(new Action<Project>(c => { }));
            var service = new ProjectService(_issueProjectRepository);
            var actual = service.Create(title, description);

            Assert.That(actual.Title, Is.EqualTo(title));
            Assert.That(actual.Description, Is.EqualTo(description));
        }

    }
}
