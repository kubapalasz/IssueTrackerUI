using System;
using IssueTracker.Extentions;
using IssueTracker.Interfaces;
using IssueTracker.Repositories;
using ServiceStack.Caching;
using ServiceStack.Redis;

namespace IssueTracker
{
    public class IssueService
    {
        private IIssueRepository _issueRepository;

        public IssueService(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository;
        }

        public Issue Create(string title, string description, DateTime dueDate, long issueId)
        {
            var newIssue = new Issue()
            {
                Title = title, 
                Description = description, 
                DueDate = dueDate,
                Status = IssueStatus.Open,
                Id = issueId
            };


            _issueRepository.PersistIssue(newIssue);

            return newIssue;
        }

        public Issue GetIssueById(long issueId)
        {
            using (IRedisClient redisClient = new RedisClient())
            {
                return redisClient.GetById<Issue>(issueId);
            }
        }

        public void DeleteIssue(long issueId)
        {
            using (IRedisClient client = new RedisClient())
            {
                var issue = GetIssueById(issueId);
                if (issue != null)
                {
                    client.DeleteById<Issue>(issue.Id);
                    client.RemoveEntryFromHash(IssueRepository.IssueIndexKey, issue.Title.ToLower());
                    client.RemoveEntryFromHash(IssueRepository.IssueIndexKey, issue.DueDate.FormattedDate());
                    ((IRemoveByPattern) client).RemoveByPattern(IssueRepository.IssueIndexKey + ":" + issue.Id + ":description:*"); 
                }
            }
        }
    }
}