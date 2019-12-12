using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Store.ViewModels
{
    public class EditCustomerView
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public List<IdentityRole> AllRoles { get; set; }

        public IList<string> UserRoles { get; set; }
    
        public EditCustomerView()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}
