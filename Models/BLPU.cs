//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GuildfordBoroughCouncil.Address.Api.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BLPU
    {
        public double UPRN { get; set; }
        public Nullable<short> BLPU_VER { get; set; }
        public bool BLPU_CUR { get; set; }
        public int ENTRY_DATE { get; set; }
        public int LAST_UPDATE_DATE { get; set; }
        public int START_DATE { get; set; }
        public Nullable<int> END_DATE { get; set; }
        public Nullable<decimal> EASTING { get; set; }
        public Nullable<decimal> NORTHING { get; set; }
        public byte RPA { get; set; }
        public int LOCAL_CUSTODIAN { get; set; }
        public byte LOGICAL_STATUS { get; set; }
        public string ORGANISATION_MATCH_STATUS { get; set; }
        public string PARTIAL_MATCH_STATUS { get; set; }
        public int MAPINFO_ID { get; set; }
        public Nullable<int> PRO_ORDER { get; set; }
        public string CHANGE_TYPE { get; set; }
        public bool ISPARENT { get; set; }
        public bool ISCHILD { get; set; }
        public short IE_NO { get; set; }
        public Nullable<bool> NEVERCHILD { get; set; }
        public bool NEVEREXPORT { get; set; }
        public Nullable<bool> UPDATED { get; set; }
        public Nullable<short> BLPU_STATE { get; set; }
        public Nullable<int> BLPU_STATE_DATE { get; set; }
        public string BLPU_CLASS { get; set; }
        public Nullable<double> PARENT_UPRN { get; set; }
        public string ORGANISATION { get; set; }
        public string WARD_CODE { get; set; }
        public string PARISH_CODE { get; set; }
        public Nullable<short> CUSTODIAN_ONE { get; set; }
        public Nullable<short> CUSTODIAN_TWO { get; set; }
        public string CAN_KEY { get; set; }
        public System.DateTime LAST_UPDATED { get; set; }
        public string CROSS_REF_DETAILS { get; set; }
        public string USAGE { get; set; }
        public byte ACCESS_POINT { get; set; }
        public Nullable<byte> FRC_STATUS { get; set; }
        public Nullable<int> FRS_ID { get; set; }
        public Nullable<int> RESPONSIBLE_FRS_ID { get; set; }
        public string POSTAL_ADDRESS { get; set; }
        public string POSTCODE_LOCATOR { get; set; }
        public Nullable<short> MULTI_OCC_COUNT { get; set; }
    }
}