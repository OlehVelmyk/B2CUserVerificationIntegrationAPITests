using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Events.Internal.Events;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class ReminderFixture
    {
        private readonly AdminApiClientFactory _adminApiClientFactory;
        private readonly AdministratorFactory _administratorFactory;
        private readonly EventsFixture _eventsFixture;
        private readonly DbFixture _dbFixture;

        public ReminderFixture(AdminApiClientFactory adminApiClientFactory, 
                               AdministratorFactory administratorFactory, 
                               EventsFixture eventsFixture,
                               DbFixture dbFixture)
        {
            _adminApiClientFactory = adminApiClientFactory ?? throw new ArgumentNullException(nameof(adminApiClientFactory));
            _administratorFactory = administratorFactory ?? throw new ArgumentNullException(nameof(administratorFactory));
            _eventsFixture = eventsFixture ?? throw new ArgumentNullException(nameof(eventsFixture));
            _dbFixture = dbFixture ?? throw new ArgumentNullException(nameof(dbFixture));
        }

        public async Task FireReminderJob(ActiveReminderDto activeReminder)
        {
            var request = new ScheduleJobRequest
            {
                JobName = Constants.Jobs.UserReminderJob,
                JobParameters = new Dictionary<string, object>
                {
                    [JobParameters.UserId] = activeReminder.UserId,
                    [JobParameters.ReminderId] = activeReminder.TargetId
                }
            };

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var correlationId = Guid.NewGuid();
            client.CorrelationId = correlationId;
            await client.Jobs.ScheduleAsync(request);

            _eventsFixture.ShouldExistSingle<UserReminderJobFinishedEvent>(correlationId);

            Func<Task<bool>> checkReminderInDB = () => _dbFixture.ReminderExists(activeReminder.UserId, activeReminder.TargetId);
            await checkReminderInDB.WaitUntil(exists => exists);
        }

        public async Task<ActiveReminderDto> GetActiveReminderAsync(Guid userId)
        {
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            
            Func<Task<ActiveReminderDto>> readReminder = async () => (await client.Reminders.GetActiveRemindersAsync(userId)).FirstOrDefault();
            var reminder = await readReminder.WaitUntil(reminders => reminders is not null);

            return reminder;
        }
        
        public async Task<ReminderDto> GetLastSentReminderAsync(Guid userId)
        {
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            
            Func<Task<ReminderDto>> readReminder = async () => 
                (await client.Reminders.GetSentRemindersAsync(userId)).
                OrderByDescending(dto => dto.SentAt).
                FirstOrDefault();
            
            var reminder = await readReminder.WaitUntil(reminders => reminders is not null);

            return reminder;
        }
    }
}