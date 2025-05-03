using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace easyshifthq.Locations;

public class Location : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    private string _name = null!;
    public string Name
    {
        get => _name;
        private set => _name = Check.NotNullOrWhiteSpace(value, nameof(Name), maxLength: LocationConsts.MaxNameLength);
    }

    private string _address = null!;
    public string Address
    {
        get => _address;
        private set => _address = Check.NotNullOrWhiteSpace(value, nameof(Address));
    }

    public bool IsActive { get; private set; }

    private string _timeZone = null!;
    public string TimeZone
    {
        get => _timeZone;
        private set => _timeZone = Check.NotNullOrWhiteSpace(value, nameof(TimeZone));
    }

    public string? JurisdictionCode { get; private set; }
    public string? Notes { get; private set; }

    private Location() { } // For EF Core

    public Location(
        Guid id,
        string name,
        string address,
        string timeZone,
        string? jurisdictionCode = null,
        string? notes = null)
        : base(id)
    {
        Name = name;
        Address = address;
        TimeZone = timeZone;
        SetJurisdictionCode(jurisdictionCode);
        SetNotes(notes);
        IsActive = true;
    }

    private void SetJurisdictionCode(string? jurisdictionCode)
    {
        if (jurisdictionCode != null)
        {
            Check.Length(
                jurisdictionCode,
                nameof(jurisdictionCode),
                maxLength: LocationConsts.MaxJurisdictionCodeLength
            );
        }
        JurisdictionCode = jurisdictionCode;
    }

    private void SetNotes(string? notes)
    {
        if (notes != null)
        {
            Check.Length(
                notes,
                nameof(notes),
                maxLength: LocationConsts.MaxNotesLength
            );
        }
        Notes = notes;
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
    }

    public void Update(
        string name,
        string address,
        string timeZone,
        string? jurisdictionCode,
        string? notes)
    {
        Name = name;
        Address = address;
        TimeZone = timeZone;
        SetJurisdictionCode(jurisdictionCode);
        SetNotes(notes);
    }
}