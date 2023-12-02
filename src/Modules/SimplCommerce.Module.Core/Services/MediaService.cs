using System;
using System.IO;
using System.Threading.Tasks;

using Ardalis.GuardClauses;

using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Models;

namespace SimplCommerce.Module.Core.Services;

public class MediaService : IMediaService
{
    private readonly IRepository<Media> _mediaRepository;
    private readonly IStorageService _storageService;

    public MediaService(IRepository<Media> mediaRepository, IStorageService storageService)
    {
        Guard.Against.Null(mediaRepository);
        Guard.Against.Null(storageService);

        _mediaRepository = mediaRepository;
        _storageService = storageService;
    }

    public string GetMediaUrl(Media media)
    {
        if (media == null)
        {
            return GetMediaUrl("no-image.png");
        }

        return GetMediaUrl(media.FileName);
    }

    public string GetMediaUrl(string fileName)
    {
        Guard.Against.NullOrWhiteSpace(fileName);

        return _storageService.GetMediaUrl(fileName);
    }

    public string GetThumbnailUrl(Media media)
    {
        return GetMediaUrl(media);
    }

    public Task SaveMediaAsync(Stream mediaBinaryStream, string fileName, string mimeType = null)
    {
        Guard.Against.Null(mediaBinaryStream);
        Guard.Against.NullOrWhiteSpace(fileName);

        return _storageService.SaveMediaAsync(mediaBinaryStream, fileName, mimeType);
    }

    /// <summary>
    /// Deletes the media file from the storage provider and the database.
    /// </summary>
    /// <param name="media">The media object that represents the media file to be deleted</param>
    /// <returns>Task</returns>
    /// <exception cref="ArgumentNullException">ArgumentNullException if media is null</exception>
    public Task DeleteMediaAsync(Media media)
    {
        Guard.Against.Null(media);

        _mediaRepository.Remove(media);
        return DeleteMediaAsync(media.FileName);
    }

    /// <summary>
    /// Deletes the media file from the storage provider based on its filename.
    /// </summary>
    /// <param name="fileName">The name of the file to delete</param>
    /// <returns>Task</returns>
    /// <exception cref="ArgumentNullException">ArgumentNullException if fileName is null</exception>
    public Task DeleteMediaAsync(string fileName)
    {
        Guard.Against.NullOrWhiteSpace(fileName);

        return _storageService.DeleteMediaAsync(fileName);
    }
}
