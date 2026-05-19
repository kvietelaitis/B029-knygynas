namespace Knygynas.Models;

public enum OrderState
{
    Processing = 0,
    Prepared = 1,
    Shipped = 2,
    Finished = 3,
    Returning = 4,
    Returned = 5,
    Cancelled = 6,
    PendingPayment = 7
}
