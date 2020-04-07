using System;
using System.Threading;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using log4net;
using System.Reflection;
using log4net.Config;
using System.IO;

namespace TwitterRobotApp
{
    public class TwitterRobotEngine
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public TwitterRobotEngine()
        {
            #region [ Log4net ]

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            #endregion
            
            #region [ Twitter Authentication ]

            Auth.SetUserCredentials(KeysAndTokens.CONSUMER_KEY, KeysAndTokens.CONSUMER_SECRET, KeysAndTokens.ACCESS_TOKEN, KeysAndTokens.ACCESS_TOKEN_SECRET);

            #endregion

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
                log.Error(string.Concat(e.Message," - ",e.StackTrace));
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
                retorno = tweet!=null? tweet.IsTweetPublished ? tweet.Id : 0 : 0;
            }
            catch (Exception e)
            {
                retorno = 0;
                log.Error(string.Concat(e.Message," - ",e.StackTrace));
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
                log.Error(string.Concat(e.Message," - ",e.StackTrace));
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
                    log.Info(string.Concat("#", retorno.ToString(), ": ", retweet == null ? "Não enviado" : string.Concat("Enviado para ",item.CreatedBy.ScreenName)));

                }
            }
            catch (Exception e)
            {
                retorno = 0;
                log.Error(string.Concat(e.Message," - ",e.StackTrace));
            }
            return retorno;
        }
    }
}
