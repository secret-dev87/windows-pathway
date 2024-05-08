using System.Collections.Generic;
using System.IO;

namespace Pathway.Loader.Infrastructure {
    public class Header {
        #region Basic Header Info

        protected List<Indices> Index = new List<Indices>();
        protected BinaryReader Reader;
        private byte[] _systemSerialByte = new byte[10];
        private byte[] _uwsDataFormatDescriptionByte = new byte[64];
        private byte[] _uwsIdentifierByte = new byte[8];
        private byte[] _uwsKeyByte = new byte[10];
        private byte[] _uwsOsNameByte = new byte[64];
        private byte[] _uwsOsVstringByte = new byte[64];
        private byte[] _uwsSignatureByte = new byte[64];
        private byte[] _uwsSignatureTypeByte = new byte[18];
        private byte[] _uwsSystemNameByte = new byte[10];
        private byte[] _uwsTpdcVstringByte = new byte[20];
        private byte[] _uwsVstringByte = new byte[30];
        public string UWSSerialNumber { get; set; }
        public short UwsBinaryFormat { get; set; }
        public int UwsCPUCount { get; set; }
        public int UwsCpuMask { get; set; }
        public int UwsDataFormat { get; set; }
        public string UwsDataFormatDescription { get; set; }

        public short UwsHLen { get; set; }
        public short UwsHVersion { get; set; }

        public string UwsIdentifier { get; set; }
        public string UwsKey { get; set; }
        public string UwsOsName { get; set; }
        public long UwsOsVersion { get; set; }
        public string UwsOsVstring { get; set; }
        public string UwsSignature { get; set; }
        public int UwsSystemId { get; set; }
        public string UwsSystemName { get; set; }
        public int UwsSystemNumber { get; set; }
        public string UwsTpdcVstring { get; set; }

        public short UwsTpdcVstringLen { get; set; }
        public int UwsVersion { get; set; }
        public string UwsVstring { get; set; }
        public short UwsXLen { get; set; }
        public short UwsXRecords { get; set; }

        public byte[] UwsIdentifierByte {
            get { return _uwsIdentifierByte; }
            set { _uwsIdentifierByte = value; }
        }

        public byte[] UwsKeyByte {
            get { return _uwsKeyByte; }
            set { _uwsKeyByte = value; }
        }

        public byte[] SystemSerialByte {
            get { return _systemSerialByte; }
            set { _systemSerialByte = value; }
        }

        public byte[] UwsTpdcVstringByte {
            get { return _uwsTpdcVstringByte; }
            set { _uwsTpdcVstringByte = value; }
        }

        public byte[] UwsSignatureTypeByte {
            get { return _uwsSignatureTypeByte; }
            set { _uwsSignatureTypeByte = value; }
        }

        public byte[] UwsSignatureByte {
            get { return _uwsSignatureByte; }
            set { _uwsSignatureByte = value; }
        }

        public byte[] UwsDataFormatDescriptionByte {
            get { return _uwsDataFormatDescriptionByte; }
            set { _uwsDataFormatDescriptionByte = value; }
        }

        public byte[] UwsVstringByte {
            get { return _uwsVstringByte; }
            set { _uwsVstringByte = value; }
        }

        public byte[] UwsSystemNameByte {
            get { return _uwsSystemNameByte; }
            set { _uwsSystemNameByte = value; }
        }

        public byte[] UwsOsNameByte {
            get { return _uwsOsNameByte; }
            set { _uwsOsNameByte = value; }
        }

        public byte[] UwsOsVstringByte {
            get { return _uwsOsVstringByte; }
            set { _uwsOsVstringByte = value; }
        }

        #endregion

        #region Header Producer Info

        private byte[] _uwsProducerDdlVstringByte = new byte[64];
        private byte[] _uwsProducerNameByte = new byte[64];
        private byte[] _uwsProducerVstringByte = new byte[64];
        public int UwsProducer { get; set; }
        public long UwsProducerDdlVersion { get; set; }
        public string UwsProducerDdlVstring { get; set; }
        public string UwsProducerName { get; set; }
        public long UwsProducerVersion { get; set; }
        public string UwsProducerVstring { get; set; }

        public byte[] UwsProducerNameByte {
            get { return _uwsProducerNameByte; }
            set { _uwsProducerNameByte = value; }
        }

        public byte[] UwsProducerVstringByte {
            get { return _uwsProducerVstringByte; }
            set { _uwsProducerVstringByte = value; }
        }

        public byte[] UwsProducerDdlVstringByte {
            get { return _uwsProducerDdlVstringByte; }
            set { _uwsProducerDdlVstringByte = value; }
        }

        #endregion

        #region Sample Info

        public long UwsClassTotalDataSize { get; set; } //Bytes
        public long UwsGmtStartTimestamp { get; set; } //Juliantimestamp
        public long UwsGmtStopTimestamp { get; set; }
        public long UwsLctStartTimestamp { get; set; }
        public long UwsSampleInterval { get; set; } //Microseconds

        #endregion

        #region Class Info

        public string UwsClassCompatibleDataVstring = string.Empty;
        private byte[] _uwsClassCollectionStateStringByte = new byte[128];
        private byte[] _uwsClassCompatibleDataVstringByte = new byte[64];
        private byte[] _uwsClassCompatibleDdlVstringByte = new byte[64];
        private byte[] _uwsClassDataVstringByte = new byte[64];
        private byte[] _uwsClassDdlVstringByte = new byte[64];
        private byte[] _uwsClassNameByte = new byte[64]; //' "Measure"
        private byte[] _uwsClassSampleErrorStringByte = new byte[96];
        private byte[] _uwsClassSampleStateStringByte = new byte[96];
        public int UwsClassCollectionState { get; set; }
        public string UwsClassCollectionStateString { get; set; }
        public int UwsClassCompatibleDataVersion { get; set; }
        public int UwsClassCompatibleDdlVersion { get; set; }
        public string UwsClassCompatibleDdlVstring { get; set; }
        public int UwsClassDataVersion { get; set; }
        public string UwsClassDataVstring { get; set; }
        public int UwsClassDdlVersion { get; set; }
        public string UwsClassDdlVstring { get; set; }
        public int UwsClassId { get; set; }
        public string UwsClassName { get; set; }
        public int UwsClassSampleError { get; set; }
        public string UwsClassSampleErrorString { get; set; }
        public int UwsClassSampleState { get; set; }
        public string UwsClassSampleStateString { get; set; }
        public string UwsSignatureType { get; set; }

        public byte[] UwsClassNameByte {
            get { return _uwsClassNameByte; }
            set { _uwsClassNameByte = value; }
        }

        public byte[] UwsClassDdlVstringByte {
            get { return _uwsClassDdlVstringByte; }
            set { _uwsClassDdlVstringByte = value; }
        }

        public byte[] UwsClassCompatibleDdlVstringByte {
            get { return _uwsClassCompatibleDdlVstringByte; }
            set { _uwsClassCompatibleDdlVstringByte = value; }
        }

        public byte[] UwsClassDataVstringByte {
            get { return _uwsClassDataVstringByte; }
            set { _uwsClassDataVstringByte = value; }
        }

        public byte[] UwsClassCompatibleDataVstringByte {
            get { return _uwsClassCompatibleDataVstringByte; }
            set { _uwsClassCompatibleDataVstringByte = value; }
        }

        public byte[] UwsClassCollectionStateStringByte {
            get { return _uwsClassCollectionStateStringByte; }
            set { _uwsClassCollectionStateStringByte = value; }
        }

        public byte[] UwsClassSampleStateStringByte {
            get { return _uwsClassSampleStateStringByte; }
            set { _uwsClassSampleStateStringByte = value; }
        }

        public byte[] UwsClassSampleErrorStringByte {
            get { return _uwsClassSampleErrorStringByte; }
            set { _uwsClassSampleErrorStringByte = value; }
        }

        #endregion

        #region Collector Data Class Info

        private byte[] _uwsCdataClassNameByte = new byte[64]; //         ' "Tpdc" or "TPDC"
        private byte[] _uwsCdataCollectionStateStringByte = new byte[96];
        private byte[] _uwsCdataCompatibleDataVstringByte = new byte[64];
        private byte[] _uwsCdataCompatibleDdlVstringByte = new byte[64];
        private byte[] _uwsCdataDataVstringByte = new byte[64];
        private byte[] _uwsCdataDdlVstringByte = new byte[64];
        private byte[] _uwsCdataSampleErrorStringByte = new byte[96];
        private byte[] _uwsCdataSampleStateStringByte = new byte[128];
        public int UwsCdataClassId { get; set; }
        public string UwsCdataClassName { get; set; }
        public int UwsCdataCollectionState { get; set; }
        public string UwsCdataCollectionStateString { get; set; }
        public int UwsCdataCompatibleDataVersion { get; set; }
        public string UwsCdataCompatibleDataVstring { get; set; }
        public int UwsCdataCompatibleDdlVersion { get; set; }
        public string UwsCdataCompatibleDdlVstring { get; set; }
        public int UwsCdataDataVersion { get; set; }
        public int UwsCdataDdlVersion { get; set; }
        public string UwsCdataDdlVstring { get; set; }
        public int UwsCdataSampleError { get; set; }
        public string UwsCdataSampleErrorString { get; set; }
        public int UwsCdataSampleState { get; set; }
        public string UwsCdataSampleStateString { get; set; }

        public byte[] UwsCdataClassNameByte {
            get { return _uwsCdataClassNameByte; }
            set { _uwsCdataClassNameByte = value; }
        }

        public byte[] UwsCdataDdlVstringByte {
            get { return _uwsCdataDdlVstringByte; }
            set { _uwsCdataDdlVstringByte = value; }
        }

        public byte[] UwsCdataCompatibleDdlVstringByte {
            get { return _uwsCdataCompatibleDdlVstringByte; }
            set { _uwsCdataCompatibleDdlVstringByte = value; }
        }

        public byte[] UwsCdataDataVstringByte {
            get { return _uwsCdataDataVstringByte; }
            set { _uwsCdataDataVstringByte = value; }
        }

        public byte[] UwsCdataCompatibleDataVstringByte {
            get { return _uwsCdataCompatibleDataVstringByte; }
            set { _uwsCdataCompatibleDataVstringByte = value; }
        }

        public byte[] UwsCdataCollectionStateStringByte {
            get { return _uwsCdataCollectionStateStringByte; }
            set { _uwsCdataCollectionStateStringByte = value; }
        }

        public byte[] UwsCdataSampleStateStringByte {
            get { return _uwsCdataSampleStateStringByte; }
            set { _uwsCdataSampleStateStringByte = value; }
        }

        public byte[] UwsCdataSampleErrorStringByte {
            get { return _uwsCdataSampleErrorStringByte; }
            set { _uwsCdataSampleErrorStringByte = value; }
        }

        #endregion

        #region Collector Info

        private byte[] _uwsAccessorNameByte = new byte[64];
        private byte[] _uwsCollectorNameByte = new byte[64];
        private byte[] _uwsCollectorVstringByte = new byte[64];
        private byte[] _uwsCreatorNameByte = new byte[64];
        private byte[] _uwsHomeTerminalByte = new byte[48];
        private byte[] _uwsProcessNameByte = new byte[64];
        private byte[] _uwsProgramFileByte = new byte[64];
        private byte[] _uwsRunStringByte = new byte[240];
        private byte[] _uwsSwapVolumeByte = new byte[64];
        public int UwsAccessorId { get; set; }
        public string UwsAccessorName { get; set; }
        public string UwsCdataDataVstring { get; set; }
        public string UwsCollectorName { get; set; }
        public int UwsCollectorVersion { get; set; }
        public string UwsCollectorVstring { get; set; }
        public int UwsCreatorId { get; set; }
        public int UwsDelaySeconds { get; set; }
        public string UwsHomeTerminal { get; set; }
        public long UwsLaunchTimestamp { get; set; } // Juliantimestamp
        public int UwsPriority { get; set; }
        public string UwsProcessName { get; set; }
        public string UwsProgramFile { get; set; }
        public string UwsRunString { get; set; }
        public int UwsSamples { get; set; }
        public string UwsSwapVolume { get; set; }

        public byte[] UwsCollectorNameByte {
            get { return _uwsCollectorNameByte; }
            set { _uwsCollectorNameByte = value; }
        }

        public byte[] UwsCollectorVstringByte {
            get { return _uwsCollectorVstringByte; }
            set { _uwsCollectorVstringByte = value; }
        }

        public byte[] UwsCreatorNameByte {
            get { return _uwsCreatorNameByte; }
            set { _uwsCreatorNameByte = value; }
        }

        public byte[] UwsAccessorNameByte {
            get { return _uwsAccessorNameByte; }
            set { _uwsAccessorNameByte = value; }
        }

        public byte[] UwsRunStringByte {
            get { return _uwsRunStringByte; }
            set { _uwsRunStringByte = value; }
        }

        public byte[] UwsProcessNameByte {
            get { return _uwsProcessNameByte; }
            set { _uwsProcessNameByte = value; }
        }

        public byte[] UwsProgramFileByte {
            get { return _uwsProgramFileByte; }
            set { _uwsProgramFileByte = value; }
        }

        public byte[] UwsHomeTerminalByte {
            get { return _uwsHomeTerminalByte; }
            set { _uwsHomeTerminalByte = value; }
        }

        public byte[] UwsSwapVolumeByte {
            get { return _uwsSwapVolumeByte; }
            set { _uwsSwapVolumeByte = value; }
        }

        #endregion
    }
}