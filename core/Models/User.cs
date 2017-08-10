using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace core.Models
{
    public class User : TableEntity
    {
        public User(string lastName, string firstName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }
        
        public User() { }
        
        public int Age { get; set; }
        
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        
        public string Image { get; set; }
        
        public DateTime CreateDateTime { get; set; }
    }
}