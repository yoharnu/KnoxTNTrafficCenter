using FluentAssertions;
using KnoxTrafficCenter.Models;
using System.Collections.Generic;
using Xunit;

namespace KnoxTrafficCenter.Tests.Models;

public class CameraGroupTests
{
    [Fact]
    public void Constructor_SetsTitle()
    {
        // Arrange
        var title = "I-40 Eastbound";

        // Act
        var group = new CameraGroup(title);

        // Assert
        group.Title.Should().Be(title);
    }

    [Fact]
    public void Add_AddsCameraToList()
    {
        // Arrange
        var group = new CameraGroup("Test Group");
        var camera = new Camera { Id = 1, Title = "Camera 1" };

        // Act
        group.Add(camera);

        // Assert
        group.Cameras.Should().ContainSingle()
            .Which.Should().Be(camera);
    }

    [Fact]
    public void Add_IgnoresDuplicateCamera()
    {
        // Arrange
        var group = new CameraGroup("Test Group");
        var camera1 = new Camera { Id = 1, Title = "Camera 1" };
        var camera2 = new Camera { Id = 1, Title = "Camera 1 Duplicate" };

        // Act
        group.Add(camera1);
        group.Add(camera2);

        // Assert
        group.Cameras.Should().ContainSingle()
            .Which.Id.Should().Be(1);
        group.Cameras.Should().Contain(camera1);
        group.Cameras.Should().NotContain(camera2);
    }

    [Fact]
    public void AddRange_AddsMultipleCameras()
    {
        // Arrange
        var group = new CameraGroup("Test Group");
        var cameras = new List<Camera>
        {
            new Camera { Id = 1, Title = "Camera 1" },
            new Camera { Id = 2, Title = "Camera 2" }
        };

        // Act
        group.AddRange(cameras);

        // Assert
        group.Cameras.Should().HaveCount(2);
        group.Cameras.Should().BeEquivalentTo(cameras);
    }

    [Fact]
    public void AddRange_HandlesDuplicates()
    {
        // Arrange
        var group = new CameraGroup("Test Group");
        var camera1 = new Camera { Id = 1, Title = "Camera 1" };
        var cameras = new List<Camera>
        {
            camera1,
            new Camera { Id = 1, Title = "Camera 1 Duplicate" },
            new Camera { Id = 2, Title = "Camera 2" }
        };

        // Act
        group.AddRange(cameras);

        // Assert
        group.Cameras.Should().HaveCount(2);
        group.Cameras.Any(c => c.Title == "Camera 1 Duplicate").Should().BeFalse();
    }

    [Fact]
    public void ToString_ReturnsValidJson()
    {
        // Arrange
        var group = new CameraGroup("JSON Test");
        var camera = new Camera { Id = 1, Title = "Camera 1" };
        group.Add(camera);

        // Act
        var json = group.ToString();

        // Assert
        json.Should().Contain("\"Title\":\"JSON Test\"");
        json.Should().Contain("\"Cameras\":");
        json.Should().Contain("\"Id\":1");
    }
}
