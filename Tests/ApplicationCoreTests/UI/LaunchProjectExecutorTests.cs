using System.Diagnostics;
using UI.Shared;
using Xunit;

namespace ApplicationCoreTests.UI
{
    public class LaunchProjectExecutorTests : IDisposable
    {
        private readonly string _testFilePath;
        private readonly string _testDirectory;

        public LaunchProjectExecutorTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "LaunchProjectExecutorTests");
            Directory.CreateDirectory(_testDirectory);
            _testFilePath = Path.Combine(_testDirectory, "test.txt");
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Fact]
        public void OpenIDEWithFileName_FileExists_ReturnsSuccess()
        {
            // Arrange
            File.WriteAllText(_testFilePath, "test content");
            string devAppPath = "notepad.exe"; // Using notepad as it's available on Windows

            // Act
            var result = LaunchProjectExecutor.OpenIDEWithFileName(_testFilePath, devAppPath);

            // Assert
            Assert.True(result.Item1);
            Assert.Equal(string.Empty, result.Item2);

            // Cleanup - kill notepad processes we created
            KillProcessesByName("notepad");
        }

        [Fact]
        public void OpenIDEWithFileName_FileDoesNotExist_ReturnsFailure()
        {
            // Arrange
            string nonExistentFile = Path.Combine(_testDirectory, "nonexistent.txt");
            string devAppPath = "notepad.exe";

            // Act
            var result = LaunchProjectExecutor.OpenIDEWithFileName(nonExistentFile, devAppPath);

            // Assert
            Assert.False(result.Item1);
            Assert.Contains("File not found:", result.Item2);
            Assert.Contains(nonExistentFile, result.Item2);
        }

        [Fact]
        public void OpenIDEWithFileName_FilePathWithSpaces_HandlesCorrectly()
        {
            // Arrange
            string fileWithSpaces = Path.Combine(_testDirectory, "file with spaces.txt");
            File.WriteAllText(fileWithSpaces, "test content");
            string devAppPath = "notepad.exe";

            // Act
            var result = LaunchProjectExecutor.OpenIDEWithFileName(fileWithSpaces, devAppPath);

            // Assert
            Assert.True(result.Item1);
            Assert.Equal(string.Empty, result.Item2);

            // Cleanup
            KillProcessesByName("notepad");
        }

        [Fact]
        public void OpenIDE_ValidProcessStartInfo_StartsProcess()
        {
            // Arrange
            File.WriteAllText(_testFilePath, "test content");
            var processInfo = new ProcessStartInfo
            {
                FileName = "notepad.exe",
                Arguments = $"\"{_testFilePath}\"",
                UseShellExecute = true,
            };

            // Act & Assert - should not throw
            LaunchProjectExecutor.OpenIDE(processInfo);

            // Give process time to start
            System.Threading.Thread.Sleep(500);

            // Verify process started by checking if notepad is running
            var notepadProcesses = Process.GetProcessesByName("notepad");
            Assert.NotEmpty(notepadProcesses);

            // Cleanup
            KillProcessesByName("notepad");
        }

        [Fact]
        public void OpenIDEWithFileName_EmptyFilePath_ReturnsFailure()
        {
            // Arrange
            string emptyPath = string.Empty;
            string devAppPath = "notepad.exe";

            // Act
            var result = LaunchProjectExecutor.OpenIDEWithFileName(emptyPath, devAppPath);

            // Assert
            Assert.False(result.Item1);
            Assert.Contains("File not found:", result.Item2);
        }

        [Fact]
        public void OpenIDEWithFileName_NullFilePath_ReturnsFailure()
        {
            // Arrange
            string? nullPath = null;
            string devAppPath = "notepad.exe";

            // Act
            var result = LaunchProjectExecutor.OpenIDEWithFileName(nullPath, devAppPath);

            // Assert
            Assert.False(result.Item1);
            Assert.Contains("File not found:", result.Item2);
        }

        private void KillProcessesByName(string processName)
        {
            try
            {
                var processes = Process.GetProcessesByName(processName);
                foreach (var process in processes)
                {
                    try
                    {
                        process.Kill();
                        process.WaitForExit(1000);
                        process.Dispose();
                    }
                    catch
                    {
                        // Ignore errors when killing test processes
                    }
                }
            }
            catch
            {
                // Ignore errors in cleanup
            }
        }
    }
}
