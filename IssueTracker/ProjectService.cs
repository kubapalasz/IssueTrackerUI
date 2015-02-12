using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IssueTracker.Interfaces;
using IssueTracker.Repositories;

namespace IssueTracker
{
    public class ProjectService
    {
        private IProjectRepository _projectRepository;
        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public Project Create(string title, string description)
        {
            var newProject = new Project()
            {
                Title = title,
                Description = description
            };
            _projectRepository.Add(newProject);
            return newProject;
        }

        public void AddIssue(Project project, Issue issue)
        {
            project.Issues.Add(issue);
        }
    }
}
