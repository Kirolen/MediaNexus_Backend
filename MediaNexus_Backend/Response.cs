using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaNexus_Backend
{
    public enum ResponseType
    {
        Neutral,
        Negative,
        Positive
    }

    public class UserResponse
    {
        public int UserID { get; set; }
        public int ResponseID { get; set; }
        public int MediaID { get; set; }
        public string ResponseText { get; set; }
        public ResponseType ResponseType { get; set; } = ResponseType.Neutral;
        public string UserNickname { get; set; }
        public string UserIMGURL { get; set; }

        public UserResponse(int userID, int responseID, int mediaID, string responseText, ResponseType responseType, string userNickname, string userIMGURL)
        {
            UserID = userID;
            ResponseID = responseID;
            MediaID = mediaID;
            ResponseText = responseText;
            ResponseType = responseType;
            UserNickname = userNickname;
            UserIMGURL = userIMGURL;
        }
    }
}