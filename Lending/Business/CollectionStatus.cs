using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lending.Business
{
    public class CollectionStatus
    {
        // get status
        public String getStatus(Boolean IsCleared, Boolean IsAbsent, Boolean IsPartialPayment, Boolean IsAdvancePayment, Boolean IsFullPayment, Boolean IsExtendCollection, Boolean IsOverdueCollection)
        {
            String status = "?";
            if (IsCleared)
            {
                if (IsAdvancePayment)
                {
                    status = "Advance";
                }
                else
                {
                    if (IsFullPayment)
                    {
                        status = "Full";
                    }
                    else
                    {
                        if (IsExtendCollection)
                        {
                            status = "Paid (Extend)";
                        }
                        else
                        {
                            if (IsOverdueCollection)
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
            }
            else
            {
                if (IsAbsent)
                {
                    if (IsExtendCollection)
                    {
                        status = "Absent (Extend)";
                    }
                    else
                    {
                        if (IsOverdueCollection)
                        {
                            status = "Absent (Overdue)";
                        }
                        else
                        {
                            status = "Absent";
                        }
                    }
                }
                else
                {
                    if (IsPartialPayment)
                    {
                        if (IsExtendCollection)
                        {
                            status = "Partial (Extend)";
                        }
                        else
                        {
                            if (IsOverdueCollection)
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
            }

            return status;
        }
    }
}