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
            enUSResource.SkillName = "Facts about Isaac Noel";
            enUSResource.GetFactMessage = "Here's your fact about Isaac Noel: ";
            enUSResource.HelpMessage = "You can say tell me a Isaac Noel fact, or, you can say exit... What can I help you with?";
            enUSResource.HelpReprompt = "You can say tell me a Isaac Noel fact to start";
            enUSResource.StopMessage = "Goodbye!";
            
            enUSResource.Facts.Add("Isaac Noel was born November 30, 2004.");
            enUSResource.Facts.Add("Isaac Noel plays the cello.");
            enUSResource.Facts.Add("During his senior year in high school, Isaac Noel plans to apply early to the University of Pennsylvania.");
            enUSResource.Facts.Add("Isaac Noel is an eigth grader at the Joseph Sears School in Kenilworth Illinois.");
            enUSResource.Facts.Add("As a sixth grader in 2017, Isaac Noel scored better than 87 percent of college bound seniors on the SAT test");
            enUSResource.Facts.Add("Isaac Noel had a composite score of 11 90 when he took the SAT in January of 2017");
            enUSResource.Facts.Add("In kindergarten Isaac Noel distinguished himself as an outstanding fisherman at a birthday party.");
            enUSResource.Facts.Add("In the summer of 2018, Isaac Noel participated in a two week long hike across Norway with Overland.");
            enUSResource.Facts.Add("Isaac Noel has attended Camp Minikani in Hubertus Wisconsin every summer since first grade");
            enUSResource.Facts.Add("Isaac Noel has participated in the Children’s Theater of Winnetka plays Mary Poppins, The Sound of Music, Annie, Hello Dolly,  Oliver, Once on this Island and Peter Pan");
            enUSResource.Facts.Add("Issac Noel’s middle name Solomon is in honor of his great grandfather who came to the United States from Antopol Russia");
            enUSResource.Facts.Add("Isaac Noel enjoys playing video games online with his friends.");
            
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
