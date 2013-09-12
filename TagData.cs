namespace ID3Lite
{
    public class TagData
    {
        public string Title;
        public string Artist;
        public string Artist2;
        public string Album;
        public string Year;
        public string Comment;
        public int Genre;
        public byte[] Cover;

    }


    public class v1TagData
    {
        public byte[] Header = new byte[3];
        public byte[] Title = new byte[30];
        public byte[] Artist = new byte[30];
        public byte[] Album = new byte[30];
        public byte[] Year = new byte[4];
        public byte[] Comment = new byte[30];
        public byte[] Genre = new byte[1];
    }

    public class v11TagData
    {
        public byte[] Header = new byte[3];
        public byte[] Title = new byte[30];
        public byte[] Artist = new byte[30];
        public byte[] Album = new byte[30];
        public byte[] Year = new byte[4];
        public byte[] Comment = new byte[28];
        public byte[] Separator = new byte[1]; //always should be 0
        public byte[] Track = new byte[1];
        public byte[] Genre = new byte[1];
    }
}
