using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WMS.CustomClass
{
    public class CustomFunc
    {
        public bool IsAllLetters(string s)
        {
            //bool result = name.All(x => char.IsLetter(x) || x == ' ' || x == '.' || x == ',');
            bool result = s.All(x => char.IsLetter(x) || x == ' ');

            //foreach (char c in s)
            //{
            //    if (!Char.IsLetter(c))
            //        return false;
            //}
            return result;
        }


        
       
    }
    public class CustomFunction
    {

        internal static List<Models.Company> GetCompanies(List<Models.Company> list, Models.User LoggedInUser)
        {
            switch (LoggedInUser.RoleID)
            {
                case 1: 
                    break;
                case 2:
                    list = list.Where(aa=>aa.CompID==1 || aa.CompID==2).ToList();
                    break;
                case 3:
                  list=  list.Where(aa => aa.CompID>=3).ToList();
                    break;
                case 4:
                  list=  list.Where(aa => aa.CompID ==LoggedInUser.CompanyID).ToList();
                    break;
                case 5:
                    break;
            }
            return list;
        }

        internal static System.Collections.IEnumerable GetLocations(List<Models.Location> locations, List<Models.UserLocation> userLocations)
        {
            List<Models.Location> tempLocs = new List<Models.Location>();
            foreach (var item in userLocations)
            {
                tempLocs.AddRange(locations.Where(aa => aa.LocID == item.LocationID).ToList());
            }

            return tempLocs;
        }

        internal static List<Models.Shift> GetShifts(List<Models.Shift> shifts, List<Models.UserLocation> userLocation)
        {
            List<Models.Shift> tempShifts = new List<Models.Shift>();
            foreach (var item in userLocation)
            {
                tempShifts.AddRange(shifts.Where(aa => aa.LocationID == item.LocationID).ToList());
            }

            return tempShifts;
        }

        internal static List<Models.Crew> GetCrewList(Models.User LoggedInUser, List<Models.Crew> crews)
        {
            switch (LoggedInUser.RoleID)
            {
                case 1:
                    break;
                case 2:
                    crews = crews.Where(aa => aa.CompanyID == 1 || aa.CompanyID == 2).ToList();
                    break;
                case 3:
                    crews = crews.Where(aa => aa.CompanyID >= 3).ToList();
                    break;
                case 4:
                    crews = crews.Where(aa => aa.CompanyID == LoggedInUser.CompanyID).ToList();
                    break;
                case 5:
                    break;
            }
            return crews;
        }

        internal static List<Models.Section> GetSections(Models.User LoggedInUser, List<Models.Section> sections)
        {
            switch (LoggedInUser.RoleID)
            {
                case 1:
                    break;
                case 2:
                    sections = sections.Where(aa => aa.CompanyID == 1 || aa.CompanyID == 2).ToList();
                    break;
                case 3:
                    sections = sections.Where(aa => aa.CompanyID >= 3).ToList();
                    break;
                case 4:
                    sections = sections.Where(aa => aa.CompanyID == LoggedInUser.CompanyID).ToList();
                    break;
                case 5:
                    break;
            }
            return sections;
        }
    }
}