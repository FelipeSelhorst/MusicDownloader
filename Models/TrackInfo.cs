using System;
using System.Collections.Generic;
using System.Text;

namespace MusicDownloader.Models;

public record TrackInfo(
    string Title,
    string Artist,
    string? Album
);
