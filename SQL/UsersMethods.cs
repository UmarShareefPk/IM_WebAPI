﻿using IM.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace IM.SQL
{
    public class UsersMethods
    {
        public static UserLogin Login(string username, string password)
        {
            var loginTable = new DataTable();
            var userTable = new DataTable();

            var parameters = new SortedList<string, object>()
            {
                  { "Username" , username },
                  { "Password" , password },
            };

            var dbResponse = DataAccessMethods.ExecuteProcedure("Login", parameters);
            var ds = dbResponse.Ds;

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;
           
            loginTable   = ds.Tables[0];
            userTable = ds.Tables[1];

            var Users = (from rw in userTable.AsEnumerable()
                         select new User()
                         {
                             Id = rw["Id"].ToString(),                            
                             CreateDate = DateTime.Parse(rw["CreateDate"].ToString()),
                             FirstName = rw["FirstName"].ToString(),
                             LastName = rw["LastName"].ToString(),
                             ProfilePic = rw["ProfilePic"].ToString(),
                             Email = rw["Email"].ToString(),
                             Phone = rw["Phone"].ToString(),
                         }).ToList();


            var userLogin = (from rw in loginTable.AsEnumerable()
                         select new UserLogin()
                         {
                             Id = rw["Id"].ToString(),
                             user = Users.First(),
                             Username = rw["Username"].ToString(),
                             Password = rw["Password"].ToString(),
                             CreateDate = DateTime.Parse(rw["CreateDate"].ToString()),
                             LastLogin = DateTime.Now // DateTime.Parse(rw["LastLogin"].ToString())
                         }).ToList();

            return userLogin.First();
        }

       public static User GetUserById(string userId)
        {
            var dt = new DataTable();
            var parameters = new SortedList<string, object>()
            {
                  { "UserId" , userId },                 
            };

            var dbResponse = DataAccessMethods.ExecuteProcedure("UserById", parameters);
            var ds = dbResponse.Ds;

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            dt = ds.Tables[0];

            var Users = (from rw in dt.AsEnumerable()
                         select new User()
                         {
                             Id = rw["Id"].ToString(),                            
                             CreateDate = DateTime.Parse(rw["CreateDate"].ToString()),
                             FirstName = rw["FirstName"].ToString(),
                             LastName = rw["LastName"].ToString(),
                             ProfilePic = rw["ProfilePic"].ToString(),
                             Email = rw["Email"].ToString(),
                             Phone = rw["Phone"].ToString(),
                         }).ToList();

            return Users.First();
        }

        public static List<User> GetAllUsers()
        {
            var dt = new DataTable();
            var parameters = new SortedList<string, object>()
            {   };

            var dbResponse = DataAccessMethods.ExecuteProcedure("GetAllUsers", parameters);

            dt = dbResponse.Ds.Tables[0];

            var Users = (from rw in dt.AsEnumerable()
                         select new User()
                         {
                             Id = rw["Id"].ToString(),
                             CreateDate = DateTime.Parse(rw["CreateDate"].ToString()),
                             FirstName = rw["FirstName"].ToString(),
                             LastName = rw["LastName"].ToString(),
                             ProfilePic = rw["ProfilePic"].ToString(),
                             Email = rw["Email"].ToString(),
                             Phone = rw["Phone"].ToString(),
                         }).ToList();

            return Users;
        }

        public static DbResponse AddUser(User user)
        {
            var parameters = new SortedList<string, object>()
            {                
                  { "FirstName" , user.FirstName },
                  { "LastName" , user.LastName },
                  { "Email" , user.Email },
                  { "ProfilePic" , user.ProfilePic },
                  { "Phone" , user.Phone }                 
            };

            return DataAccessMethods.ExecuteProcedure("AddNewUser", parameters);
        }

        public static UsersWithPage GetUsersPage(int pageSize, int pageNumber, string sortBy, string sortDirection, string Serach)
        {
            var dt = new DataTable();
            var parameters = new SortedList<string, object>()
            {
                 { "PageSize" , pageSize},
                 { "PageNumber" , pageNumber},
                 { "SortBy" , sortBy},
                 { "SortDirection" , sortDirection},
                 { "SearchText" , Serach},
            };

            var dbResponse = DataAccessMethods.ExecuteProcedure("GetUsersPage", parameters);
            var ds = dbResponse.Ds;

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            dt = ds.Tables[1];
            int total_users = int.Parse(ds.Tables[0].Rows[0][0].ToString());

            var users = (from rw in dt.AsEnumerable()
                             select new User()
                             {
                                 Id = rw["Id"].ToString(),
                                 CreateDate = DateTime.Parse(rw["CreateDate"].ToString()),
                                 FirstName = rw["FirstName"].ToString(),
                                 LastName = rw["LastName"].ToString(),
                                 ProfilePic = rw["ProfilePic"].ToString(),
                                 Email = rw["Email"].ToString(),
                                 Phone = rw["Phone"].ToString(),
                             }).ToList();

            return new UsersWithPage
            {
                Total_Users = total_users,
                Users = users
            };
        }
    } // end of class

    public class UsersWithPage
    {
        public int Total_Users { get; set; }
        public List<User> Users { get; set; }
    }

} //end of namespace