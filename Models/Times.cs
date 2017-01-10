using SQLite;

namespace Front_End.Models
{
    // Local Database table model.
    public class Times
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int Time { get; set; }

        public override string ToString()
        {
			return string.Format("[Times: ID={0}, Time={1}]", ID, Time);
        }
    }
}

