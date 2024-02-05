﻿using System;
using WX.B2C.User.Verification.Events.Internal.Dtos;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class CheckCreatedEventArgs : System.EventArgs
    {
        public Guid CheckId { get; set; }
        
        public Guid UserId { get; set; }
        
        public Guid VariantId { get; set; }

        public Guid[] RelatedTasksId { get; set; }

        public InitiationDto Initiation { get; set; }

        public static CheckCreatedEventArgs Create(Guid checkId,
                                                   Guid userId,
                                                   Guid variantId,
                                                   Guid[] relatedTasks,
                                                   InitiationDto initiationDto)
        {
            return new CheckCreatedEventArgs
            {
                CheckId = checkId,
                UserId = userId,
                VariantId = variantId,
                RelatedTasksId = relatedTasks,
                Initiation = initiationDto,
            };
        }
    }
}
