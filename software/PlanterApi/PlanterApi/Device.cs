using System.Diagnostics.CodeAnalysis;

namespace PlanterApi;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class Device
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastReceivedTimestamp { get; set; }
}