using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class Subscription
{
    public int SubscriptionId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int DurationInDays { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
