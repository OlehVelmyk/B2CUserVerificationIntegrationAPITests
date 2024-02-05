using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps
{
    internal interface ICollectionStepsUpdater
    {
        Task<Dictionary<string,Guid>> SaveAsync(Guid userId, 
                                                Dictionary<PolicyCollectionStep, NewCollectionStep> collectionSteps, 
                                                CollectionStepEntity[] existingSteps);
    }
}