using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using GuildfordBoroughCouncil.Address.Api.SinglePoint;
using DotNetCoords;
using GuildfordBoroughCouncil.Address.Models;
using GuildfordBoroughCouncil.Linq;

using System.Threading.Tasks;
using System.Data.Entity;

namespace GuildfordBoroughCouncil.Address.Api.Lookup
{
    public static class SearchResultItemExtension
    {
        private static string TryGetValue(this FieldInfo[] fi, string Tag)
        {
            var val = fi.Where(f => f.Tag == Tag).FirstOrDefault();

            if (val != null)
            {
                return val.Value;
            }

            return string.Empty;
        }

        public static GuildfordBoroughCouncil.Address.Models.Address ToAddress(this SearchResultItem result, AddressSearchScope Scope)
        {
            if (result != null)
            {
                var Address = new GuildfordBoroughCouncil.Address.Models.Address();

                string FormattedAddress;

                if (Scope == AddressSearchScope.Local)
                {
                    FormattedAddress = result.FieldItems.TryGetValue("FULL_ADDRESS");
                }
                else
                {
                    try
                    {
                        FormattedAddress = result.FieldItems.TryGetValue("DELIVERY_POINT_ADDRESS");
                    }
                    catch
                    {
                        FormattedAddress = result.TextToDisplay;
                    }
                }

                var Index = Math.Max(0, FormattedAddress.IndexOf(result.FieldItems.TryGetValue("STREET") + ","));


                Address.Uprn = Int64.Parse(result.FieldItems.TryGetValue("UPRN") ?? null);
                Address.Usrn = Int64.Parse(result.FieldItems.TryGetValue("USRN") ?? null);
                Address.Organisation = result.FieldItems.TryGetValue("ORGANISATION").ToLower().ToTitleCase();
                Address.Property = (!String.IsNullOrWhiteSpace(FormattedAddress) ? FormattedAddress.Substring(0, Index).ToLower().ToTitleCase() : String.Empty).Trim().TrimEnd(',');
                Address.Street = result.FieldItems.TryGetValue("STREET").ToLower().ToTitleCase();
                Address.Locality = result.FieldItems.TryGetValue("LOCALITY").ToLower().ToTitleCase();
                Address.Town = result.FieldItems.TryGetValue("TOWN").ToLower().ToTitleCase();
                Address.County = result.FieldItems.TryGetValue("COUNTY").ToLower().ToTitleCase();
                Address.PostTown = result.FieldItems.TryGetValue("POSTTOWN");
                Address.PostCode = result.FieldItems.TryGetValue("POSTCODE");
                Address.Country = "UK";

                Index = FormattedAddress.LastIndexOf(",");
                Address.FullAddress = (!String.IsNullOrWhiteSpace(FormattedAddress) ? FormattedAddress.Substring(0, Index).ToLower().ToTitleCase() + FormattedAddress.Substring(Index + 1) : Address.Street);


                Address.Classification = result.FieldItems.TryGetValue("CLASSIFICATION");
                Address.Distance = result.FieldItems.TryGetValue("DISTANCE").Replace(" Meter", "m");

                Address.Northing = Double.Parse(result.FieldItems.TryGetValue("NORTHING"));
                Address.Easting = Double.Parse(result.FieldItems.TryGetValue("EASTING"));

                var LatLong = new OSRef(Address.Easting.Value, Address.Northing.Value).ToLatLng();
                LatLong.ToWGS84();
                Address.Latitude = LatLong.Latitude;
                Address.Longitude = LatLong.Longitude;

                try
                {
                    using (var db = new Models.BluelightEntities())
                    {
                        var Blpu = db.BLPUs.Where(b => b.UPRN == Address.Uprn).SingleOrDefault();
                        Address.AuthorityCode = Blpu.LOCAL_CUSTODIAN;
                        Address.Authority = db.AUTHORITies.Where(a => a.AUTHORITY_REF == Address.AuthorityCode).SingleOrDefault().AUTHORITY_NAME.ToLower().ToTitleCase();
                    }
                }
                catch
                {
                    if (!Address.AuthorityCode.HasValue && Scope == AddressSearchScope.Local)
                    {
                        Address.AuthorityCode = Properties.Settings.Default.ThisAuthorityCode;
                        Address.Authority = Properties.Settings.Default.ThisAuthorityName;
                    }
                }

                switch(result.FieldItems.TryGetValue("LOGICAL_STATUS"))
                {
                    case "1":
                        Address.Status = AddressStatus.Active;
                        break;
                    case "3":
                        Address.Status = AddressStatus.Alternative;
                        break;
                    case "5":
                        Address.Status = AddressStatus.Candidate;
                        break;
                    case "8":
                        Address.Status = AddressStatus.Historic;
                        break;
                    case "6":
                        Address.Status = AddressStatus.Provisional;
                        break;
                    case "9":
                        Address.Status = AddressStatus.Rejected;
                        break;
                }

                return Address;
            }
            return null;
        }
    }

    public class Data
    {
        private static IEnumerable<int?> LocalPlusSurrounding
        {
            get
            {
                return Properties.Settings.Default.LocalPlusSurroundingCodes.Cast<int?>();
            }
        }

        private static async Task<IEnumerable<GuildfordBoroughCouncil.Address.Models.Address>> AdvancedSearch(string SearchText, AddressSearchScope Scope)
        {
            SearchServiceSoapClient SPClient = new SearchServiceSoapClient();
            SearchResultData Search = await SPClient.AdvancedSearchAsync("LLPG", SearchText);
            var ResultsScope = AddressSearchScope.Local;

            if (Search.Results.FullResultsCount == 0 && Scope != AddressSearchScope.Local)
            {
                Search = await SPClient.AdvancedSearchAsync("AddressBase", SearchText);
                ResultsScope = Scope;
            }

            return Search.Results.Items.Select(r => r.ToAddress(ResultsScope)).WhereIf(ResultsScope == AddressSearchScope.LocalPlusSurrounding, r => LocalPlusSurrounding.Contains(r.AuthorityCode));
        }

        public static async Task<IEnumerable<GuildfordBoroughCouncil.Address.Models.Address>> ByUprn(Int64 Uprn, bool IncludeHistorical = false, AddressSearchScope Scope = AddressSearchScope.Local)
        {
            return await AdvancedSearch("UPRN=" + Uprn.ToString() + ((IncludeHistorical) ? String.Empty : "|LOGICAL_STATUS=1"), Scope);
        }

        public static async Task<IEnumerable<GuildfordBoroughCouncil.Address.Models.Address>> ByStreet(Int64 Usrn, bool IncludeHistorical = false, AddressSearchScope Scope = AddressSearchScope.Local)
        {
            return await AdvancedSearch("USRN=" + Usrn.ToString() + ((IncludeHistorical) ? String.Empty : "|LOGICAL_STATUS=1"), Scope);
        }

        public static async Task<IEnumerable<GuildfordBoroughCouncil.Address.Models.Address>> ByPostCode(string PostCode, bool IncludeHistorical = false, AddressSearchScope Scope = AddressSearchScope.Local)
        {
            return await AdvancedSearch("POSTCODE=" + GuildfordBoroughCouncil.Address.PostCode.Format(PostCode) + ((IncludeHistorical) ? String.Empty : "|LOGICAL_STATUS=1"), Scope);
        }

        public static async Task<IEnumerable<GuildfordBoroughCouncil.Address.Models.Address>> BySomething(string Query, bool IncludeHistorical = false, AddressSearchScope Scope = AddressSearchScope.Local)
        {
            SearchServiceSoapClient SPClient = new SearchServiceSoapClient();
            var ResultsScope = AddressSearchScope.Local;
            SearchResultData Search = await SPClient.SearchAsync("LLPG", new QueryToken()
            {
                SearchText = new AnonymousSearchText()
                {
                    Value = Query
                },
                ReturnAllFields = true,

            });

            var Filter = new List<AddressStatus>()
            {
                AddressStatus.Active,
                AddressStatus.Alternative
            };

            if (IncludeHistorical)
            {
                Filter.Add(AddressStatus.Historic);
            }

            if (Search.Results.FullResultsCount == 0 && Scope != AddressSearchScope.Local)
            {
                Search = await SPClient.SearchAsync("AddressBase", new QueryToken()
                {
                    SearchText = new AnonymousSearchText()
                    {
                        Value = Query
                    },
                    ReturnAllFields = true,

                });

                ResultsScope = Scope;
            }

            return Search.Results.Items.Select(r => r.ToAddress(ResultsScope)).Where(r => Filter.Contains(r.Status));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Long"></param>
        /// <param name="Lat"></param>
        /// <param name="Distance">Any positive value</param>
        /// <returns></returns>
        public static async Task<IEnumerable<GuildfordBoroughCouncil.Address.Models.Address>> FindNearest(double Long, double Lat, double Distance, AddressSearchScope Scope = AddressSearchScope.Local)
        {
            var OS = new LatLng(Lat, Long).ToOSRef();

            SearchServiceSoapClient SPClient = new SearchServiceSoapClient();
            SearchResultData Search = await SPClient.SpatialRadialSearchByEastingNorthingAsync("LLPG", OS.Easting.ToString(), OS.Northing.ToString(), "Meter", Distance.ToString());
            var ResultsScope = AddressSearchScope.Local;

            if (Search.Results.FullResultsCount == 0 && Scope != AddressSearchScope.Local)
            {
                Search = await SPClient.SpatialRadialSearchByEastingNorthingAsync("AddressBase", OS.Easting.ToString(), OS.Northing.ToString(), "Meter", Distance.ToString());
                ResultsScope = Scope;
            }

            return Search.Results.Items.Select(r => r.ToAddress(ResultsScope)).WhereIf(ResultsScope == AddressSearchScope.LocalPlusSurrounding, r => LocalPlusSurrounding.Contains(r.AuthorityCode));
        }
    }
}