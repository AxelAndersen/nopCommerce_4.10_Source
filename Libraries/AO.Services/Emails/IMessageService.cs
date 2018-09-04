using Nop.Services.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace AO.Services.Emails
{
    public interface IMessageService : IWorkflowMessageService
    {
        IList<int> SendAdminEmail(string email, string subject, string body);
    }
}
