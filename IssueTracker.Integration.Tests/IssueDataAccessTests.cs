using System;
using System.Linq;
using FluentAssertions;
using IssueTracker.Interfaces;
using IssueTracker.Repositories;
using NUnit.Framework;
using ServiceStack.Redis;

namespace IssueTracker.Integration.Tests
{
    [TestFixture]
    public class IssueDataAccessTests
    {
        private IIssueRepository _issueRepository;
        [SetUp]
        public void SetUp()
        {
            _issueRepository = new IssueRepository();
        }

        [Test]
        public void CreateIssue_ShouldBeStoredInRedisDb()
        {
            long issueId = 123;
            new IssueService(_issueRepository).DeleteIssue(issueId);
            var redisIssueId = new IssueService(_issueRepository).Create("Issue title", "", new DateTime(2000, 1, 1), issueId).Id;

            Issue redisIssue = null;
            using (IRedisClient client = new RedisClient())
            {
                redisIssue = client.GetById<Issue>(redisIssueId);
            }

            redisIssue.Title.Should().Be("Issue title");
            redisIssue.Status.Should().Be(IssueStatus.Open);
            redisIssue.DueDate.Should().Be(new DateTime(2000, 1, 1));
            redisIssue.Description.Should().Be("");
        }


        [Test]
        public void DeletedIssue_CannotBeFoundByTitle()
        {
            long issueId = 1234;
            new IssueService(_issueRepository).DeleteIssue(issueId);
            new IssueService(_issueRepository).Create("Issue title 1", "", new DateTime(2000, 1, 1), issueId);
            new IssueService(_issueRepository).DeleteIssue(issueId);

            new IssueTrackerBoard().FindByTitle("Issue title 1").Should().BeEmpty();
        }

        [Test]
        public void IssueCanBeFoundByTitle()
        {
            long issueId = 1235;
            new IssueService(_issueRepository).DeleteIssue(issueId);
            new IssueService(_issueRepository).Create("Issue title 1", "", new DateTime(2000, 1, 1), issueId);

            var issues = new IssueTrackerBoard().FindByTitle("Issue title 1");
            var issue = issues.Single();
            issue.Title.Should().Be("Issue title 1");
            issue.Description.Should().Be("");
            issue.DueDate.Should().Be(new DateTime(2000, 1, 1));
        }

        [Test]
        public void FindIssueByTitle_ShouldBeCaseInsensitive()
        {
            long issueId = 1235;
            new IssueService(_issueRepository).DeleteIssue(issueId);
            new IssueService(_issueRepository).Create("Issue title 1", "", new DateTime(2000, 1, 1), issueId);

            var issues = new IssueTrackerBoard().FindByTitle("issue title 1");
            var issue = issues.Single();
            issue.Title.Should().Be("Issue title 1");
            issue.Description.Should().Be("");
            issue.DueDate.Should().Be(new DateTime(2000, 1, 1));
        }

        [Test]
        public void IssuesCanBeFoundByDate()
        {
            long issueId1 = 1235;
            long issueId2 = 1236;
            new IssueService(_issueRepository).DeleteIssue(issueId1);
            new IssueService(_issueRepository).DeleteIssue(issueId2);
            new IssueService(_issueRepository).Create("Issue title 1", "", new DateTime(2000, 1, 1, 2, 1, 0), issueId1);
            new IssueService(_issueRepository).Create("Issue title 2", "", new DateTime(2000, 1, 2), issueId2);

            var issues = new IssueTrackerBoard().FindByDueDate(new DateTime(2000, 1, 1, 3, 0, 0));
            var issue = issues.Single();
            issue.Title.Should().Be("Issue title 1");
            issue.Description.Should().Be("");
            issue.DueDate.Should().Be(new DateTime(2000, 1, 1, 2, 1, 0));
        }

        [Test]
        public void IssuesCanBeFoundByPartOfTheDescription()
        {
            long issueId1 = 1235;
            long issueId2 = 1236;
            long issueId3 = 1237;
            new IssueService(_issueRepository).DeleteIssue(issueId1);
            new IssueService(_issueRepository).DeleteIssue(issueId2);
            new IssueService(_issueRepository).DeleteIssue(issueId3);
            new IssueService(_issueRepository).Create("Issue title 1", "awesome", new DateTime(2000, 1, 1), issueId1);
            new IssueService(_issueRepository).Create("Issue title 2", "someONe", new DateTime(2000, 1, 1), issueId2);
            new IssueService(_issueRepository).Create("Issue title 3", "none", new DateTime(2000, 1, 2), issueId3);

            var issues = new IssueTrackerBoard().FindByDescription("some");
            issues.Count().Should().Be(2);
            var issue1 = issues.First();
            var issue2 = issues.Last();

            issue1.Title.Should().Be("Issue title 1");
            issue1.Description.Should().Be("awesome");
            issue1.DueDate.Should().Be(new DateTime(2000, 1, 1));

            issue2.Title.Should().Be("Issue title 2");
            issue2.Description.Should().Be("someONe");

            issue2.DueDate.Should().Be(new DateTime(2000, 1, 1));
        }

    }
}