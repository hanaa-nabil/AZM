using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Notifications
{
    public class FcmOptions
    {
        public const string SectionName = "Fcm";
        public string ProjectId { get; set; } = string.Empty;
        public string ServiceAccountKeyPath { get; set; } = string.Empty;
    }
}
