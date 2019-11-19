using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlListEntry
    {
        private SmlListEntry(string a_obis, UInt64? a_status, SmlTime a_valTime, uint? a_unit, int? a_scaler, SmlBase a_valueNode, SmlBase a_smlSignature)
        {
            Obis = a_obis;
            Status = a_status;
            ValTime = a_valTime;
            Unit = a_unit;
            Scaler = a_scaler;
            ValueNode = a_valueNode;
            SmlSignature = a_smlSignature;

            Value = null;
            double factor = 1;
            if (a_scaler.HasValue)
                factor = Math.Pow(10, a_scaler.Value);
            switch(a_valueNode.SmlFieldType)
            {
                case SmlFieldType.Signed8:
                    Value = (double)((a_valueNode as SmlSigned8).Value) * factor;
                    break;
                case SmlFieldType.Signed16:
                    Value = (double)((a_valueNode as SmlSigned16).Value) * factor;
                    break;
                case SmlFieldType.Signed32:
                    Value = (double)((a_valueNode as SmlSigned32).Value) * factor;
                    break;
                case SmlFieldType.Signed64:
                    Value = (double)((a_valueNode as SmlSigned64).Value) * factor;
                    break;
                case SmlFieldType.Unsigned8:
                    Value = (double)((a_valueNode as SmlUnsigned8).Value) * factor;
                    break;
                case SmlFieldType.Unsigned16:
                    Value = (double)((a_valueNode as SmlUnsigned16).Value) * factor;
                    break;
                case SmlFieldType.Unsigned32:
                    Value = (double)((a_valueNode as SmlUnsigned32).Value) * factor;
                    break;
                case SmlFieldType.Unsigned64:
                    Value = (double)((a_valueNode as SmlUnsigned64).Value) * factor;
                    break;
            }
        }

        public string Obis { get; private set; }
        public UInt64? Status { get; private set; }
        public SmlTime ValTime { get; private set; }

        /// <summary>
        /// Zahlenwerte siehe DLMS-Unit-List, zu finden beispielsweise in IEC 62056-62.
        /// </summary>
        public uint? Unit { get; private set; }
        /// <summary>
        /// Zahlenwert = SML_Value x 10^scaler
        /// </summary>
        public int? Scaler { get; private set; }
        public SmlBase ValueNode { get; private set; }
        public double? Value { get; private set; }
        public SmlBase SmlSignature { get; private set; }


        public static SmlListEntry Create(SmlBase a_baseNode)
        {
            var list = a_baseNode as SmlList;
            if (list == null || list.Length != 7)
                return null;

            var objName = list.GetElement(0) as SmlString;
            if (objName == null)
                return null;
            var obis = GenerateObis(objName);
            if (obis == null)
                return null;

            UInt64? status = null;
            switch(list.GetElement(1).SmlFieldType)
            {
                case SmlFieldType.Optional:
                    break;

                case SmlFieldType.Unsigned8:
                    status = (list.GetElement(1) as SmlUnsigned8).Value;
                    break;

                case SmlFieldType.Unsigned16:
                    status = (list.GetElement(1) as SmlUnsigned16).Value;
                    break;

                case SmlFieldType.Unsigned32:
                    status = (list.GetElement(1) as SmlUnsigned32).Value;
                    break;

                case SmlFieldType.Unsigned64:
                    status = (list.GetElement(1) as SmlUnsigned64).Value;
                    break;

                default:
                    return null;
            }

            SmlTime valTime = null;
            if (!list.GetElement(2).IsOptional)
            {
                valTime = SmlTime.Create(list.GetElement(2));
                if (valTime == null)
                    return null;
            }

            uint? unit = (list.GetElement(3) as SmlUnsigned8)?.Value;
            if (list.GetElement(3).IsOptional)
                unit = null;

            int? scaler = (list.GetElement(4) as SmlSigned8)?.Value;
            if (scaler == null && !list.GetElement(4).IsOptional)
                return null;

            var value = list.GetElement(5);
            if (list.GetElement(5).IsOptional)
                return null;

            var smlSignature = list.GetElement(6);
            if (list.GetElement(6).IsOptional)
                smlSignature = null;

            return new SmlListEntry(obis, status, valTime, unit, scaler, value, smlSignature);
        }

        private static string GenerateObis(SmlString a_string)
        {
            if (a_string.ValueBytes.Length != 6)
                return null;

            return $"{a_string.ValueBytes[0]}-{a_string.ValueBytes[1]}:{a_string.ValueBytes[2]}.{a_string.ValueBytes[3]}.{a_string.ValueBytes[4]}*{a_string.ValueBytes[5]}";
        }
    }
}
