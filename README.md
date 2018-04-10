# GtaBot
A chat bot combined with Vision API for identifying batteries (!)
Read the blog post [here](http://pedro.digitaldias.com/?p=455) for details about the project. 

## Steps to replicate

* Make sure you have a version of [Visual Studio IDE](https://www.visualstudio.com) (any version will do)
* Navigate to your [Azure Portal] and set up a **Resource Group** for the project (recommended)
* Within the Resource group, create: 
  * One **Custom Vision Service**
  * One **Web App Bot** for hosting your bot
* Navigate to the [Custom Vision Portal](https://customvision.ai) to set up your Custom Vision project (see the blogpost for reference)
* Clone this project onto your computer and load the solution into Visual Studio  
* Publish the Bot code to your **Web App Bot**
* In the Settings page of your **Web App Bot** make sure to add TWO Settings:
  1. **VisionApiUrl** Taken from your Custom Vision Service Project
  2. **VisionPredictionKey** Taken from your Custom Vision Service Project

Your bot is now ready to be tested!
  
