ID3Lite
=======

Simple ID3 Tag Readable/Writeable .NET Library // Written in C#

Requires
--------


.NET Framework 4.5 or Higher

How to Use
-------
###Reading
```csharp
using ID3Lite;

...

 Version1 ID3 = new Version1("File Path Goes Here");
 v1Data Tags  = ID3.Read();

 // If you want to read id3 v2.x, use Version2 instead of Version1
 // Version2 ID3 = new Version2("File Path Goes Here");
 // v2Data Tags = ID3.Read(); 

 SomeNiceStuff = ID3Tags.Artist;

...

```
-------
###Writing

####ID3 1.x Writing

```csharp
using ID3Lite;

...

 Version1 ID3 = new Version1("File Path Goes Here");
   
 ID3.Write(Revision.Rev1, DataType.Title, "SONG TITLE");
 ID3.Write(Revision.Rev1, DataType.Album, "> Album Title Goes Here <");
 ID3.Write(Revision.Rev1, DataType.Artist, "dat artist");
 ID3.Write(Revision.Rev1, DataType.Comment, "nice stuff. bro");
 ID3.Write(Revision.Rev1, DataType.Year, "2013");
 ID3.Write(Revision.Rev1, DataType.Track, "4");
 // can't Genre data writing in current version.

```

#### how to check the results
```csharp
if(ID3.Write(Revision.Rev1, DataType.Title, "SONG TITLE") == true)
{
    Console.WriteLine("Success! yayXD");
}
else
{
    Console.WriteLine("FAILED D:");
}
```

-------
###Gettable Data
#####v1Data. 
* Title
* Artist
* Album
* Year
* Comment
* Track (Only in v1.1)
* Genre


#####v2Data.
* Album (Album Title)
* Title (Song Title) 
* Artist (Artist - TPE1 Tag)
* Artist2 (Artist - TPE2 Tag)
* Cover (Album Cover Image Data in Byte Array)
* and.. more

License
-------
ID3Lite follows MIT License