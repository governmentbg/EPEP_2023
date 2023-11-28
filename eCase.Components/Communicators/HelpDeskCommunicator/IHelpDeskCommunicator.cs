using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCase.Components.Communicators.HelpDeskCommunicator
{
    public interface IHelpDeskCommunicator
    {
        string Login(string domain, string clientId, string secret, string username, string password);
        void Send(string domain, string accessToken, string subject, string name, string email, string description);
    }
}
