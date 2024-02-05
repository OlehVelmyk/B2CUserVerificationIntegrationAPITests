using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Commands;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Commands;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Ticket;
using WX.B2C.User.Verification.Core.Contracts.Triggers;
using WX.B2C.User.Verification.Core.Contracts.Triggers.Configs;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services.Monitoring
{
    internal class TriggerCommandRunner : ITriggerCommandRunner
    {
        private readonly ICommandService _commandService;
        private readonly ITicketService _ticketService;
        private readonly IBatchCommandPublisher _commandPublisher;

        public TriggerCommandRunner(ICommandService commandService, ITicketService ticketService, IBatchCommandPublisher commandPublisher)
        {
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _commandPublisher = commandPublisher ?? throw new ArgumentNullException(nameof(commandPublisher));
        }

        public async Task RunAsync(Guid userId,
                                   Guid applicationId,
                                   Guid triggerId,
                                   CommandConfig[] commands,
                                   InitiationDto initiation)
        {
            var addCollectionStepsConfigs = commands.OfType<AddCollectionStepCommandConfig>();
            var instructCheckCommandConfigs = commands.OfType<InstructCheckCommandConfig>();
            var addTasksConfig = commands.OfType<AddTaskCommandConfig>();

            //TODO WRXB-9854 It seems that we have two types of commands : independent and dependent. 
            //TODO later code can be improved to avoid reading all configs in such way: the idea is to:
            //TODO reading commands configs in any order but put them in special queue builder which
            //TODO will order commands and add new not defined like AddCollectionStepsTask

            var sendTicketsConfig = commands.OfType<SendTickedCommandConfig>();
            await SendTicketsCommandsAsync(userId, sendTicketsConfig);

            var createdSteps = await RunCollectionStepsCommandsAsync(userId, addCollectionStepsConfigs, initiation);
            await RunTasksCommandsAsync(userId, applicationId, addTasksConfig, initiation);
            await AddCollectionStepsToTasksAsync(applicationId, createdSteps, initiation);
            await InstructChecksAsync(userId, applicationId, instructCheckCommandConfigs, initiation);

            await _commandPublisher.PublishAsync(new CompleteTriggerCommand(triggerId));
        }

        private Task AddCollectionStepsToTasksAsync(Guid applicationId,
                                                    Dictionary<TaskType, Guid[]> createdSteps,
                                                    InitiationDto initiation) =>
            createdSteps.Foreach(createdStep =>
            {
                var command = new AddCollectionStepsToTaskCommand
                {
                    ApplicationId = applicationId,
                    TaskType = createdStep.Key,
                    CollectionSteps = createdStep.Value.ToArray()
                };
                return _commandService.RunAsync(command, initiation);
            });

        private Task RunTasksCommandsAsync(Guid userId,
                                           Guid applicationId,
                                           IEnumerable<AddTaskCommandConfig> addTasksConfig,
                                           InitiationDto initiation) =>
            addTasksConfig.Foreach(config =>
            {
                var command = new AddTaskCommand
                {
                    UserId = userId,
                    ApplicationId = applicationId,
                    VariantId = config.VariantId,
                    AddCompleted = config.AddCompleted
                };
                return _commandService.RunAsync(command, initiation);
            });

        private async Task<Dictionary<TaskType, Guid[]>> RunCollectionStepsCommandsAsync(Guid userId,
                                                                                         IEnumerable<AddCollectionStepCommandConfig>
                                                                                             configs,
                                                                                         InitiationDto initiation)
        {
            var createdSteps = await configs.Foreach(async config =>
            {
                var command = new AddCollectionStepCommand
                {
                    UserId = userId,
                    CollectionStep = config.CollectionStep
                };
                var id = await _commandService.RunAsync(command, initiation);

                return (config.TaskType, id);
            });
            return createdSteps.GroupBy(tuple => tuple.TaskType, tuple => tuple.id)
                               .ToDictionary(grouping => grouping.Key, grouping => grouping.ToArray());
        }
        
        private Task InstructChecksAsync(Guid userId,
                                         Guid applicationId,
                                         IEnumerable<InstructCheckCommandConfig> configs,
                                         InitiationDto initiation) =>
            configs.Foreach(config =>
            {
                var command = new InstructCheckCommand
                {
                    UserId = userId,
                    ApplicationId = applicationId,
                    VariantId = config.VariantId,
                    Force = config.Force,
                    TaskType = config.TaskType
                };
                return _commandService.RunAsync(command, initiation);
            });

        private Task SendTicketsCommandsAsync(Guid userId, IEnumerable<SendTickedCommandConfig> configs) =>
            configs.Foreach(config => _ticketService.SendAsync(new TicketContext
            {
                Reason = config.Reason,
                UserId = userId
            }));
    }
}