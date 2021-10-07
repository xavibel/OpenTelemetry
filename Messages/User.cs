using System;
using System.ComponentModel.DataAnnotations;

namespace Messages
{
    public class User
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public string MailAddress { get; set; }
        
    }
}
