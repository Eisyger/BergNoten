using SQLite;
using System.Reflection;

namespace BergNoten.Model
{
    public class Grade : TableData
    {
        #region Properties  
        [Indexed]
        public int ID_Participant { get; set; }

        [Indexed]
        public int ID_Exam { get; set; }

        [MaxLength(10)]
        public string Note { get; set; }

        [MaxLength(255)]
        public string Bemerkung { get; set; }

        [Ignore]
        public Participant Participant { get; set; }

        [Ignore]
        public Exam Exam { get; set; }
        #endregion

        /// <summary>
        /// Diesen Konstruktor nicht verwenden!
        /// </summary>
        public Grade()
        {
            ID_Participant = 0;
            ID_Exam = 0;
            Note = "-";
            Bemerkung = string.Empty;
            Participant = null;
            Exam = null;
        }

        public Grade(Participant participant, Exam exam, string grade = "-", string bemerkung = "")
        {
            Note = grade;
            Bemerkung = bemerkung;

            Participant = participant;
            Exam = exam;

            ID_Exam = Exam.ID;
            ID_Participant = Participant.ID;
        }

        public override List<PropertyInfo> GetProperties()
        {
            return [.. typeof(Grade).GetProperties()];
        }
    }
}
