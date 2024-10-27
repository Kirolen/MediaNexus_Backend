using System;

namespace MediaNexus_Backend
{
    public enum MediaStatusInUserList
    {
        InProcess,
        Completed,
        Planned,
        Dropped
    }

    public class UserMediaStatus
    {
        public int MediaID { get; set; }
        public int UserID { get; set; }
        public MediaStatusInUserList Status { get; set; }
        public int EndedPageOrEpisode { get; set; }

        public UserMediaStatus(int mediaId, int userId, MediaStatusInUserList status, int endedPageOrEpisode = 0)
        {
            MediaID = mediaId;
            UserID = userId;
            Status = status;
            EndedPageOrEpisode = endedPageOrEpisode;
        }

        public override string ToString()
        {
            return $"MediaID: {MediaID}, UserID: {UserID}, Status: {Status}, EndedPageOrEpisode: {EndedPageOrEpisode}";
        }
    }
}
