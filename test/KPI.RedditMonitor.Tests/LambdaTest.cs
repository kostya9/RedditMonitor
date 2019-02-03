using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using Amazon.Lambda.TestUtilities;
using KPI.RedditMonitor.ImageProcessing;
using Xunit;

namespace KPI.RedditMonitor.Tests
{
    public class LambdaTest
    {
        [Fact]
        public async Task FunctionHandler_MemoryTest()
        {
            TestLambdaContext context;

            Function functions = new Function();
            
            context = new TestLambdaContext();
            await functions.FunctionHandler(new SQSEvent
            {
                Records = GenerateEvents().Take(100).ToList()
            }, context);
        }

        IEnumerable<SQSEvent.SQSMessage> GenerateEvents()
        {
            while (true)
            {
                yield return new SQSEvent.SQSMessage()
                {
                    Body = @"
{'Id':'" + Guid.NewGuid() + @"
','ImageUrl':'https://i.redd.it/3tg239nugde21.jpg','RedditId':'amqiqg','Url':'/r/WrestleWithThePlot/comments/amqiqg/oh_zelina_and_her_booty/','Text':'OH Zelina and her booty) https://i.redd.it/3tg239nugde21.jpg','CreatedAt':'2019-02-03T15:31:14Z','Ignore':false,'FeatureBuckets':null}
"
                };
            }
        }
    }
}
