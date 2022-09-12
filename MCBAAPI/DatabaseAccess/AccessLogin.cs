using MCBAAPI.Models;
using MCBAAPI.Data;
using System.Diagnostics;

namespace MCBAAPI.DatabaseAccess
{
    public static class AccessLogin
    {
        public static void Create(McbaContext context, Login login)
        {
            context.Logins.Add(login);
        }

        public static Login Read(McbaContext context, string loginId)
        {
            var login = context.Logins.Find(loginId);
            return login;
        }

        public static List<Login> ReadAll(McbaContext context)
        {
            var loginList = context.Logins.ToList();
            return loginList;
        }

        public static void Update(McbaContext context, Login newLogin)
        {
            var oldLogin = context.Logins.Find(newLogin.LoginID);
            if (newLogin.LoginID != null)
                oldLogin.LoginID = newLogin.LoginID;
            if (newLogin.CustomerID != 0)
                oldLogin.CustomerID = newLogin.CustomerID;
            if (newLogin.PasswordHash != null)
                oldLogin.PasswordHash = newLogin.PasswordHash;
        }

        public static void Delete(McbaContext context, Login login)
        {
            context.Logins.Remove(login);
        }
    }
}
