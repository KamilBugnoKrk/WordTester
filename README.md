# WordTester
![Build status](https://github.com/KamilBugnoKrk/WordTester/actions/workflows/dotnet.yml/badge.svg)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=KamilBugnoKrk_WordTester&metric=ncloc)](https://sonarcloud.io/dashboard?id=KamilBugnoKrk_WordTester)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=KamilBugnoKrk_WordTester&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=KamilBugnoKrk_WordTester)

<img src="WordTester.gif" alt="WordTester">

<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li>
		<a href="#acknowledgements">Acknowledgements</a>
		<ul>
			<li><a href="#related-to-blazor">Related to Blazor</a></li>
			<li><a href="#related-to-c">Related to C#</a></li>
			<li><a href="#related-to-images">Related to images</a></li>
			<li><a href="#others">Others</a></li>
		</ul>
	</li>
  </ol>
</details>


## About The Project

There are several tools for learning foreign words using flashcards and spaced repetition, but I found that most of them don't include the functionality of learning in a context. The main goal of this project is to create an application that requires you not only to provide the translation of a word but also a definition and example use of the given word. Thanks to it, I believe that the quality of your learning and memorization will be greatly improved.
### Built With

* [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
* [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
* [Azure Text to Speech](https://azure.microsoft.com/en-us/services/cognitive-services/text-to-speech/)

## Getting Started

This section contains instructions on setting up the project locally. 

### Prerequisites

* Visual Studio 2019 16.8 or later with the ASP.NET and web development workload
* .NET 5.0 SDK or later

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/KamilBugnoKrk/WordTester.git
   ```
2. Open in Visual Studio
3. Use your configuration data:
  - appsettings.json:
    - SpeechServiceKey - your Azure Speech Key
    - FTPName - name of your FTP
    - FTPPassword - password to your FTP
    - FTPURL - URL to your FTP
  - ReCaptcha.cs
    - secret - secret to your reCaptcha
4. Click IIS Express to run
5. The application will be available on https://localhost:44370/, API on https://localhost:44370/swagger/index.html

## License

Distributed under the GPL-3.0 License. See `LICENSE` for more information.

## Contact

Kamil Bugno - [Website](https://kamilbugno.com/)

E-mail: hello@wordtester.org

Project Website: [http://wordtester.org](http://wordtester.org)

Project Repository: [https://github.com/KamilBugnoKrk/WordTester](https://github.com/KamilBugnoKrk/WordTester)

## Acknowledgements
It would not be possible to start developing this project without the great free resources. I would like to say thanks to all people who dedicate their time to develop open-source projects:

### Related to Blazor
* [BlazorWithIdentity](https://github.com/stavroskasidis/BlazorWithIdentity) - project template
* [bUnit](https://github.com/egil/bunit) - testing library
* [MatBlazor](https://github.com/SamProf/MatBlazor) - material design components
* [MudBlazor](https://github.com/Garderoben/MudBlazor) - component library based on Material design
* [Blazored Modal](https://github.com/Blazored/Modal) - modal implementation 
* [Toolbelt.Blazor.HotKeys](https://github.com/jsakamoto/Toolbelt.Blazor.HotKeys) - keyboard shortcuts
* [Blazor-UseGoogleReCAPTCHA](https://github.com/sample-by-jsakamoto/Blazor-UseGoogleReCAPTCHA) - example usage of reCaptcha

### Related to C#
* [Flurl](https://flurl.dev/) - HTTP client library
* [xUnit.net](https://xunit.net/) - unit testing tool
* [Fluent Assertions](https://fluentassertions.com/) - Fluent API for asserting the results of unit tests 
* [Moq](https://github.com/moq/moq4) - mocking framework 
* [MediatR](https://github.com/jbogard/MediatR) - mediator implementation
* [combinatorics](https://github.com/eoincampbell/combinatorics) - combinatorics library
* [NetArchTest](https://github.com/BenMorris/NetArchTest) - Fluent API for .Net that can enforce architectural rules in unit tests

### Related to images
* [unDraw](https://undraw.co/) - open-source illustrations
* Main photo by <a href="https://unsplash.com/@chuklanov?utm_source=unsplash&utm_medium=referral&utm_content=creditCopyText">Avel Chuklanov</a> on <a href="https://unsplash.com/s/photos/learning?utm_source=unsplash&utm_medium=referral&utm_content=creditCopyText">Unsplash</a>
  
### Others
* [Best-README-Template](https://github.com/othneildrew/Best-README-Template) - README template