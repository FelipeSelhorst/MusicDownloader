using System;
using System.Collections.Generic;
using System.Text;

namespace MusicDownloader.Models;

public record DownloadResult
(
    bool Success,
    string? FilePath = null,
    string? ErrorMessage = null
);