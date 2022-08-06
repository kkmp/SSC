using System.Text;

namespace SSC.Tools
{
    public interface ICSV
    {
        string Header();
        string ToCSV();

        public static string CreateCSV(List<ICSV> objects)
        {
            if(objects.Count == 0)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(objects[0].Header());
            foreach(ICSV obj in objects)
            {
                sb.AppendLine(obj.ToCSV());
            }
            return sb.ToString();
        }
    }
}
