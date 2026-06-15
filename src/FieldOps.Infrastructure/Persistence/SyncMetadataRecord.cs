using SQLite;

namespace FieldOps.Infrastructure.Persistence;

[Table("SyncMetadata")]
public sealed class SyncMetadataRecord
{
    [PrimaryKey]
    public int Id { get; set; } = 1;

    public DateTimeOffset? LastSyncAt { get; set; }
}
