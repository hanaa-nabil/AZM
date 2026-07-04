using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.DTOs.Notification
{
    public record NotificationDto(
      Guid Id,
      string Type,
      string Title,
      string Body,
      Guid? RelatedEventId,
      bool IsRead,
      DateTime CreatedAt
  );
}
