using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lending.Business
{
    public class CollectionStatus
    {
        // get status
        public String getStatus(Boolean IsCleared, Boolean IsAbsent, Boolean IsPartiallyPaid, Boolean IsPaidInAdvanced, Boolean IsFullyPaid, Boolean IsOverdue)
        {
            String status = "?";
            if (IsCleared)
            {
                if (IsPaidInAdvanced)
                {
                    status = "Advance";
                }
                else
                {
                    if (IsFullyPaid)
                    {
                        status = "Full";
                    }
                    else
                    {
                        if (IsOverdue)
                        {
                            status = "Paid (Overdue)";
                        }
                        else
                        {
                            status = "Paid";
                        }
                    }
                }
            }
            else
            {
                if (IsAbsent)
                {
                    if (IsOverdue)
                    {
                        status = "Absent (Overdue)";
                    }
                    else
                    {
                        status = "Absent";
                    }
                }
                else
                {
                    if (IsPartiallyPaid)
                    {
                        if (IsOverdue)
                        {
                            status = "Partial (Overdue)";
                        }
                        else
                        {
                            status = "Partial";
                        }
                    }
                }
            }

            return status;
        }
    }
}