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

 Version1 Reader = new Version1();
 // If you want to read id3 v2.x, use Version2 instead of Version1
 // Version2 Reader = new Version2();
 
 TagData ID3Tags  = Reader.Read("");
 SomeNiceStuff = ID3Tags.Artist;

...

```
-------
###Writing
Not Supported Now

-------
###Gettable Data
#####TagData.
* Album (Album Title)
* Title (Song Title) 
* Artist (Artist - TPE1 Tag)
* Artist2 (Artist - TPE2 Tag)
* Cover (Album Cover Image Data in Byte Array)
* and.. more

License
-------
ID3Lite follows MIT License


 

