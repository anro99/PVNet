using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlTime
    {
        private bool m_isSecIndex = false;
        private DateTime m_time;
        private TimeSpan m_secIndex;


        private SmlTime(TimeSpan a_secIndex)
        {
            m_isSecIndex = true;
            m_secIndex = a_secIndex;
        }

        private SmlTime(DateTime a_time)
        {
            m_isSecIndex = false;
            m_time = a_time;
        }


        public static SmlTime Create(SmlBase a_baseNode)
        {
            var list = a_baseNode as SmlList;
            if (list == null || list.Length != 2)
                return null;

            var choice = list.GetElement(0) as SmlUnsigned8;
            if (choice == null)
                return null;
            switch(choice.Value)
            {
                case 0x00:
                    var secIndex = list.GetElement(1) as SmlUnsigned32;
                    if (secIndex == null)
                        return null;
                    return new SmlTime(TimeSpan.FromSeconds(secIndex.Value));
                case 0x01:
                    var secUnixUtc = list.GetElement(1) as SmlUnsigned32;
                    if (secUnixUtc == null)
                        return null;
                    return new SmlTime(new DateTime(1970, 1, 1).AddSeconds(secUnixUtc.Value));
                case 0x02:
                    var localTime = list.GetElement(1) as SmlList;
                    if (localTime == null || localTime.Length != 3)
                        return null;
                    var secUnixLocal = localTime.GetElement(0) as SmlUnsigned32;
                    var localOffsetMins = localTime.GetElement(1) as SmlSigned16;
                    var seasonOffsetMins = localTime.GetElement(2) as SmlSigned16;
                    if (secUnixLocal == null || localOffsetMins == null || seasonOffsetMins == null)
                        return null;
                    var date = new DateTime(1970, 1, 1).AddSeconds(secUnixLocal.Value);
                    date = date.AddMinutes(localOffsetMins.Value + seasonOffsetMins.Value);
                    return new SmlTime(date);
                default:
                    return null;
            }
        }

        public bool IsSecondIndex => m_isSecIndex;

        public DateTime Time => m_time;

        public TimeSpan SecondIndex => m_secIndex;
    }
}
