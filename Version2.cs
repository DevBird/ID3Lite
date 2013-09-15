using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows
namespace ID3Lite
{
    public class Version2
    {

    public TagData Read(string FilePath)
        {
            TagData tagData = new TagData();
            using (FileStream fs = File.Open(FilePath, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    ulong readSize = 0;
                    byte[] tag = new byte[3];
                    byte[] version = new byte[2];
                    byte[] flags = new byte[1];
                    byte[] size = new byte[4];

                    byte[] frameId = new byte[4];
                    byte[] frameSize = new byte[4];
                    byte[] dump = new byte[3];

                    br.Read(tag, 0, tag.Length);
                    br.Read(version, 0, version.Length);
                    br.Read(flags, 0, flags.Length);
                    br.Read(size, 0, size.Length);

                    ulong totalSize = (ulong)size[0] << 24 | (ulong)size[1] << 16 | (ulong)size[2] << 8 | (ulong)size[3];


                    //MessageBox.Show(version[0].ToString());
                    while (readSize < totalSize)
                    {


                        br.Read(frameId, 0, frameId.Length);
                        br.Read(frameSize, 0, frameSize.Length);
                        //br.Read(dump, 0, dump.Length);
                        //br.Read();
                        //br.Read();
                        ulong iSize = (ulong)frameSize[0] << 24 | (ulong)frameSize[1] << 16 | (ulong)frameSize[2] << 8 | (ulong)frameSize[3];

                        if (iSize.ToString() == "0") break;
                        if (iSize >= totalSize) break;
                        readSize += iSize;
                        //System.Windows.MessageBox.Show(iSize.ToString());
                        string frameName = ConvertHexToString(BitConverter.ToString(frameId));
                        try
                        {
                            //MessageBox.Show(frameName);
                            if (ConvertHexToString(BitConverter.ToString(frameId)) != "APIC")
                            {

                                string data;

                                // 첫 시작이 FF라면 두번째 바이트인 FE를 00으로 바꿔주고 마지막의 00 문자를 읽지 않기 위해 최대 길이를 -1해서 읽어준다.
                                // 이럴 경우에는 UTF-16으로 인코딩 된 경우
                                br.Read(dump, 0, dump.Length);
                                byte[] body = new byte[iSize - 1];
                                br.Read(body, 0, body.Length);
                                if (body[0] == 0xff && body[1] == 0xfe) //UTF-16 Big Endian
                                {
                                    if (body[4] == 0 && body[body.Length - 2] == 0)
                                    {
                                        body = body.Where(b => b != 0xff).ToArray();
                                        Array.Resize(ref body, body.Length - 1);
                                        if (body[0] == 0xfe) body[0] = 0;
                                        data = Encoding.BigEndianUnicode.GetString(body);
                                        //System.Windows.MessageBox.Show("1\n" + data);
                                    }
                                    else
                                    {

                                        body = body.Where(b => b != 0xff && b != 0xfe).ToArray();
                                        data = Encoding.Unicode.GetString(body);
                                        //System.Windows.MessageBox.Show("2\n" + data);
                                    }
                                }
                                else if (body[0] == 0xfe && body[1] == 0xff) //UTF-16 Little Endian
                                {
                                    body = body.Where(b => b != 0xff).ToArray();
                                    Array.Resize(ref body, body.Length - 1);
                                    if (body[0] == 0xfe) body[0] = 0;
                                    data = Encoding.GetEncoding("UTF-16LE").GetString(body);
                                }
                                else
                                {
                                    data = Encoding.Default.GetString(body);

                                }

                                if (frameName == "TIT2") tagData.Title = data;
                                if (frameName == "TPE1") tagData.Artist = data;
                                if (frameName == "TPE2") tagData.Artist2 = data;
                                if (frameName == "TALB") tagData.Album = data;

                            }


                            else
                            {
                                br.Read(dump, 0, 2);
                                byte[] EncodingType = new byte[13]; // image/jpeg
                                br.Read(EncodingType, 0, EncodingType.Length);
                                if (EncodingType[12] != 0x00) br.ReadByte(); // image/확장자 00 플래그 00 이렇게 되는데 00으로 끝나지 않는다면 한 바이트 더 있는것이므로 한 바이트 더 읽어준다.
                                //MessageBox.Show((BitConverter.ToString(EncodingType)));


                                byte[] body = new byte[Convert.ToInt32(iSize)];
                                br.Read(body, 0, body.Length);
                                tagData.Cover = body;
                                br.Read(EncodingType, 0, 12);
                            }
                        }
                        catch
                        {
                        }
                    }
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

        string ConvertHexToString(string HexValue)
        {


            string StrValue = "";
            HexValue = HexValue.Replace("-", "");
            while (HexValue.Length > 0)
            {
                StrValue += Convert.ToChar(Convert.ToUInt32(HexValue.Substring(0, 2), 16)).ToString();
                HexValue = HexValue.Substring(2, HexValue.Length - 2);
            }
            return StrValue;
        }

       
    }
}