using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.Web.Http.Cors;
using System.Threading.Tasks;
using GuildfordBoroughCouncil.Address.Models;

namespace GuildfordBoroughCouncil.Address.Api.Controllers
{
    /// <summary>
    /// A lookup service to support Twitter's TypeAhead.js - http://twitter.github.io/typeahead.js/
    /// </summary>
    [EnableCors("*", "*", "*")]
    [RoutePrefix("Typeahead")]
    public class TypeaheadV2Controller : ApiController
    {
        /// <summary>
        /// Lookup addresses by post code.
        /// </summary>
        /// <param name="q">The post code to lookup</param>
        /// <param name="IncludeHistorical">Include historical records?</param>
        /// <param name="Scope">What is the scope for the search?</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ByPostCode")]
        public async Task<JArray> ByPostCode(string q, bool IncludeHistorical = false, AddressSearchScope Scope = AddressSearchScope.National)
        {
            JArray PrefetchList = new JArray();

            (await Lookup.Data.ByPostCode(q, IncludeHistorical, Scope)).ToList().ForEach(a =>
            {

                List<string> Tokens = new List<string>();
                Tokens.Add(a.PostCode);

                dynamic o = new JObject();
                o.value = a.FullAddress;
                o.tokens = new JArray();

                foreach (var Token in Tokens.Distinct())
                {
                    var t = System.Text.RegularExpressions.Regex.Replace(Token, @"[^a-zA-Z0-9]", String.Empty);

                    if (!String.IsNullOrWhiteSpace(t))
                    {
                        o.tokens.Add(t);
                    }
                }

                o.data = JObject.FromObject(a);

                PrefetchList.Add(o);
            });

            return PrefetchList;
        }

        /// <summary>
        /// Lookup addresses by something.
        /// </summary>
        /// <param name="q">The query term (e.g. UPRN, USRN, post code, partial property address)</param>
        /// <param name="IncludeHistorical">Include historical records?</param>
        /// <param name="Scope">What is the scope for the search?</param>
        /// <returns></returns>
        [HttpGet]
        [Route("BySomething")]
        public async Task<JArray> BySomething(string q, bool IncludeHistorical = false, AddressSearchScope Scope = AddressSearchScope.Local)
        {
            JArray PrefetchList = new JArray();

            (await Lookup.Data.BySomething(q, IncludeHistorical, Scope)).ToList().ForEach(a =>
            {
                List<string> Tokens = new List<string>();
                Tokens.Add(a.PostCode);
                Tokens.Add(a.Property);
                Tokens.Add(a.Street);
                Tokens.Add(a.Organisation);

                dynamic o = new JObject();
                o.value = a.FullAddress;
                o.tokens = new JArray();

                foreach (var Token in Tokens.Distinct())
                {
                    var t = System.Text.RegularExpressions.Regex.Replace(Token, @"[^a-zA-Z0-9]", String.Empty);

                    if (!String.IsNullOrWhiteSpace(t))
                    {
                        o.tokens.Add(t);
                    }
                }

                o.data = JObject.FromObject(a);

                PrefetchList.Add(o);
            });

            return PrefetchList;
        }
    }
}
