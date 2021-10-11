using System;
using System.ComponentModel.DataAnnotations;

namespace WebPage.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public string MailAddress { get; set; }
    }
}
