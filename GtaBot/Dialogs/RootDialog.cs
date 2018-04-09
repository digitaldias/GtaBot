using GtaBot.Cognitive;
using Microsoft.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GtaBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            
            var activity = await result as Activity;
            var quickReply = activity.CreateReply();
            quickReply.Type = ActivityTypes.Typing;
            quickReply.Text = "Hold on a tic..";
            await context.PostAsync(quickReply);

            if(activity.Attachments.Any() && activity.Attachments[0].ContentType == "image/jpeg")
            {
                var visionApiUrl  = CloudConfigurationManager.GetSetting("VisionApiUrl");
                var predictionKey = CloudConfigurationManager.GetSetting("VisionPredictionKey");
                var visionApi     = new GtaVisionApi(visionApiUrl, predictionKey); 

                var contentData   = await GetImageDataAsync(activity);
                var visionResult  = await visionApi.GetPrediction(contentData);
                var scores        = visionResult.Predictions.Where(p => p.Probability > 0.8);

                await ReactToScores(context, activity, scores);                
            }
            else
            {
                await context.PostAsync("I've no idea how to deal with that! Try sending me an image?");
            }

            context.Wait(MessageReceivedAsync);
        }


        private async Task ReactToScores(IDialogContext context, Activity activity, IEnumerable<Prediction> scores)
        {
            HeroCard card;
            if(scores.Any(s => s.Tag == "duracell"))
            {
                const string duracellBunnyImage = "http://www.africanmc.org/media/k2/items/cache/4d8c9898b5bb88437f053c8b957f47f3_XL.jpg";
                card = new HeroCard("Power the bunnies!", "This is no less than a duracell bunny battery!", null, new List<CardImage> { new CardImage(duracellBunnyImage) }, null, null);
            }
            else if(scores.Any(s => s.Tag == "battery"))
            {
                const string disposeBatteryUrl = "http://www.acwastewatcher.org/wp-content/uploads/2013/11/battery-recycle-2.jpg";
                card = new HeroCard("This is a Battery", "Be mindful of how you dispose your batteries", null, new List<CardImage> { new CardImage(disposeBatteryUrl) }, null, null);
            }
            else if (scores.Any(s => s.Tag == "batteries"))
            {
                const string badBatteriesUrl = "https://www.vapeloft.com/image/catalog/Blog%20Images/battery%20exploding/efests-are-shit.jpg";
                card = new HeroCard("These are batteries", "Don't use recognized brand batteries!", "You shouldn't mess with unknown brand batteries", new List<CardImage> { new CardImage(badBatteriesUrl) }, null, null);
            }
            else
            {
                const string noIdeaImageUrl = "https://pre00.deviantart.net/eda5/th/pre/i/2013/317/0/c/no_clue_daffy__not_my_drawing__only_my_quotes_by_bushtaushamay2011-d6u64b0.jpg";
                card = new HeroCard("Hmm.. Tough one!", "I've no idea what you just showed me", "Try something else, like an image of a battery... ,-)", new List<CardImage> { new CardImage(noIdeaImageUrl) }, null, null);
            }

            var reply = activity.CreateReply();
            reply.Attachments.Add(card.ToAttachment());

            await context.PostAsync(reply);
        }


        private async Task<byte[]> GetImageDataAsync(Activity activity)
        {
            var url        = activity.Attachments[0].ContentUrl;
            var httpClient = new HttpClient();
            return await httpClient.GetByteArrayAsync(url);
        }
    }
}