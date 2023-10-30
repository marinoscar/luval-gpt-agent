using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Chatbot
{
    public class SecurityProvider
    {
        //Daniel: "5235751730"
        private List<string> _users = new List<string>() { "-5640988132" };

        public bool IsAuthenticated(string userId)
        {
            return _users.Contains(userId);
        }
    }
}
