using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCase.Components.Communicators.HelpDeskCommunicator
{
    public class FakeHelpDeskCommunicator : IHelpDeskCommunicator
    {
        public string Login(string domain, string clientId, string secret, string username, string password)
        {
            return "access_token";
        }

        public void Send(string domain, string accessToken, string subject, string name, string email, string description)
        {

        }
    }
}
