using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace easyshifthq.Locations;

public class Location : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string Address { get; private set; }
    public bool IsActive { get; private set; }

    private Location() { } // For EF Core

    public Location(Guid id, string name, string address)
    {
        Id = id;
        Name = name;
        Address = address;
        IsActive = true;
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
    }

    public void Update(string name, string address)
    {
        Name = name;
        Address = address;
    }
}