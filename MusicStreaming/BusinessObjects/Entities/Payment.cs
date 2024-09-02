using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public DateTime PayementDate { get; set; }

    public decimal Price { get; set; }

    public int UserId { get; set; }

    public int SubscriptionId { get; set; }

    public virtual Subscription Subscription { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
