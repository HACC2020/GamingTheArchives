# Instructions

A demo of this application is available at [https://www.hiscribe.app/](https://www.hiscribe.app/). There's a few things you'll need to know about using the site.

The app should generally be compatible with major browsers, however most testing has been done on Chromium based browser (Google Chrome, Microsoft Edge, Brave, etc). There is at least one known issue in FireFox. So if possible, please use Google Chrome or a Chromium based web browser.

## Logging In

While the site should be accessible to anonymous visitors, it is read only until have have logged in. So if you want to transcribe documents or create new projects you'll need to sign up and log in. Sign up is done with Facebook. When you click "Continue With Facebook" or "Log In With Facebook" you will be taken to facebook.com where you may need to log in to your Facebook account, and on the first usage you will need to authorize the GamingTheArchives app. After you are logged in to Facebook you should be redirected to the [profile page](https://www.hiscribe.app/#/profile) where you will need to enter a display name and save your profile before you can start using the site.

## Transcribing

Transcribing is pretty self explanatory. First choose a project from the list. Next select a document to start with. On the transcription page you can type in data into each field, or you can click and drag one of the target icons next to each input field and drag it to a highlighted spot on the document to automatically fill in text that was found on the document using Azure Cognitive Services.

Your changes will be saved to the server as you go. You can click next or previous to see other documents. Once you have entered all the data you want for a given document you can click submit to make it ready for review, at which point it will become read only.

The help button is there to submit a question or report a problem to the Hawai'i State Archives staff. However it has not yet been implemented.

## Creating a new Project

This app is "fully" functional and not just demo ware! You can actually add new projects and start transcribing them. To do so, go to the home screen, click the circular green plus button in the lower right, and select Add Project. Enter a project name and hit next. Then you can optionally upload a specially formatted XML file that the Hawai'i State Archives uses to define their projects. You can find a number of examples in our [git repository](https://github.com/HACC2020/GamingTheArchives/tree/master/samples). It is important that the XML file be well formed and in the right format. On the next screen you can also manually add and modify fields.

The last step of creating a project is to submit a list of image URLs for the documents to be transcribed. It is important that these URLs be **publicly** accessible. URLs that start with `file:///` aren't going to work because they are only accessible on your own machine. Also some file hosting services restrict access to the files they host. To be sure your URL is publicly accessible check that it starts with `http://` or `https://` and that you can view it using an "Incognito" or "Private" browser window. If you need some URLs to test with you can find a list of samples [here](samples/ChineseArrivals_1878/images/image-urls.txt).

Once your project is created it will be listed as "Inactive" and only visible to logged in users. To publish your new project, click on its name to go to the project details page, then click the gear icon on the right and side of the page. This will take you to the settings screen where you can change details of the project, make adjustments to the field list, and activate the project by clicking the checkbox in the lower left. Once you save that change your project is ready to start archiving.

## TODO

I don't want to take away from the incredible amount of great functionality that has been implemented in this project. However there is a lot of work still to do on this project:

 * Badges awarded to transcribers for various achievements as they are working.
 * Leader boards showing who has transcribed the most documents.
 * An activity feed showing all recent activity on the site.
 * Personal statistics & activity feed showing each user all of their activity both as condensed statistics, list of documents, and as an exhaustive log.
 * Document Upload via integration with consumer file storage (OneDrive, Box.com, DropBox).
 * Reviewer workflow (review transcriptions, highlight discrepancies, merge and approve final versions).
 * Transcription field validation
 * Help/Problem requests and comment/discussion threads.