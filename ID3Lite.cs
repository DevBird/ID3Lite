using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ID3Lite
{
    public class ID3Lite
    {

        Dictionary<string, byte[]> frames;
        string filePath;

        /* internal variables for CoverArt */
        static bool _isExist = false;
        static string _MIME = String.Empty;
        static PictureType _PictureType = 0;
        static string _Description = String.Empty;
        static byte[] _Image = null;

        struct CoverArt
        {
            bool IsExist {
                get { return _isExist; }
            }

            string MIME {
                get { return _MIME; }
            }

            PictureType PictureType {
                get { return _PictureType; }
            }
            string Description {
                get { return _Description; }
            }

            byte[] Image{
                get { return _Image; }
            }
        }

        #region Enums
        private enum Flag
        {
            TagAlterPreservation = 0x8000,
            FileAlterPreservation = 0x4000,
            ReadOnly = 0x2000,
            Compression = 0x0080,
            Encryption = 0x0040,
            GroupingIdentity = 0x0020
        }

        private enum PictureType
        {
            Other = 0x00,
            PNGFileIcon = 0x01,
            OtherFileIcon = 0x02,
            FrontCover = 0x03,
            BackCover = 0x04,
            LeafletPage = 0x05,
            Media = 0x06,
            LeadArtist = 0x07,
            Artist = 0x08,
            Conductor = 0x09,
            Orchestra = 0x0A,
            Composer = 0x0B,
            Lyricist = 0x0C,
            RecordingLocation = 0x0D,
            DuringRecording = 0x0E,
            DuringPerformance = 0x0F,
            VideoSnapshot = 0x10,
            BrightColouredFish = 0x11,
            Illustration = 0x12,
            ArtistLogotype = 0x13,
            PublisherLogotype = 0x14
        }
        #endregion

        public ID3Lite(string _filePath)
        {
            filePath = _filePath;
            frames = new Dictionary<string, byte[]>();
            using (FileStream fs = File.Open(filePath, FileMode.Open))
            {
                int frameSizeInt;

                byte[] tag = new byte[3];
                byte[] version = new byte[2];
                byte[] flags = new byte[1];
                byte[] size = new byte[4];

                byte[] frameId = new byte[4];
                byte[] frameSize = new byte[4];
                byte[] dump = new byte[2];

                Flag frameFlag;

                fs.Read(tag, 0, tag.Length);
                fs.Read(version, 0, version.Length);
                fs.Read(flags, 0, flags.Length);
                fs.Read(size, 0, size.Length);

                if (version[0] == 2)
                {
                    throw new Exception("ID3v2.2 is not supported");
                }

                ulong totalSize = (ulong)(size[0] * 0x200000 + size[1] * 0x4000 + size[2] * 0x80 + size[3]);
                byte buff;

                while (totalSize > 10)
                {
                    buff = (byte)fs.ReadByte();

                    //Check Next Byte is Padding
                    if (buff == 0)
                    {
                        totalSize--;
                        continue;
                    }

                    fs.Seek(-1, SeekOrigin.Current);
                    fs.Read(frameId, 0, frameId.Length);

                    fs.Read(frameSize, 0, frameSize.Length);

                    frameFlag = (Flag)fs.Read(dump, 0, 2);

                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(frameSize);

                    frameSizeInt = BitConverter.ToInt32(frameSize, 0);

                    //Skip Text Encoding Type Flag
                    buff = (byte)fs.ReadByte();

                    if (buff == 0)
                        frameSizeInt--;
                    else
                        fs.Seek(-1, SeekOrigin.Current);

                    byte[] data = new byte[frameSizeInt];
                    fs.Read(data, 0, data.Length);

                    frames.Add(Encoding.UTF8.GetString(frameId), data);

                    totalSize -= (ulong)frameSizeInt + 10;
                }

                fs.Close();
            }

            //parse cover art data;
            if (frames.ContainsKey("APIC"))
            {
                _isExist = true;

                byte[] data = frames["APIC"];
                int i;

                //get mime
                for (i = 0; data[i] != '\0'; i++)
                {
                    _MIME += System.Text.Encoding.UTF8.GetString(new[] { data[i] }); ;
                }

                //get picture type
                _PictureType = (PictureType)Convert.ToInt32(data[++i].ToString(), 16);

                //get image description
                for (; data[i] != '\0'; i++)
                {
                    _Description += System.Text.Encoding.UTF8.GetString(new[] { data[i] }); ;
                }
                //++i for skip encoding type flag
                _Image = new byte[data.Length - ++i];
                Buffer.BlockCopy(data, i, _Image, 0, _Image.Length);
            }
        }
        
        public void SetFrameText(string FrameName, byte[] FrameData){
            frames[FrameName] = FrameData;
        }

        public void Save()
        {
            byte[] id3Header;
            int fullLength = 10;
            int i;

            for (i = 0; i < frames.Count; i++) {
                KeyValuePair<string, byte[]> pair = frames.Skip(i).First();
                fullLength += 10 + pair.Value.Length;
            }
            
            id3Header = new byte[4]{ 73, 68, 51, 4 };

            Array.Resize(ref id3Header, fullLength);

            for (i = 9; i >= 6; i--)
            {
                id3Header[i] = Convert.ToByte(fullLength % 0x80);
                fullLength /= 0x80;
            }

            
            try
            {
                using (FileStream fs = File.Open(filePath, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                
                    }
                }
            }
            catch
            {
                //catch
            }
        }

        public string GetFrameData(string frameName)
        {
            return Encoding.UTF8.GetString(frames[frameName]);
        }

        public byte[] GetFrameByteData(string frameName)
        {
            return frames[frameName];
        }

        #region Basic Infromation Getter
        public string Title {
            get { return GetFrameData("TIT2"); }
        }

        public string Album
        {
            get { return GetFrameData("TALB"); }
        }

        public string Artist
        {
            get { return GetFrameData("TPE1"); }
        }

        public string Comment
        {
            get { return GetFrameData("COMM"); }
        }

        public int Track
        {
            get { return Convert.ToInt32(GetFrameData("TRCK")); }
        }

        public int ReleaseYear
        {
            get { return Convert.ToInt32(GetFrameData("TYER")); }
        }
        #endregion

    }
}