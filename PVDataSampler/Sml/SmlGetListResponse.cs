using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlGetListResponse : SmlMessageBody
    {
        private SmlGetListResponse(SmlString a_clientId, SmlString a_serverId, SmlString a_listName, SmlTime a_actSensorTime, List<SmlListEntry> a_values, SmlBase a_smlSignature, SmlTime a_actGatewayTime)
        {
            ClientId = a_clientId;
            ServerId = a_serverId;
            ListName = a_listName;
            ActSensorTime = a_actSensorTime;
            Values = a_values;
            SmlSignature = a_smlSignature;
            ActGatewayTime = a_actGatewayTime;
        }

        public SmlString ClientId { get; private set; }
        public SmlString ServerId { get; private set; }
        public SmlString ListName { get; private set; }
        public SmlTime ActSensorTime { get; private set; }
        public List<SmlListEntry> Values { get; private set; }
        public SmlBase SmlSignature { get; private set; }
        public SmlTime ActGatewayTime { get; private set; }


        internal static SmlMessageBody Create(SmlBase a_baseNode)
        {
            var list = a_baseNode as SmlList;
            if (list == null || list.Length != 7)
                return null;

            var clientId = list.GetElement(0) as SmlString;
            if (clientId == null && !list.GetElement(0).IsOptional)
                return null;

            var serverId = list.GetElement(1) as SmlString;
            if (serverId == null)
                return null;

            var listName = list.GetElement(2) as SmlString;
            if (listName == null && !list.GetElement(2).IsOptional)
                return null;

            SmlTime actSensorTime = null;
            if (!list.GetElement(3).IsOptional)
            {
                actSensorTime = SmlTime.Create(list.GetElement(3));
                if (actSensorTime == null)
                    return null;
            }

            var valListNode = list.GetElement(4) as SmlList;
            if (valListNode == null)
                return null;

            var valList = new List<SmlListEntry>(valListNode.Length);
            for (int i = 0; i < valListNode.Length; i++)
            {
                var listEntry = SmlListEntry.Create(valListNode.GetElement(i));
                if (listEntry == null)
                    return null;
                valList.Add(listEntry);
            }

            var smlSignature = list.GetElement(5);

            SmlTime actGatewayTime = null;
            if (!list.GetElement(6).IsOptional)
            {
                actGatewayTime = SmlTime.Create(list.GetElement(6));
                if (actGatewayTime == null)
                    return null;
            }

            return new SmlGetListResponse(clientId, serverId, listName, actSensorTime, valList, smlSignature, actGatewayTime);
        }

    }
}
