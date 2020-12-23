using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using IM.SQL;
using IM.Models;
using System.Security.Claims;
using System.Web;

namespace IM.ApiControllers
{
    public class UsersController : ApiController
    {
        //[Authorize]
        [HttpGet]
        public User UserDetail()
        {
            var claims = Request.GetOwinContext().Authentication.User.Claims;
            string userId = "";
            foreach (var claim in claims)
            {
                if (claim.Type == ClaimTypes.Name)
                {
                    var name = claim.Value;
                }
                if (claim.Type == "Id")
                {
                    userId = claim.Value;
                }
            }
            return UsersMethods.GetUserById(userId);
        }

        [HttpGet]
        public IHttpActionResult AllUsers()
        {
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, UsersMethods.GetAllUsers()));
        }

        [HttpPost]
        public IHttpActionResult AddUser()
        {      
            User user = new User();
            user.FirstName = HttpContext.Current.Request["FirstName"];
            user.LastName = HttpContext.Current.Request["LastName"];
            user.Email = HttpContext.Current.Request["Email"];
            user.Phone = HttpContext.Current.Request["Phone"];
            user.ProfilePic = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0].FileName : "";

            if (user == null  || string.IsNullOrWhiteSpace(user.FirstName)
                 || string.IsNullOrWhiteSpace(user.LastName) || string.IsNullOrWhiteSpace(user.Email) 
                )
            {               
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Please enter all required fields."));
            }    

            var dbResponse = UsersMethods.AddUser(user);
            if(dbResponse.Error)
            {  
                if(dbResponse.ErrorMsg.Contains("UNQ__Users__Username"))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Username already exists."));
                else if (dbResponse.ErrorMsg.Contains("UNQ__Users__Email"))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Email already exists."));
                else
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Internal Error. " + dbResponse.ErrorMsg));
            }

            string user_Id = dbResponse.Ds.Tables[0].Rows[0][0].ToString();

            if (HttpContext.Current.Request.Files.Count > 0)
            {
                foreach (var fileName in HttpContext.Current.Request.Files.AllKeys)
                {
                    HttpPostedFile file = HttpContext.Current.Request.Files[fileName];
                    if (file != null)
                    {    
                        //var FileUniqueName = Guid.NewGuid().ToString();
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Attachments/Users/") + user_Id);
                        var rootPath = HttpContext.Current.Server.MapPath("~/Attachments/Users/" + user_Id);
                        var fileSavePath = System.IO.Path.Combine(rootPath, file.FileName);
                        file.SaveAs(fileSavePath);                        
                    }
                }//end of foreach

            }//end of if count > 0

            return ResponseMessage( Request.CreateResponse(  HttpStatusCode.OK, "New User has been added."));
        }


        [HttpGet]
        public UsersWithPage GetUsersWithPage(int PageSize, int PageNumber, string SortBy, string SortDirection, string Search)
        {
            return UsersMethods.GetUsersPage(PageSize, PageNumber, SortBy, SortDirection, Search);
        }
        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}