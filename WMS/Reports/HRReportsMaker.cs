using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WMS.Models;
using WMS.Reports;
using WMSLibrary;
using System.Linq.Dynamic;

namespace WMS.CustomClass
{
    public class HRReportsMaker
    {

      
            /// <summary>
        ///  This function takes FiltersModel and an empty list of AttDeptSummary as input and 
        ///  gives a populated AttDeptSummary as output. If there is any dept selected in the filter
        ///  it will iterate through all the departments if there is no dept selected it will try to
        ///  find all the depts of the company.
        /// </summary>
        /// <param name="fm"> FiltersModel </param>
        /// <param name="attDeptList">Empty List of AttDeptSummary</param>
        /// <returns>Populated List of AttDeptSummary</returns>
public  List<AttDeptSummary> GetListForAttDepartmentsSummary(FiltersModel fm, string _dateFrom,string _dateTo)
{
    List<AttDeptSummary> attDeptList = new List<AttDeptSummary>();
    TAS2013Entities db = new TAS2013Entities();
   //To get the query from the db
    QueryBuilder qb = new QueryBuilder();
    //Get the Department filters  
    List<FiltersAttributes> departments = new List<FiltersAttributes>();
    
    departments = fm.DepartmentFilter;
    //if more then 0 it means some departments are selected now we iterate over them and see 
    //how many employee does one dept has. The table empView is the most suitable db for this case.
    if (departments.Count > 0)
    {
        //do nothing still cater for it in future there might be a need
    }
    //if there is no department list in the filter then first get the company's department and then do the same as above.
    else
    {
        foreach (var comp in fm.CompanyFilter)
        {
            int compID = Convert.ToInt16(comp.ID);
            List<Department> depts = db.Departments.Where(aa => aa.CompanyID == compID).ToList();
            foreach (var dept in depts)
                departments.Add(new FiltersAttributes() { ID=dept.DeptID + "", FilterName=dept.DeptName });


        }
    }
    //NOw run the departments if they are from the department filter or from the company itself

    foreach (var dept in departments)
    {
        
        DataTable dt = qb.GetValuesfromDB("select * from EmpView where DeptID=" + dept.ID);
        List<EmpView> EmView = dt.ToList<EmpView>();
        
        //2015-01-24
       


            foreach (DateTime day in EachDay(Convert.ToDateTime(_dateFrom), Convert.ToDateTime(_dateTo)))
            {
                AttDeptSummary singleInstance = new AttDeptSummary();
                singleInstance.Department = dept.FilterName;
                singleInstance.TotalStrength = EmView.Count();
                singleInstance.Total = singleInstance.TotalStrength;
                singleInstance.Location = EmView.FirstOrDefault().LocName;
                singleInstance.Section = EmView.FirstOrDefault().SectionName;
                singleInstance.Company = EmView.FirstOrDefault().CompName;
                singleInstance.CardSwapped = 0;
                singleInstance.Absent = 0;
                singleInstance.OnLeave = 0;
                string dynamicQueryForEmps = GetDynamicQueryForEmps(EmView);
                string queryForCardSwapped = "select * from AttData where (TimeIn is not null AND AttDate = '" + day + "') AND ";
                string queryForAbsent = "select * from AttData where (StatusAB = 1 AND AttDate = '" + day + "') AND ";
                string queryOnLeave = "select * from AttData where (StatusHL = 1 OR StatusLeave =1  AND StatusSL=1 AND AttDate = '" + day + "') AND ";

               
                queryForCardSwapped = queryForCardSwapped + dynamicQueryForEmps;
                
                singleInstance.CardSwapped = qb.GetValuesfromDB(queryForCardSwapped).ToList<AttData>().Count();
                queryForAbsent = queryForAbsent + dynamicQueryForEmps;
                singleInstance.Absent = qb.GetValuesfromDB(queryForAbsent).ToList<AttData>().Count(); queryOnLeave = queryOnLeave + dynamicQueryForEmps;
                singleInstance.OnLeave = qb.GetValuesfromDB(queryOnLeave).ToList<AttData>().Count();

                //foreach (var emp in EmView)
                //{
                   
                //s= singleInstance.CardSwapped + db.AttDatas.Where(aa => aa.TimeIn != null && aa.AttDate == day && aa.EmpID == emp.EmpID).Count();
                //singleInstance.Absent = singleInstance.Absent+ db.AttDatas.Where(aa => aa.StatusAB == true && aa.AttDate == day && aa.EmpID == emp.EmpID).Count();
                //singleInstance.OnLeave = singleInstance.OnLeave + db.AttDatas.Where(aa => (aa.StatusHL == true || aa.StatusLeave == true || aa.StatusSL == true) && aa.AttDate == day && aa.EmpID == emp.EmpID).Count();


               
                //}
                singleInstance.date = day;
                attDeptList.Add(singleInstance);
            }



        





    }
    return attDeptList;
}

private string GetDynamicQueryForEmps(List<EmpView> EmView)
{
    string query = "(";
    foreach (var emp in EmView)
            query = query + "EmpID=" + emp.EmpID + " or ";
    if (query.Length > 4)
        query = query.Substring(0, query.Length - 4);
    query = query + ")";
    return query;
    
}


public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
{
    for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
        yield return day;
}
public static string PathString { get; set; }

public static string title { get; set; }
    }
}