using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Threading.Tasks;
using GuildfordBoroughCouncil.Address.Models;

namespace GuildfordBoroughCouncil.Address.Api.Controllers
{
    /// <summary>
    /// Address lookup version 2.
    /// </summary>
    [EnableCors("http://juba.guildford.gov.uk,http://wellington.guildford.gov.uk,http://localhost:62625", "*", "*")]
    [RoutePrefix("2/Lookup")]
    public class LookupV2Controller : ApiController
    {
        /// <summary>
        /// Lookup addresses by UPRN.
        /// </summary>
        /// <param name="Uprn">The UPRN to lookup</param>
        /// <param name="Scope">What is the scope for the search?</param>
        /// <param name="IncludeHistorical">Include historical records for the UPRN</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ByUprn/{Uprn:long}")]
        public async Task<IEnumerable<Address.Models.Address>> ByUprn(Int64 Uprn, AddressSearchScope Scope = AddressSearchScope.National, bool IncludeHistorical = true)
        {
            // Always include historical
            return await Lookup.Data.ByUprn(Uprn, true, Scope);
        }

        /// <summary>
        /// Lookup addresses by USRN.
        /// </summary>
        /// <param name="Usrn">The USRN to lookup</param>
        /// <param name="Classifications">A list of classifications to filter by - defaults to Residential. Use "All" to include everything.</param>
        /// <param name="Scope">What is the scope for the search?</param>
        /// <param name="IncludeHistorical">Include historical records for the USRN</param>
        /// <param name="PostallyAddressable">Is the property postally addressable?</param>
        /// <returns></returns>
        [HttpGet]
        [Route("OnStreet/{Usrn:long}")]
        public async Task<IEnumerable<Address.Models.Address>> OnStreet(long Usrn, [FromUri]IEnumerable<string> Classifications, AddressSearchScope Scope = AddressSearchScope.National, bool IncludeHistorical = true, bool PostallyAddressable = true)
        {
            if (Classifications == null || Classifications.Count() == 0)
            {
                Classifications = new string[] { "Residential" };
            }
            else
            {
                // Remove empty entries
                Classifications = Classifications.Where(c => !string.IsNullOrWhiteSpace(c));

                if (Classifications.Contains("All"))
                {
                    Classifications = null;
                }
            }

            // Always include historical
            return await Lookup.Data.ByStreet(Usrn, true, Scope, Classifications, PostallyAddressable);
        }

        /// <summary>
        /// Lookup addresses by post code.
        /// </summary>
        /// <param name="Id">The post code to lookup</param>
        /// <param name="Classifications">A list of classifications to filter by - defaults to Residential. Use "All" to include everything.</param>
        /// <param name="Scope">What is the scope for the search?</param>
        /// <param name="IncludeHistorical">Include historical records?</param>
        /// <param name="PostallyAddressable">Is the property postally addressable?</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ByPostCode")]
        public async Task<IEnumerable<Address.Models.Address>> ByPostCode(string Id, [FromUri]IEnumerable<string> Classifications, AddressSearchScope Scope = AddressSearchScope.National, bool IncludeHistorical = false, bool PostallyAddressable = true)
        {
            if (Classifications == null || Classifications.Count() == 0)
            {
                Classifications = new string[] { "Residential" };
            }
            else
            {
                // Remove empty entries
                Classifications = Classifications.Where(c => !string.IsNullOrWhiteSpace(c));

                if (Classifications.Contains("All"))
                {
                    Classifications = null;
                }
            }

            return await Lookup.Data.ByPostCode(Id, IncludeHistorical, Scope, Classifications, PostallyAddressable);
        }

        /// <summary>
        /// Find the nearest addresses for the given co-ordinates.
        /// </summary>
        /// <param name="Scope">What is the scope for the search?</param>
        /// <param name="Long">Logitude</param>
        /// <param name="Lat">Latitude</param>
        /// <param name="Accuracy">Accuracy (in metres)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Nearest")]
        public async Task<IEnumerable<Address.Models.Address>> Nearest(AddressSearchScope Scope = AddressSearchScope.National, double Long = 0, double Lat = 0, double Accuracy = 10)
        {
            return await Lookup.Data.FindNearest(Long, Lat, Accuracy, Scope);
        }

        /// <summary>
        /// Lookup addresses by something.
        /// </summary>
        /// <param name="Query">The query term (e.g. UPRN, USRN, post code, partial property address)</param>
        /// <param name="Classifications">A list of classifications to filter by - defaults to Residential. Use "All" to include everything.</param>
        /// <param name="Scope">What is the scope for the search?</param>
        /// <param name="IncludeHistorical">Include historical records?</param>
        /// <param name="PostallyAddressable">Is the property postally addressable?</param>
        /// <returns></returns>
        [HttpGet]
        [Route("BySomething")]
        public async Task<IEnumerable<Address.Models.Address>> BySomething(string Query, [FromUri]IEnumerable<string> Classifications, AddressSearchScope Scope = AddressSearchScope.Local, bool IncludeHistorical = false, bool PostallyAddressable = true)
        {
            if (Classifications == null || Classifications.Count() == 0)
            {
                Classifications = new string[] { "Residential" };
            }
            else
            {
                // Remove empty entries
                Classifications = Classifications.Where(c => !string.IsNullOrWhiteSpace(c));

                if (Classifications.Contains("All"))
                {
                    Classifications = null;
                }
            }

            return await Lookup.Data.BySomething(Query, IncludeHistorical, Scope, Classifications, PostallyAddressable);
        }
    }
}
