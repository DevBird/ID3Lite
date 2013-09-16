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

        public enum Revision
        {
            Rev0,
            Rev1
        }

        public enum Type
        {
            Title,
            Artist,
            Album,
            Year,
            Comment,
            Track,
            Genre,
        }
        private string filePath;
        public Version1(string FilePath)
        {
            filePath = FilePath;
        }

        public v1Data Read()
        {
            v1Data tagData = new v1Data();
            using (FileStream fs = File.OpenRead(filePath))
            {
                Byte[] versionChecker = new byte[1];

                fs.Seek(-3, SeekOrigin.End);

                fs.Read(versionChecker, 0, 1);
                fs.Seek(-128, SeekOrigin.End);

                dynamic tag;

                Console.WriteLine(versionChecker[0].ToString());
                if (versionChecker[0] != 0x00)
                {
                    tag = new v1TagData();

                }
                else
                {
                    tag = new v11TagData();
                }
                Console.WriteLine(tag.GetType().ToString());
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
                        tagData.Track = Encoding.Default.GetString(tag.Track);
                    }
                    fs.Read(tag.Genre, 0, tag.Genre.Length);
                    
                    tagData.Title = Encoding.Default.GetString(RemoveNullBits(tag.Title));
                    tagData.Artist = Encoding.Default.GetString(RemoveNullBits(tag.Artist));
                    tagData.Album = Encoding.Default.GetString(RemoveNullBits(tag.Album));
                    tagData.Year = Encoding.Default.GetString(RemoveNullBits(tag.Year));
                    tagData.Comment = Encoding.Default.GetString(RemoveNullBits(tag.Comment));
                    tagData.Genre = Encoding.Default.GetString(tag.Genre);
                }

                //return tagData;
            }
            return tagData;
        }



        public bool Write(Revision Revision, Type DataType, string Value)
        {
            bool result = true;
            try
            {
                using (FileStream fs = File.OpenRead(filePath))
                {
                    byte[] data = Encoding.UTF8.GetBytes(Value);
                    int offset = getStartOffset(Revision, DataType);
                    int length = getDataSize(Revision, DataType);

                    if (Revision == Revision.Rev1 && DataType == Type.Track)
                    {
                        byte[] Separator = { 0x00 };
                        fs.Write(Separator, offset - 1, 1);
                    }

                    fs.Write(data, offset, length);
                }
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

        private int getStartOffset(Revision Rev, Type DataType)
        {
            int offset = -127;
            if (DataType == Type.Title)
                offset += 3;

            else if (DataType == Type.Artist)
                offset += 33;

            else if (DataType == Type.Album)
                offset += 63;

            else if (DataType == Type.Year)
                offset += 93;

            else if (DataType == Type.Comment)
                offset += 97;

            else if (Rev == Revision.Rev1 && DataType == Type.Track)
                offset += 126;

            else if (DataType == Type.Genre)
                offset += 127;

            return offset;
        }

        private int getDataSize(Revision Rev, Type DataType)
        {
            int length = 0;

            if (DataType == Type.Title ||
                DataType == Type.Artist ||
                DataType == Type.Album ||
                (Rev == Revision.Rev0 && DataType == Type.Comment))
                length = 30;
            else if (DataType == Type.Year)
                length = 4;
            else if (Rev == Revision.Rev1 && DataType == Type.Comment)
                length = 28;
            else if ((Rev == Revision.Rev1 && DataType == Type.Track) || DataType == Type.Genre)
                length = 1;

            return length;
        }
    }
}