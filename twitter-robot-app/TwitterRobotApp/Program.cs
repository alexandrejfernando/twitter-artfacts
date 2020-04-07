using System;

namespace TwitterRobotApp
{
    class Program
    {
        private static TwitterRobotEngine _twitterRobotEngine;
      
        static void Main(string[] args)
        {
            _twitterRobotEngine = new TwitterRobotEngine();
            _twitterRobotEngine.SetSearchText("#ForaBolsonaro");
            _twitterRobotEngine.RetweetSearchedText("Presidente Lixo!");
        }
    }
}
