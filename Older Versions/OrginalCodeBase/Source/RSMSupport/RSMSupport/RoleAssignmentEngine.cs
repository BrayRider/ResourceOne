using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RSM.Support;
using RSM.Support.S2;


namespace RSM.Support
{
    public class RoleAssignmentEngine
    {
        RSMDataModelDataContext _context;
       

        public RoleAssignmentEngine()
        {
            _context = new RSMDataModelDataContext();
        }

        public RoleAssignmentEngine(RSMDataModelDataContext context)
        {
            _context = context;
        }

        public bool ProcessDirtyPeople(S2API api)
        {
            // Loop through all the people that have changes that effect rules and process them

            var people = (from p in _context.Persons
                          where p.NeedsRulePass == true
                          select p);
            foreach (Person person in people)
            {
                ProcessPerson(person, api);
            }

            return true;
        }


        public bool ProcessPerson(int id)
        {

            Person person = (from p in _context.Persons
                             where p.PersonID == id
                             select p).Single();

            return ProcessPerson(person, null);
        }

        public IQueryable<Person> PeopleWithRule(int ruleID)
        {
            var theRule = (from r in _context.JCLRoleRules
                           where r.ID == ruleID
                           select r).Single();

            IQueryable<Person> people = null;

            int mask = 0;

            if (theRule.Departments.DeptID  != "0")
                mask = 1;

            if (theRule.Job.JobCode != "0")
                mask |= 2;

            if (theRule.Locations.LocationID > 0)
                mask |= 4;

            switch (mask)
            {
                case 1:
                    // Just department
                    people = (from p in _context.Persons
                              where p.DeptID == theRule.Departments.DeptID
                              select p);
                    break;
                case 2:
                    // Just Job
                    people = (from p in _context.Persons
                              where p.JobCode == theRule.Job.JobCode
                              select p);
                    break;
                case 3:
                    // Job and Dept
                    people = (from p in _context.Persons
                              where p.JobCode == theRule.Job.JobCode &&
                                    p.DeptID == theRule.Departments.DeptID
                              select p);
                    break;
                case 4:
                    // Just location
                    people = (from p in _context.Persons
                              where p.Facility == theRule.Locations.LocationName 
                              select p);
                    break;
                case 5:
                    // Location and department
                    people = (from p in _context.Persons
                              where p.Facility == theRule.Locations.LocationName &&
                                    p.DeptID == theRule.Departments.DeptID
                              select p);
                    break;
                case 6:
                    // Location and job
                    people = (from p in _context.Persons
                              where p.Facility == theRule.Locations.LocationName &&
                                    p.JobCode == theRule.Job.JobCode
                              select p);
                    break;
                case 7:
                    // The rule is specific on all three fields
                    people = (from p in _context.Persons
                              where p.Facility == theRule.Locations.LocationName &&
                                    p.JobCode == theRule.Job.JobCode &&
                                    p.DeptID == theRule.Departments.DeptID
                              select p);
                    break;


            }

            return people;
        }

        public int FlagPeopleWithRule(int ruleID, bool needsApproval)
        {
            IQueryable<Person> people;
            try
            {
                 people  = PeopleWithRule(ruleID);
            }
            catch
            {
                return 0;
            }

            int count = 0;

            foreach (Person p in people)
            {
                count++;
                p.NeedsRulePass = true;
                //p.NeedsApproval = true;
            }

            _context.SubmitChanges();
            return count;
        }

        public int ProcessPeopleWithRule(int ruleID)
        {

            var people = PeopleWithRule(ruleID);

            return Enumerable.Count(people, p => ProcessPerson(p, null));
        }

        public bool ProcessPerson(Person person, S2API api)
        {

            // First let's remove any existing automatically added role.
            var thisPersonRoles = from r in _context.PeopleRoles
                                  where r.PersonID == person.PersonID
                                  select r;
            if (thisPersonRoles.Count() > 0)
            {
                _context.PeopleRoles.DeleteAllOnSubmit(thisPersonRoles);
            }

            if (person.Active)
            {
                // Find the rules that apply to this person.
                var rules = (from r in _context.JCLRoleRules 
                             where (  ((r.JobCode == "0")  || (r.JobCode == person.JobCode))
                                   && ((r.DeptID == "0")   || (r.DeptID == person.DeptID ))
                                   && ((r.Location == 0) || (r.Locations.LocationName  == person.Facility )))
                             select r);

                
                // Assemble the roles into a distinct list
                HashSet<int> distinctRoleIDs = new HashSet<int>();
                HashSet<Role> distinctRoles = new HashSet<Role>();
                foreach (JCLRole role in rules.SelectMany(rule => rule.JCLRoles))
                {
                    distinctRoles.Add(role.Role);
                    distinctRoleIDs.Add(role.RoleID);
                }

                if (api == null)
                    person.NeedsUpload = true;

                person.NeedsRulePass = false;
                // Add them to the employee
                foreach (Role role in distinctRoles)
                {
                    person.PeopleRoles.Add(new PeopleRole(person, role));
                }
            }
            else
            {
                if (api == null)
                    person.NeedsUpload = true;

                person.NeedsRulePass = false;
            }

            _context.SubmitChanges();
           
            if (api != null)
            {
                try
                {
                    api.SavePerson(person);
                    person.NeedsUpload = false;
                    _context.SubmitChanges();
                }
                catch (Exception)
                {
                }
            }

            return true;
        }



    }
}
