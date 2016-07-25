using Lending.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lending.Controllers
{
    public class UserController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (User != null)
            {
                var context = new ApplicationDbContext();
                var username = User.Identity.Name;

                if (!string.IsNullOrEmpty(username))
                {
                    var user = context.Users.SingleOrDefault(u => u.UserName == username);
                    string email = string.Concat(new string[] { user.Email });
                    string firstname = string.Concat(new string[] { user.FirstName });
                    string lastname = string.Concat(new string[] { user.LastName });
                    
                    // View Data
                    ViewData.Add("Email", email);
                    ViewData.Add("FullName", firstname + " " + lastname);
                }
            }
            base.OnActionExecuted(filterContext);
        }
        public UserController()
        { }
    }
}