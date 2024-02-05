﻿using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.DataAccess.Entities.Audit
{
    internal class AuditEntry
    {
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }
        
        public Guid EntryKey { get; set; }
        
        public EntryType EntryType { get; set; }
        
        public string EventType { get; set; } 

        public DateTime CreatedAt { get; set; }
        
        public string Data { get; set; }
        
        public string Initiator { get; set; }
        
        public string Reason { get; set; }
    }
}