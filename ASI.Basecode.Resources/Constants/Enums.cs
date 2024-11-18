namespace ASI.Basecode.Resources.Constants
{
    /// <summary>
    /// Class for enumerated values
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// API Result Status
        /// </summary>
        public enum Status
        {
            Success,
            Error,
            CustomErr,
        }

        /// <summary>
        /// Login Result
        /// </summary>
        public enum LoginResult
        {
            Success = 0,
            Failed = 1,
        }

        public enum ChangePassToken
        {
            Invalid= 0,
            Valid = 1,
        }
        public enum Roles
        {
            User,
            Admin,
            Superadmin
        }

        public enum RoomType
        {
            MeetingRoom, //Has TV
            PrivateRoom, //Personal space for any work
            StudyRoom,   //Designed for studying
            ConferenceRoom, //Has TV and whiteboard
            WorkshopRoom, //Has whiteboard only
        }

        public enum Location
        {
            Hive,            // A buzzing hub for collaboration and creativity
            FocusRoom,       // A dedicated space for quiet work and concentration
            Lounge,          // A casual area for relaxation and informal meetings
            InnovationLab,   // A room designed for brainstorming and project development
            ResourceCenter   // A place equipped with tools and materials for research and study
        }

        public enum BookingStatus
        {
            Approved,
            Disapproved,
            Cancelled
        }

    }
}
