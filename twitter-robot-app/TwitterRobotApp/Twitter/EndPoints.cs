using System;
using System.Collections.Generic;
using System.Text;
using static TwitterRobotApp.Twitter.ListTypes;

namespace TwitterRobotApp.Twitter
{
    public class EndPoints
    {
        private readonly Dictionary<string, string> endPoints =
        new Dictionary<string, string>();

        public EndPoints()
        {
            endPoints.Add("followers", "https://api.twitter.com/1.1/followers/ids.json?screen_name={0}");
            endPoints.Add("friends", "https://api.twitter.com/1.1/friends/ids.json?screen_name={0}");
            endPoints.Add("friendship_unfollow", "https://api.twitter.com/1.1/friendships/destroy.json?user_id={0}");
            endPoints.Add("friends_list", "https://api.twitter.com/1.1/friends/list.json?cursor=-1&screen_name={0}&skip_status=true&include_user_entities=false");
        }
        public string GetEndPoint (TwitterApiList endPointType)
        {
            return endPoints[endPointType.ToString().ToLower()].ToString().ToLower();
        } 
    }
}
