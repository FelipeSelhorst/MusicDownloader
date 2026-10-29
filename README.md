# MusicDownloader

<div align="center">
    ![Version](https://img.shields.io/badge/Version-1.0.0-blue)
    ![License](https://img.shields.io/badge/License-MIT-green)
    ![Language](https://img.shields.io/badge/Language-C%23-blue)
</div>

<br>

Terminal based music downloader in C#.

It downloads the music from YouTube using yt-dlp, and converts it to mp3 using ffmpeg. It then exports the downloaded music information to a JSON file.

The project was created to work alongside with [Boombox](https://github.com/requiemkind/boombox).

### Requirements

Before running the application, make sure you have the following installed on your machine:

    - yt-dlp.
    - FFmpeg.
    - The .NET SDK required by the project.

Make sure yt-dlp and FFmpeg are installed and accessible through your system's PATH.

### How to Use

#### Option 1: Download the Application

    - Go to the Releases page.
    - Download the latest Windows build.
    - Extract the downloaded archive, if applicable.
    - Install or configure the required dependencies (yt-dlp and FFmpeg).
    - Run MusicDownloader.exe and paste a YouTube URL when prompted.

#### Option 2: Run from Source

    - Clone or download this repository.
    - Open the project in Visual Studio.
    - Run the application by pressing F5.

Downloaded songs are saved in the songs folder. The application scans this folder and updates songs.json with the available songs for the Boombox playlist.

### Build Output

When you build or run the project in Visual Studio, the executable is typically generated in a directory similar to:

bin/Debug/net10.0/

The exact path depends on your project configuration and target framework.

### Notes

    - yt-dlp is used for downloading music from YouTube.
    - FFmpeg is used for converting the downloaded music to mp3.
    - The playlist is stored in songs/songs.json.
    - Existing songs are added to the playlist without duplicated entries based on their filenames.