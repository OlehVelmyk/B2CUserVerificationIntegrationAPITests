using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class AuditEntryDto
    {
        public Guid UserId { get; set; }
        
        public Guid EntryKey { get; set; }
        
        public EntryType EntryType { get; set; }
        
        public string EventType { get; set; } 

        public DateTime CreatedAt { get; set; }

        public string Data { get; set; }
        
        public InitiationDto Initiation { get; set; }

        public static AuditEntryDto Create(Guid userId,
                                           Guid entryKey,
                                           EntryType entryType,
                                           string eventType,
                                           DateTime createdAt,
                                           string data,
                                           InitiationDto initiation)
        {
            if (eventType == null)
                throw new ArgumentNullException(nameof(eventType));
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (initiation == null)
                throw new ArgumentNullException(nameof(initiation));

            return new()
            {
                UserId = userId,
                EntryKey = entryKey,
                EntryType = entryType,
                EventType = eventType,
                CreatedAt = createdAt,
                Data = data,
                Initiation = initiation,
            };
        }
    }
}