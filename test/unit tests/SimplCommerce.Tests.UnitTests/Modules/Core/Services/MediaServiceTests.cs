using AutoFixture;
using AutoFixture.AutoMoq;

using Moq;

using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.StorageLocal;

namespace SimplCommerce.Tests.UnitTests.Modules.Core.Services;

public class MediaServiceTests
{
    private MediaService _testClass;
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Media>> _mediaRepository;
    private readonly Mock<IStorageService> _storageService;

    public MediaServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mediaRepository = _fixture.Freeze<Mock<IRepository<Media>>>();
        _storageService = _fixture.Freeze<Mock<IStorageService>>();
        _testClass = _fixture.Create<MediaService>();
    }

    [Fact]
    public void Can_Construct()
    {
        // Act
        var instance = new MediaService(_mediaRepository.Object, _storageService.Object);

        // Assert
        Assert.NotNull(instance);
    }

    [Fact]
    public void Cannot_Construct_WithNull_MediaRepository()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new MediaService(default(IRepository<Media>), _storageService.Object));

        Assert.Equal("Value cannot be null. (Parameter 'mediaRepository')", exception.Message);
    }

    [Fact]
    public void Cannot_Construct_WithNull_StorageService()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new MediaService(_mediaRepository.Object, default(IStorageService)));

        Assert.Equal("Value cannot be null. (Parameter 'storageService')", exception.Message);
    }

    [Fact]
    public void CanCall_GetMediaUrl_WithMedia()
    {
        // Arrange
        var media = _fixture.Create<Media>();
        var localStorageService = new LocalStorageService();
        _testClass = new MediaService(_mediaRepository.Object, localStorageService);

        // Act
        var actual = _testClass.GetMediaUrl(media);

        // Assert
        Assert.Equal($"/user-content/{media.FileName}", actual);
    }

    [Fact]
    public void CanCall_GetMediaUrlWithMedia_WithMedia()
    {
        // Arrange
        var fileName = $"{_fixture.Create<string>()}.gif";
        var localStorageService = new LocalStorageService();
        _testClass = new MediaService(_mediaRepository.Object, localStorageService);

        var media = new Media
        {
            FileName = fileName
        };

        // Act
        var actual = _testClass.GetMediaUrl(media);

        // Assert
        Assert.Equal($"/user-content/{fileName}", actual);
    }

    [Fact]
    public void CanCall_GetMediaUrlWithMedia_WithNull_Media()
    {
        // Arrange
        // Using LocalStorageService only to validate that "no-image.png" is returned when media is null.
        // If not, then I'd be only testing that _testClass.GetMediaUrl() didn't throw an
        var localStorageService = new LocalStorageService();
        _testClass = new MediaService(_mediaRepository.Object, localStorageService);

        // Act
        var actual = _testClass.GetMediaUrl(default(Media));

        // Assert
        Assert.Equal("/user-content/no-image.png", actual);
    }

    [Fact]
    public void CanCall_GetMediaUrl_WithString()
    {
        // Arrange
        var fileName = _fixture.Create<string>();

        var localStorageService = new LocalStorageService();
        _testClass = new MediaService(_mediaRepository.Object, localStorageService);

        // Act
        var actual = _testClass.GetMediaUrl(fileName);

        // Assert
        Assert.Equal($"/user-content/{fileName}", actual);
    }

    [Theory]
    [InlineData(null, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    [InlineData("   ", typeof(ArgumentException))]
    public void CannotCall_GetMediaUrlWithString_WithInvalid_FileName(string value, Type exceptionType)
    {
        Assert.Throws(exceptionType, () => _testClass.GetMediaUrl(value));
    }

    [Fact]
    public void CanCall_GetThumbnailUrl_WithMedia()
    {
        // Arrange
        var fileName = $"{_fixture.Create<string>()}.gif";
        var localStorageService = new LocalStorageService();
        _testClass = new MediaService(_mediaRepository.Object, localStorageService);

        var media = new Media
        {
            FileName = fileName
        };

        // Act
        var actual = _testClass.GetThumbnailUrl(media);

        // Assert
        Assert.Equal($"/user-content/{fileName}", actual);
    }

    [Fact]
    public void CannotCall_GetThumbnailUrl_WithNull_Media()
    {
        // Arrange
        // Using LocalStorageService only to validate that "no-image.png" is returned when media is null.
        // If not, then I'd be only testing that _testClass.GetMediaUrl() didn't throw an
        var localStorageService = new LocalStorageService();
        _testClass = new MediaService(_mediaRepository.Object, localStorageService);

        // Act
        var actual = _testClass.GetThumbnailUrl(default(Media));

        // Assert
        Assert.Equal("/user-content/no-image.png", actual);
    }

    [Fact]
    public async Task CanCall_SaveMediaAsync()
    {
        // Arrange
        var mediaBinaryStream = _fixture.Create<Stream>();
        var fileName = _fixture.Create<string>();
        var mimeType = _fixture.Create<string>();

        // Act
        await _testClass.SaveMediaAsync(mediaBinaryStream, fileName, mimeType);

        // Assert
        // Not sure what to Assert here
    }

    [Fact]
    public async Task CannotCall_SaveMediaAsync_WithNull_MediaBinaryStream()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SaveMediaAsync(default(Stream), _fixture.Create<string>(), _fixture.Create<string>()));
    }

    [Theory]
    [InlineData(null, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    [InlineData("   ", typeof(ArgumentException))]
    public async Task CannotCall_SaveMediaAsync_WithInvalid_FileName(string value, Type exceptionType)
    {
        await Assert.ThrowsAsync(exceptionType, () => _testClass.SaveMediaAsync(_fixture.Create<Stream>(), value, _fixture.Create<string>()));
    }

    [Fact]
    public async Task CanCall_DeleteMediaAsync_WithMedia()
    {
        // Arrange
        var media = _fixture.Create<Media>();

        // Act
        await _testClass.DeleteMediaAsync(media);

        // Assert
        _storageService.Verify(mock => mock.DeleteMediaAsync(It.IsAny<string>()));
    }

    [Fact]
    public async Task CannotCall_DeleteMediaAsyncWithMedia_WithNull_Media()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.DeleteMediaAsync(default(Media)));
    }

    [Fact]
    public async Task CanCall_DeleteMediaAsync_WithString()
    {
        // Arrange
        var fileName = _fixture.Create<string>();

        // Act
        await _testClass.DeleteMediaAsync(fileName);

        // Assert
        _storageService.Verify(mock => mock.DeleteMediaAsync(It.IsAny<string>()));
    }

    [Theory]
    [InlineData(null, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    [InlineData("   ", typeof(ArgumentException))]
    public async Task CannotCall_DeleteMediaAsyncWithString_WithInvalid_FileName(string value, Type exceptionType)
    {
        await Assert.ThrowsAsync(exceptionType, () => _testClass.DeleteMediaAsync(value));
    }
}
