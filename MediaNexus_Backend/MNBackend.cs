using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaNexus_Backend
{
    public enum RegisterResult
    {
        Success,
        UserLoginTaken,
        EmailTaken,
        RegistrationFailed,
        Error
    }

    static public class MNBackend
    {
        static public bool CheckLogin(string loginUser, string passUser)
        {
            return DB.CheckLogin(loginUser, passUser); 
        }

        static public RegisterResult Register(string userLogin, string password, string email)
        {
            return DB.Register(userLogin, password,email);
        }
    }
}
