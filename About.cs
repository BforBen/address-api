using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GuildfordBoroughCouncil.Address.Api
{
    public static class About
    {
        public static string Name
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title.Length > 0)
                    {
                        return titleAttribute.Title;
                    }
                }
                return String.Empty;
            }
        }

        public static string Version
        {
            get
            {
                var Ver = Assembly.GetExecutingAssembly().GetName().Version;
                return String.Format("{0}.{1}.{2}", Ver.Major, Ver.Minor, Ver.Build);
            }
        }
        
        [Display(Name = "Build date")]
        [DisplayFormat(DataFormatString = "{0:dddd d MMMM yyyy}")]
        public static DateTime BuildDate
        {
            get
            {
                var Ver = Assembly.GetExecutingAssembly().GetName().Version;
                return new DateTime(2000, 01, 01).AddDays(Ver.Build);
            }
        }

        [Display(Name = "Build date")]
        public static string BuildDateAsString
        {
            get
            {

                return String.Format("{0:d MMMM yyyy}", BuildDate);
            }
        }
    }
}
