using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Threading.Tasks;
using GuildfordBoroughCouncil.Address.Models;

namespace GuildfordBoroughCouncil.Address.Api.Controllers
{
    /// <summary>
    /// Address lookup version 1. - *DEPRECATED*
    /// </summary>
    [EnableCors("*", "*", "*")]
    [RoutePrefix("Lookup")]
    //[RoutePrefix("v1/Lookup")]
    [Obsolete]
    public class LookupV1Controller : ApiController
    {
        /// <summary>
        /// Lookup addresses by UPRN.
        /// </summary>
        /// <param name="Uprn">The UPRN to lookup</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ByUprn/{Uprn:long}")]
        public async Task<IEnumerable<Address.Models.Address>> ByUprn(long Uprn)
        {
            // Always include historical
            return await Lookup.Data.ByUprn(Uprn, true, AddressSearchScope.National);
        }

        /// <summary>
        /// Lookup addresses by USRN.
        /// </summary>
        /// <param name="Usrn">The USRN to lookup</param>
        /// <param name="CheckNational">If no results are found in the LLPG, should AddressBase be searched?</param>
        /// <param name="IncludeHistorical">Include historical records for the USRN</param>
        /// <returns></returns>
        [HttpGet]
        [Route("OnStreet/{Usrn:long}")]
        public async Task<IEnumerable<Address.Models.Address>> OnStreet(long Usrn, bool CheckNational = false, bool IncludeHistorical = true)
        {
            var Scope = AddressSearchScope.Local;

            if (CheckNational)
            {
                Scope = AddressSearchScope.National;
            }

            // Always include historical
            return await Lookup.Data.ByStreet(Usrn, true, Scope, new string[] { "Residential" });
        }

        /// <summary>
        /// Lookup addresses by post code.
        /// </summary>
        /// <param name="Id">The post code to lookup</param>
        /// <param name="CheckNational">If no results are found in the LLPG, should AddressBase be searched?</param>
        /// <param name="IncludeHistorical">Include historical records?</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ByPostCode")]
        public async Task<IEnumerable<Address.Models.Address>> ByPostCode(string Id, bool CheckNational = false, bool IncludeHistorical = false)
        {
            var Scope = AddressSearchScope.Local;

            if (CheckNational)
            {
                Scope = AddressSearchScope.National;
            }

            return await Lookup.Data.ByPostCode(Id, IncludeHistorical, Scope, new string[] { "Residential" });
        }

        /// <summary>
        /// Find the nearest addresses for the given co-ordinates.
        /// </summary>
        /// <param name="CheckNational">If no results are found in the LLPG, should AddressBase be searched?</param>
        /// <param name="Long">Logitude</param>
        /// <param name="Lat">Latitude</param>
        /// <param name="Accuracy">Accuracy (in metres)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Nearest")]
        public async Task<IEnumerable<GuildfordBoroughCouncil.Address.Models.Address>> Nearest(bool CheckNational = false, double Long = 0, double Lat = 0, double Accuracy = 10)
        {
            var Scope = AddressSearchScope.Local;

            if (CheckNational)
            {
                Scope = AddressSearchScope.National;
            }

            return await Lookup.Data.FindNearest(Long, Lat, Accuracy, Scope);
        }

        /// <summary>
        /// Lookup addresses by something.
        /// </summary>
        /// <param name="Query">The query term (e.g. UPRN, USRN, post code, partial property address)</param>
        /// <param name="CheckNational">If no results are found in the LLPG, should AddressBase be searched?</param>
        /// <param name="IncludeHistorical">Include historical records?</param>
        /// <returns></returns>
        [HttpGet]
        [Route("BySomething")]
        public async Task<IEnumerable<Address.Models.Address>> BySomething(string Query, bool CheckNational = false, bool IncludeHistorical = false)
        {
            var Scope = AddressSearchScope.Local;

            if (CheckNational)
            {
                Scope = AddressSearchScope.National;
            }

            return await Lookup.Data.BySomething(Query, IncludeHistorical, Scope, new string[] { "Residential" });
        }
    }
}
