using System.Collections.Concurrent;
using FireTracker.Core.DTOs;

namespace FireTracker.Core.Persistence;

// We mock the db for now, if we have a real db, we can set up the real db context here
public static class DbContext
{
    public static readonly ConcurrentDictionary<Guid, IncidentModel> IncidentCollection = new();
}