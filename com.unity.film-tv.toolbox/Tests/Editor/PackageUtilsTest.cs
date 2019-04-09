using System.IO;
using UnityEngine;
using NUnit.Framework;
using UnityEditor.FilmTV.Toolbox;


class PackageUtilsTest
{
    [Test, Description("Test getting a relative path from a base with no trailing slash")]
    public void TestRelativeFolderPathNoTrailingSlash()
    {
        var res = PackageUtils.GetRelativeFolderPath("/path/to/folder", "/path/to/folder/subfolder/file.ext");
        Assert.That(res == "subfolder");
    }

    [Test, Description("Test getting a relative path from a base with a trailing slash")]
    public void TestRelativeFolderPathTrailingSlash()
    {
        var res = PackageUtils.GetRelativeFolderPath("/path/to/folder/", "/path/to/folder/subfolder/file.ext");
        Assert.That(res == "subfolder");
    }

    [Test, Description("Test getting a relative Windows path from a base with a trailing slash")]
    public void TestRelativeFolderPathTrailingSlashWindows()
    {
        var res = PackageUtils.GetRelativeFolderPath("C:\\path\\to\\folder\\", "C:\\path\\to\\folder\\subfolder\\file.ext");
        Assert.That(res == "subfolder");
    }
    
    [Test, Description("Test Unrelated paths")]
    public void TestUnrelatedPaths()
    {
        var res = PackageUtils.GetRelativeFolderPath("/another/path/to/folder/", "/path/to/folder/subfolder/file.ext");
        Assert.That(res == "../../../../path/to/folder/subfolder");
    }

    [Test, Description("Test Unrelated paths on Windows")]
    public void TestUnrelatedPathsWindows()
    {
        const string folderPath = "D:\\path\\to\\folder\\subfolder";
        var res = PackageUtils.GetRelativeFolderPath("C:\\path\\to\\folder\\", folderPath + "\\file.ext");
        Assert.That(res == folderPath.Replace('\\', Path.DirectorySeparatorChar));
    }
}
