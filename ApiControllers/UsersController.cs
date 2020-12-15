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

        [HttpPost]
        public IHttpActionResult AddUser([FromBody] User user)
        {            
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
            return ResponseMessage( Request.CreateResponse(  HttpStatusCode.OK, "New User has been added."));
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