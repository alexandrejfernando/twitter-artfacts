using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using Twittosfera.TwitterRobotEngine.Twitter.ConfiguracoesEngine;
using Twittosfera.TwitterRobotEngine.Twitter.KeysAndTokens;

namespace Twittosfera.TwitterRobotEngine.TwitterAutomationMainEngine
{
    public class TwitterAutomationMainEngine
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TwitterAutomationMainEngine()
        {
            #region [ Log4net ]

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            #endregion

            #region [ Twitter Authentication ]

            Auth.SetUserCredentials(KeysAndTokens.CONSUMER_KEY, KeysAndTokens.CONSUMER_SECRET, KeysAndTokens.ACCESS_TOKEN, KeysAndTokens.ACCESS_TOKEN_SECRET);

            #endregion
        }
        public void MountFriendsListCSV()
        {
            string file = @"C:\Users\alexandrej\Documents\twitterrobot\data\FriendsList.csv";
            if (File.Exists(file))
                File.Delete(file);

            using (StreamWriter outputFile = new StreamWriter(file, false))
            {
                outputFile.WriteLine("Now;Number;Id;UserName;IFollowNotFollowBack;FollowersCount;Certified;Link");

                //foreach (var friend in friends)
                //{

                //    IRelationshipDetails relationshipDetailsFollowBack = Friendship.GetRelationshipDetailsBetween(_authenticatedUser.ScreenName, friend.ScreenName);

                //    bool followBack = false;

                //    if (relationshipDetailsFollowBack != null)
                //    {
                //        followBack = relationshipDetailsFollowBack.FollowedBy;
                //    }


                //    string link = "https://twitter.com/{0}";

                //    outputFile.WriteLine(string.Concat(DateTime.Now.ToString(), ";",
                //                                           contador.ToString(), ";",
                //                                           friend.Id.ToString(), ";",
                //                                           friend.ScreenName, ";",
                //                                           followBack ? "Sim" : "Nao", ";",
                //                                           friend.FollowersCount, ";",
                //                                           friend.Verified ? "Sim" : "Nao", ";",
                //                                           string.Format(link, friend.ScreenName)));

                //    Console.WriteLine(string.Concat(contador.ToString(), " de ", friends.Count(), " (", Math.Round(((decimal)contador / friends.Count() * 100), 2), "%)"));
                //    contador++;
                //}
            }
        }



        private string _searchText;

        public void SetSearchText(string searchText)
        {
            _searchText = searchText;
        }
        public long Tweeta(string mensagem)
        {
            long retorno = 0;
            try
            {
                var tweet = Tweet.PublishTweet(mensagem);
                retorno = tweet.IsTweetPublished ? tweet.Id : 0;
            }
            catch (Exception e)
            {
                retorno = 0;
                log.Error(string.Concat(e.Message, " - ", e.StackTrace));
            }
            return retorno;
        }
        public ITweet RetornaTweet(long tweetId)
        {
            var tweet = Tweet.GetTweet(tweetId);
            return tweet;
        }
        public long ReplytoTweet(long tweetIdtoReplyTo, string mensagem)
        {
            long retorno = 0;
            try
            {
                var reply = Tweet.PublishTweet(mensagem, new PublishTweetOptionalParameters
                {
                    InReplyToTweetId = tweetIdtoReplyTo,
                    AutoPopulateReplyMetadata = true // Auto populate the @mentions
                });

                var tweetToReplyTo = Tweet.GetTweet(tweetIdtoReplyTo);

                // We must add @screenName of the author of the tweet we want to reply to
                var textToPublish = string.Format("@{0} {1}", tweetToReplyTo.CreatedBy.ScreenName, mensagem);
                var tweet = Tweet.PublishTweetInReplyTo(textToPublish, tweetIdtoReplyTo);
                retorno = tweet != null ? tweet.IsTweetPublished ? tweet.Id : 0 : 0;
            }
            catch (Exception e)
            {
                retorno = 0;
                log.Error(string.Concat(e.Message, " - ", e.StackTrace));
            }
            return retorno;
        }
        public long Retweet(long tweetId)
        {
            long retorno = 0;

            try
            {
                var tweet = Tweet.GetTweet(tweetId);
                var retweet = tweet.PublishRetweet();
                retorno = retweet.IsTweetPublished ? retweet.Id : 0;
            }
            catch (Exception e)
            {
                retorno = 0;
                log.Error(string.Concat(e.Message, " - ", e.StackTrace));
            }
            return retorno;
        }
        public long RetweetSearchedText(string mensagem)
        {
            long retorno = 0;
            try
            {

                var searchParameter = Search.CreateTweetSearchParameter(_searchText);
                searchParameter.MaximumNumberOfResults = ConfiguracoesEngine.SEARCH_MAXIMUM_NUMBER_OF_RESULTS;
                searchParameter.TweetSearchType = TweetSearchType.OriginalTweetsOnly;

                foreach (var item in Search.SearchTweets(searchParameter))
                {
                    DateTime momento = DateTime.Now;
                    Console.WriteLine(string.Concat(momento, " -> Retweeted: ", item.FullText));

                    var retweet = Tweet.GetTweet(ReplytoTweet(item.Id, mensagem));
                    if (retweet != null)
                    {
                        Console.WriteLine(string.Concat(momento, " -> Retweeted: ", retweet.FullText));
                    }
                    if (ConfiguracoesEngine.DELAY_MINUTOS_ENTRE_TWEETS_MASSA != 0)
                    {
                        Thread.Sleep(TimeSpan.FromMinutes(ConfiguracoesEngine.DELAY_MINUTOS_ENTRE_TWEETS_MASSA));
                    }
                    retorno++;
                    log.Info(string.Concat("#", retorno.ToString(), ": ", retweet == null ? "Não enviado" : string.Concat("Enviado para ", item.CreatedBy.ScreenName)));

                }
            }
            catch (Exception e)
            {
                retorno = 0;
                log.Error(string.Concat(e.Message, " - ", e.StackTrace));
            }
            return retorno;
        }


    }
}
