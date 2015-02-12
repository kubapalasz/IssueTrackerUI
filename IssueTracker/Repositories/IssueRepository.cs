using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using IssueTracker.Extentions;
using IssueTracker.Interfaces;
using ServiceStack.Redis;

namespace IssueTracker.Repositories
{
    public class IssueRepository : IIssueRepository
    {
        public const string IssueIndexKey = "urn:issue";

        public void PersistIssue(Issue newIssue)
        {
            using (IRedisClient redisClient = new RedisClient())
            {
                var issueClient = redisClient.As<Issue>();
                issueClient.Store(newIssue);

                redisClient.SetEntryInHash(IssueIndexKey, newIssue.Title.ToLower(),
                    newIssue.Id.ToString(CultureInfo.InvariantCulture));

                redisClient.SetEntryInHash(IssueIndexKey, newIssue.DueDate.FormattedDate(),
                    newIssue.Id.ToString(CultureInfo.InvariantCulture));

                if (newIssue.Description != null)
                {
                    redisClient.Set(IssueIndexKey + ":" + newIssue.Id + ":description:" + newIssue.Description.ToLower(), newIssue.Id);
                }
            }
        }

        public IEnumerable<Issue> GetByTitle(string title)
        {
            using (IRedisClient redisClient = new RedisClient())
            {
                var issueIds = redisClient.GetValuesFromHash(IssueIndexKey, title.ToLower());
                if (issueIds!= null)
                    return redisClient.GetByIds<Issue>(issueIds);
            }

            return null;
        }

        public IEnumerable<Issue> GetByDueDate(DateTime dt)
        {
            using (IRedisClient redisClient = new RedisClient())
            {
                var issueIds = redisClient.GetValuesFromHash(IssueIndexKey, dt.FormattedDate());
                if (issueIds != null)
                    return redisClient.GetByIds<Issue>(issueIds);
            }

            return null;
        }

        public IEnumerable<Issue> GetByDescription(string description)
        {
            using (IRedisClient redisClient = new RedisClient())
            {
                var descriptionIndexes = redisClient.SearchKeys(IssueIndexKey + ":*:description:*" + description + "*");
                if (descriptionIndexes != null)
                {
                    var issueIds = redisClient.GetValues(descriptionIndexes);
                    return redisClient.GetByIds<Issue>(issueIds).OrderBy(x=>x.Title);
                }
            }

            return null;
        }
    }
}