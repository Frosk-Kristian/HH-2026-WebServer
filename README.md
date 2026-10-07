# HH-2026-WebServer
ASP.NET web server for 2026 WADSIH Hardware Hack project.

## Layout
Overview of repository structure.
* [Controllers](Controllers) - Subdirectory for API controllers.
  - [TTSController.cs](Controllers/TTSController.cs) - Controller providing the functionality needed by our ESP32 (accepting an image, extracting text from image, generating and returning audio). Include additional functions for generating speech from text directly (which we used to create prerecorded phrases for error handling and status messages) and transcribing text from an image, returning the text.
  - [TestController.cs](Controllers/TestController.cs) - Controller used to test server connection, provides functionality to ping the server.
* [Properties](Properties) - Subdirectory for application properties/settings, mostly auto-generated files.
  - [launchSettings.json](Properties/launchSettings.json) - Application launch settings.
* [Services](Services) - Subdirectory for various helper classes and services.
  - [OpenRouterService.cs](Services/OpenRouterService.cs) - Service handling communicating with the OpenRouter API, we use this in TTSController to get Google Gemma  4-31b to perform image transcription for us, returning the resulting text.
  - [TtsService.cs](Services/TtsService.cs) - Service handling text to speech, using the library KokoroSharp to generate audio from supplied text, format it as a .wav and returning a resulting byte array (this is never saved locally on the server, just temporarily kept in memory for privacy reasons and to limit impact on server storage).
* [Program.cs](Program.cs) - Initial entry point into application, services are registered here.
* [README.md](README.md) - Markdown file for README (this file).
* [appsettings.Development.json](appsettings.Development.json) - Separate server settings for development, unused at this point.
* [appsettings.json](appsettings.json) - Server config and settings, before running must add a valid OpenRouter API key to this file (do not commit key to repo, it has been left blank intentionally).
