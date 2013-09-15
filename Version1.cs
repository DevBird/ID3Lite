using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3Lite
{
    public class Version1
    {
        public TagData Read(string FilePath)
        {
            TagData tagData = new TagData();
            using (FileStream fs = File.OpenRead(FilePath))
            {
                if (fs.Length >= 128)
                {
                    Byte[] versionChecker = new byte[1];

                    fs.Seek(-125, SeekOrigin.End);

                    fs.Read(versionChecker, 0, 1);
                    fs.Seek(-128, SeekOrigin.End);

                    dynamic tag;


                    if (versionChecker[0] != 0x00)
                    {
                        tag = new v1TagData();

                    }
                    else
                    {
                        tag = new v11TagData();
                    }

                    fs.Read(tag.Header, 0, tag.Header.Length);
                    string theTAGID = Encoding.Default.GetString(tag.Header);
                    if (theTAGID.Equals("TAG"))
                    {
                        fs.Read(tag.Title, 0, tag.Title.Length);
                        fs.Read(tag.Artist, 0, tag.Artist.Length);
                        fs.Read(tag.Album, 0, tag.Album.Length);
                        fs.Read(tag.Year, 0, tag.Year.Length);
                        fs.Read(tag.Comment, 0, tag.Comment.Length);
                        if (tag.GetType() == typeof(v11TagData))
                        {
                            fs.Read(tag.Separator, 0, tag.Separator.Length);
                            fs.Read(tag.Track, 0, tag.Track.Length);
                        }
                        fs.Read(tag.Genre, 0, tag.Genre.Length);

                        tagData.Title = Encoding.Default.GetString(RemoveNullBits(tag.Title));
                        tagData.Artist = Encoding.Default.GetString(RemoveNullBits(tag.Artist));
                        tagData.Album = Encoding.Default.GetString(RemoveNullBits(tag.Album));
                        tagData.Year = Encoding.Default.GetString(RemoveNullBits(tag.Year));

                    }

                    return tagData;
                }
            }
            return tagData;
        }


        public bool Write()
        {
            bool result = true;
            try
            {

            }
            catch
            {
                result = false;
            }

            return result;
        }

        private byte[] RemoveNullBits(byte[] source)
        {
            int i = source.Length - 1;
            while (source[i] == 0)
            {
                --i;
            }

            byte[] tmp = new byte[i + 1];
            Array.Copy(source, tmp, i + 1);

            return tmp;
        }


    }
}