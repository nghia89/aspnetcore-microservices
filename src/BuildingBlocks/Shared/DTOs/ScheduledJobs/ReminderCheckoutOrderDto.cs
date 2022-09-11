using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.ScheduledJobs
{
    public record ReminderCheckoutOrderDto(string email,
      string subject, string emailContent,
      DateTimeOffset enqueueAt);
}
