using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IssueTracker.Interfaces;
using IssueTracker.Repositories;
using NUnit.Framework;
using Rhino.Mocks;

namespace IssueTracker.Unit.Tests
{
    [TestFixture]
    public class IssueAttachmentTests 
    {
        private IIssueRepository _issueRepository;

        [SetUp]
        public void SetUp()
        {
            _issueRepository = MockRepository.GenerateMock<IIssueRepository>();
        }

        
        [Test]
        public void CanReadEmbeddedResourceWithSuccess()
        {
            // Arrange
            var assembly = Assembly.GetExecutingAssembly();
            var resource = "IssueTracker.Unit.Tests.Resources.Attachment1.txt";
            string actual = string.Empty;
            // Act
            using (var stream = assembly.GetManifestResourceStream(resource))
            using (var reader = new StreamReader(stream))
            {
                actual  = reader.ReadToEnd();
            }
            // Assert
            Assert.That(actual, Is.Not.Empty);
        }

        [Test]
        [TestCase("IssueTracker.Unit.Tests.Resources.Attachment1.txt", "THIS IS A FILE")]
        [TestCase("IssueTracker.Unit.Tests.Resources.Attachment2.txt", "THIS IS ANOTHER FILE")]
        public void EmbeddedResourceContainsCorrectText(string resource, string expected)
        {
            // Arrange
            var assembly = Assembly.GetExecutingAssembly();
            string actual = string.Empty;
            // Act
            using (var stream = assembly.GetManifestResourceStream(resource))
            using (var reader = new StreamReader(stream))
            {
                actual = reader.ReadToEnd();
            }
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void AddAttachmentToIssueWithSuccess()
        {
            // Arrange
            var resource = "IssueTracker.Unit.Tests.Resources.Attachment1.txt";
            byte[] actual = null;
            
            // Act
            _issueRepository.Stub(c => c.PersistIssue(null)).IgnoreArguments().Do(new Action<Issue>(c => { }));
            var service = new IssueService(_issueRepository);//_issueTrackerRepository);
            var issue = service.Create("foo", "bar", DateTime.Today,123);

            actual = ReadEmbeddedResource(resource);

            issue.Attachments.Add(actual);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(issue.Attachments.First(), Is.EqualTo(actual));

        }
        [Test]
        public void AddMultipleAttachmentsToIssueWithSuccess()
        {
            // Arrange
            IList<byte[]> expected = new List<byte[]>();

            // Act
            _issueRepository.Stub(c => c.PersistIssue(null)).IgnoreArguments().Do(new Action<Issue>(c => { }));
            var service = new IssueService(_issueRepository);
            var issue = service.Create("foo", "bar", DateTime.Today,123);

            expected.Add(ReadEmbeddedResource("IssueTracker.Unit.Tests.Resources.Attachment1.txt"));
            expected.Add(ReadEmbeddedResource("IssueTracker.Unit.Tests.Resources.Attachment2.txt"));

            issue.Attachments.AddRange(expected);
            var actual = issue.Attachments;

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(issue.Attachments.Count(), Is.EqualTo(expected.Count));

        }

        private static byte[] ReadEmbeddedResource(string resource)
        {
            byte[] fileData;
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resource))
            {
                if (stream == null)
                    throw new ArgumentNullException("resource",
                        string.Format("{0} is an empty embedded resource", resource));

                fileData = new byte[stream.Length];
                stream.Read(fileData, 0, fileData.Length);
            }
            return fileData;
        }
    }
}
