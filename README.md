<div align="center">
    <h1>MusicDownloader</h1>
    <img src="https://img.shields.io/badge/Version-1.1.0-blue" alt="Version">
    <img src="https://img.shields.io/badge/License-MIT-green" alt="License">
    <img src="https://img.shields.io/badge/Language-C%23-blue" alt="Language">
</div>

<br>

Terminal based music downloader in C#.

It downloads the music from YouTube using yt-dlp, and converts it to mp3 using ffmpeg. It then exports the downloaded music information to a JSON file.

The project was created to work alongside with [Boombox Project](https://github.com/requiemkind/boombox).

<h2 align="center">Requirements</h2>

Before running the application, make sure you have the following installed on your machine:

    - yt-dlp.
    - FFmpeg.
    - .NET SDK 10.

Make sure yt-dlp and FFmpeg are installed and accessible through your system's PATH, otherwise the application will not work correctly.

### How to install the requirements using winget

In your preferred terminal, run the following commands:

For <strong>yt-dlp & FFmpeg</strong>:

    winget install --source winget --id yt-dlp.yt-dlp

If <strong>FFmpeg</strong> does not install automatically, you can install it separately:

    winget install -e --id Gyan.FFmpeg

<strong>.NET SDK 10</strong>

    winget install -e --id Microsoft.DotNet.SDK.10

> - Type "y" when asked to agree to the license terms and start the download.
> - Follow the instructions given by the installer to complete the installation.

When everything is completed, verify the installation:

    ffmpeg -version
    yt-dlp --version
    dotnet --version

<h2 align="center">How to Use</h2>

### Option 1: Download the Application

Download the latest version for Windows [here](https://github.com/FelipeSelhorst/MusicDownloader/releases/tag/v1.0.0).

    - Go to the Releases page.
    - Download the latest Windows build.
    - Extract the downloaded archive.
    - Install or configure the required dependencies (yt-dlp and FFmpeg).
    - Run MusicDownloader.exe and paste a YouTube URL when prompted.

### Option 2: Run from Source

    - Clone or download this repository.
    - Open the project in Visual Studio or preferred IDE.
    - Run the application in your IDE.

Downloaded songs are saved in the songs folder. The application scans this folder and updates songs.json with the available songs for the Boombox playlist.

<h2 align="center">Build Output</h2>

When you build or run the project through an IDE, the executable is typically generated in a directory similar to:

`bin/Debug/net10.0/`

The exact path depends on your project configuration and target framework.

<h2 align="center">Notes</h2>

    - yt-dlp is used for downloading music from YouTube.
    - FFmpeg is used for converting the downloaded music to mp3.
    - The playlist is stored in songs/songs.json.
    - If you download an existing song, it will replace the old version so there are no duplicates.

<h2 align="center">Contributing</h2>

Feel free to fork the repository and submit pull requests. For major changes, please open an issue first to discuss what you would like to change.