using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Extensions
{
    internal static class AuditableEntityExtensions
    {
        internal static void SetAuditables(this ChangeTracker changeTracker)
        {
            if (changeTracker == null)
                throw new ArgumentNullException(nameof(changeTracker));

            var entries = changeTracker.Entries();  
            var modifiedEntries = entries.Where(IsModifiedOrAdded);

            var utcNow = DateTime.UtcNow;
            foreach (var entry in modifiedEntries)
            {
                if (entry.Entity is AuditableEntity auditableEntity)
                {
                    _ = entry.State switch
                    {
                        EntityState.Added => ModifyCreatedAt(auditableEntity, utcNow),
                        EntityState.Modified => ModifyUpdatedAt(auditableEntity, utcNow),
                        _ => auditableEntity
                    };
                }
            }

            static bool IsModifiedOrAdded(EntityEntry entry) =>
                entry.State is EntityState.Added or EntityState.Modified;
        }

        private static AuditableEntity ModifyCreatedAt(AuditableEntity entity, DateTime createdAt)
        {
            if (entity.CreatedAt == default)
                entity.CreatedAt = createdAt;

            return entity;
        }

        private static AuditableEntity ModifyUpdatedAt(AuditableEntity entity, DateTime updatedAt)
        {
            entity.UpdatedAt = updatedAt;
            return entity;
        }
    }
}
