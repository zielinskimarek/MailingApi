using System.Collections.Generic;

namespace MailingApi.DataAccess.Dtos
{
    public class EmailDataInput
    {
        public string Sender { get; set; }

        public IEnumerable<string> Recipients { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
