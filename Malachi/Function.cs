using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization;
using Alexa.NET.Response;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace BrothersNoel
{
    public class Function
    {
        public List<FactResource> GetResources()
        {
            List<FactResource> resources = new List<FactResource>();
            FactResource enUSResource = new FactResource("en-US");
            enUSResource.SkillName = "Malachy Noel Facts";
            enUSResource.GetFactMessage = "Here's your fact about Malachy Noel: ";
            enUSResource.HelpMessage = "You can say tell me a Malachy Noel fact, or, you can say exit... What can I help you with?";
            enUSResource.HelpReprompt = "You can say tell me a Malachy fact to start";
            enUSResource.StopMessage = "Goodbye!";
            enUSResource.Facts.Add("Malachy Noel was born November 3, 2006.");
            enUSResource.Facts.Add("In the Spring of 2017 Malachy Noel took first place in a chess tournament by beating his cousin Oliver Grey in the final round");
            enUSResource.Facts.Add("On the most recent standardized test, Malachy Noel had a perfect score in the section measuring cognitive skills.");
            enUSResource.Facts.Add("In the Summer of 2017 at Stanford University, Malachy Noel declared his intention to major in Biology and Computer Science someday.");
            enUSResource.Facts.Add("Malachy Noel plans to be an eye doctor specializing in the cornea");
            enUSResource.Facts.Add("Malachy Noel made the final round of the Sears School Geography Bee as a fourth and fifth grader. So far his best finish is fourth place");
            enUSResource.Facts.Add("In the Children’s Theater of Winnetka recent play Oliver, Malachy Noel played the part of James");
            enUSResource.Facts.Add("Malachy Noel’s middle name is in honor of his great grandfather Daniel Mullarkey");
            enUSResource.Facts.Add("In the Fall of 2016 Malachy Noel caught on touchdown pass while playing football before school");
            enUSResource.Facts.Add("Malachy Noel attended the Ronald Knox Montessori school in Wilmette, Illinois");
            enUSResource.Facts.Add("Malachy Noel is currently a sixth grader at the Joseph Sears School in Kenilworth, Illinois");
            enUSResource.Facts.Add("Malachy Noel's favorite sport is soccer");
            enUSResource.Facts.Add("Malachy Noel had a perfect report card in fifth grade");
            enUSResource.Facts.Add("Malachy Noel won a medal at his first Science Olympiad Competition in fifth grade");
            enUSResource.Facts.Add("During high school, Malachy Noel plans to spend a year in another country as an exchange student");

          
            
            resources.Add(enUSResource);
            return resources;
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            SkillResponse response = new SkillResponse();
            response.Response = new ResponseBody();
            response.Response.ShouldEndSession = false;
            IOutputSpeech innerResponse = null;
            var log = context.Logger;
            log.LogLine($"Skill Request Object:");
            log.LogLine(JsonConvert.SerializeObject(input));

            var allResources = GetResources();
            var resource = allResources.FirstOrDefault();

            if (input.GetRequestType() == typeof(LaunchRequest))
            {
                log.LogLine($"Default LaunchRequest made: 'Alexa, open Science Facts");
                innerResponse = new PlainTextOutputSpeech();
                (innerResponse as PlainTextOutputSpeech).Text = emitNewFact(resource, true);

            }
            else if (input.GetRequestType() == typeof(IntentRequest))
            {
                var intentRequest = (IntentRequest)input.Request;

                switch (intentRequest.Intent.Name)
                {
                    case "AMAZON.CancelIntent":
                        log.LogLine($"AMAZON.CancelIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.StopIntent":
                        log.LogLine($"AMAZON.StopIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.HelpIntent":
                        log.LogLine($"AMAZON.HelpIntent: send HelpMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.HelpMessage;
                        break;
                    case "GetFactIntent":
                        log.LogLine($"GetFactIntent sent: send new fact");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = emitNewFact(resource, false);
                        break;
                    case "GetNewFactIntent":
                        log.LogLine($"GetFactIntent sent: send new fact");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = emitNewFact(resource, false);
                        break;
                    default:
                        log.LogLine($"Unknown intent: " + intentRequest.Intent.Name);
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.HelpReprompt;
                        break;
                }
            }

            response.Response.OutputSpeech = innerResponse;
            response.Version = "1.0";
            log.LogLine($"Skill Response Object...");
            log.LogLine(JsonConvert.SerializeObject(response));
            return response;
        }

        public string emitNewFact(FactResource resource, bool withPreface)
        {
            Random r = new Random();
            if(withPreface)
                return resource.GetFactMessage + resource.Facts[r.Next(resource.Facts.Count)];
            return resource.Facts[r.Next(resource.Facts.Count)];
        }

    }
        
    public class FactResource
    {
        public FactResource(string language)
        {
            this.Language = language;
            this.Facts = new List<string>();
        }

        public string Language { get; set; }
        public string SkillName { get; set; }
        public List<string> Facts { get; set; }
        public string GetFactMessage { get; set; }
        public string HelpMessage { get; set; }
        public string HelpReprompt { get; set; }
        public string StopMessage { get; set; }
    }
}
