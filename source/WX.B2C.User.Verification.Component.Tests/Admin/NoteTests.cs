using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Admin
{
    internal class NoteTests : BaseComponentTest
    {
        private AdminApiClientFactory _adminApiClientFactory;
        private AdministratorFactory _administratorFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _administratorFactory = Resolve<AdministratorFactory>();

            Arb.Register<CreateNoteRequestArbitrary>();
            Arb.Register<InvalidNoteRequestArbitrary>();
        }

        /// <summary>
        /// Scenario: Admin leaves a note on a subject
        /// Given subject without notes
        /// When an admin leaves new note on a subject
        /// Then note is successfully created
        /// And note with id is returned in response
        /// </summary>
        [Theory]
        public async Task ShouldCreateNote(CreateNoteRequest noteRequest)
        {
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            // Act
            var createdNote = await client.Notes.CreateAsync(noteRequest);

            // Assert
            createdNote.Id.Should().NotBeEmpty();
            createdNote.Should().BeEquivalentTo(new
            {
                AuthorEmail = admin.Username,
                noteRequest.Text,
                noteRequest.SubjectId,
                noteRequest.Subject
            });
        }

        /// <summary>
        /// Scenario: Admin fails to create note with invalid text
        /// Given subject without notes
        /// When admin leaves note with too long (2000+ digits) or empty text
        /// Then he should receive error response with status code "Bad Request"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenCreateNoteWithInvalidText(InvalidNoteRequest noteRequest)
        {
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            // Act
            Func<Task> action = () => client.Notes.CreateAsync(noteRequest);

            // Assert
            var exception = await action.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Scenario: Admin fails to create note with for invalid subject
        /// When admin tries to leave a note for invalid subject type
        /// Then he should receive error response with status code "Bad Request"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenCreateNoteWithInvalidSubject(CreateNoteRequest noteRequest)
        {
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            // Arrange
            noteRequest.Subject = (NoteSubject)100;

            // Act
            Func<Task> action = () => client.Notes.CreateAsync(noteRequest);

            // Assert
            var exception = await action.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Scenario: Admin requests notes by subject and subjectId
        /// Given subject with several notes
        /// When admin requests notes by subject and subjectId
        /// Then he receives all notes related to requested subject
        /// And all fields are present
        /// </summary>
        [Theory]
        public async Task ShouldGetNotes(NoteSubject subject, Guid subjectId, NonEmptyArray<CreateNoteRequest> notes)
        {
            var requests = notes.Item.Select(CreateRequest).ToArray();

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            // Given
            var createdNotes = new List<NoteDto>();
            foreach (var request in requests)
            {
                var createdNote = await client.Notes.CreateAsync(request);
                createdNotes.Add(createdNote);
            }

            // Act
            var response = await client.Notes.GetAsync(subject, subjectId);

            // Assert
            response.Count.Should().Be(notes.Item.Length);
            response.Should().BeEquivalentTo(createdNotes);

            CreateNoteRequest CreateRequest(CreateNoteRequest request) => new(request.Text, subject, subjectId);
        }

        /// <summary>
        /// Scenario: Admin requests notes by subject and subjectId (Empty)
        /// Given subject without notes
        /// When admin requests notes by subject and subjectId
        /// Then he should get empty response
        /// </summary>
        [Theory]
        public async Task ShouldGetEmptyResponse_WhenNoNotesWithSubjectFromRequest(NoteSubject subject, Guid subjectId)
        {
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            // Act
            var response = await client.Notes.GetAsync(subject, subjectId);

            // Assert
            response.Should().BeEmpty();
        }

        /// <summary>
        /// Scenario: Admin removes note by id
        /// Given subject with several notes
        /// When admin removes one of notes by id
        /// Then note with specified id are removed from subject
        /// And subject contains only remaining notes
        /// </summary>
        [Theory]
        public async Task ShouldRemoveNote(NoteSubject subject,
                                           Guid subjectId,
                                           NonEmptyArray<CreateNoteRequest> notes,
                                           Seed seed)
        {
            var requests = notes.Item.Select(CreateRequest).ToArray();

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            // Given
            var createdNotes = new List<NoteDto>();
            foreach (var note in requests)
            {
                var createdNote = await client.Notes.CreateAsync(note);
                createdNotes.Add(createdNote);
            }

            // Arrange
            var faker = FakerFactory.Create(seed);
            var noteToRemove = faker.PickRandom(createdNotes);

            // Act
            await client.Notes.DeleteAsync(noteToRemove.Id);

            // Assert
            var response = await client.Notes.GetAsync(subject, subjectId);

            createdNotes.Remove(noteToRemove);
            response.Count.Should().Be(createdNotes.Count);
            response.Should().BeEquivalentTo(createdNotes);

            CreateNoteRequest CreateRequest(CreateNoteRequest request) => new(request.Text, subject, subjectId);
        }

        /// <summary>
        /// Scenario: Admin try to remove not existing note
        /// Given subject without notes
        /// When admin tries to remove note by id
        /// Then he should receive error response with status code "Not Found"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenRemovingNoteNotExist(Guid noteId)
        {
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            // Act
            Func<Task> action = () => client.Notes.DeleteAsync(noteId);

            // Assert
            var exception = await action.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
